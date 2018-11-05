using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Actor : MonoBehaviour {
	// settings:

	[Header("Movement")]
	public float moveSpeed;
	public float airAccelerationTime;
	[Range(0, 1f)]
	public float groundFriction = 0.05f;

	// constants:
	private const float LOOK_AHEAD_DIST = 0.01f;
	private const float MIN_GROUND_NORMAL_Y = 0.65f;

	// state:
	protected Vector2 velocity;
	protected float velocityXRef;

	protected bool grounded;
	protected Vector2 groundNormal;
	protected bool wallSliding;
	protected bool lastWasWall;
	protected float wallNormalX;
	protected int groundFrames = 100; // number of frames since player touched ground or wall
	protected int jumpFrames = 100; // number of frames since last jump keypress

	protected float forceMoveTimer = 0f;
	protected float forceMoveX;

	protected bool facingRight;

	// inputs:
	protected Vector2 input;

	// misc:
	protected Rigidbody2D rigid;

	private ContactFilter2D contactFilter;
	private RaycastHit2D[] raycastResults;

	public void Awake() {
		this.rigid = this.gameObject.GetComponent<Rigidbody2D>();

		this.contactFilter = new ContactFilter2D();
		this.contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(this.gameObject.layer));
		this.contactFilter.useLayerMask = true;
		this.contactFilter.useTriggers = false;

		this.raycastResults = new RaycastHit2D[32];

		Init();
	}

	protected abstract void Init();

	public virtual void Spawn(Vector3 location) {
		this.transform.position = location;

		// reset state:
		this.velocity = Vector2.zero;
		this.velocityXRef = 0;
		this.grounded = false;
		this.groundNormal = Vector2.zero;
		this.wallSliding = false;
		this.lastWasWall = false;
		this.wallNormalX = 0;
		this.groundFrames = 100;
		this.jumpFrames = 100;
		this.forceMoveTimer = 0f;
		this.facingRight = true;
	}

	public void FixedUpdate() {
		BeforeMovementPhase();

		// handle horizontal movement:

		this.wallSliding = false;

		Vector2 moveVector;

		if(this.grounded) {
			this.velocity.x *= 1f - this.groundFriction;

			if(Mathf.Abs(this.input.x) > 0.1f) {
				this.velocity.x = this.input.x * this.moveSpeed;
			}
			
			moveVector = new Vector2(this.groundNormal.y, -this.groundNormal.x) * this.velocity.x * Time.deltaTime;
		
			this.velocityXRef = 0;
		} else {
			if(Mathf.Abs(this.input.x) > 0.1f) { // air control
				this.velocity.x = Mathf.SmoothDamp(
					this.velocity.x,
					this.moveSpeed * (this.forceMoveTimer > 0 ? this.forceMoveX : this.input.x),
					ref this.velocityXRef,
					this.airAccelerationTime
				);
			}

			moveVector = Vector2.right * this.velocity.x * Time.deltaTime;
		}

		this.forceMoveTimer -= Time.deltaTime;

		float dist = moveVector.magnitude;

		int c = this.rigid.Cast(moveVector, this.contactFilter, this.raycastResults, dist + LOOK_AHEAD_DIST);

		for(int i = 0; i < c; i++) {
			RaycastHit2D hit = this.raycastResults[i];
			Vector2 normal = hit.normal;

			if(Mathf.Abs(normal.y) < MIN_GROUND_NORMAL_Y && !this.grounded) {
				this.wallSliding = true;
				this.lastWasWall = true;
				this.groundFrames = 0;
				this.wallNormalX = normal.x;
	
				normal.y = 0;
			}

			float p = Vector2.Dot(velocity, normal);
			if(p < 0) {
				this.velocity -= p * normal;
			}

			if(hit.distance - LOOK_AHEAD_DIST < dist) {
				dist = hit.distance - LOOK_AHEAD_DIST;
			}
		}

		this.rigid.position += moveVector.normalized * dist;

		// gravity:

		this.velocity += Physics2D.gravity * Time.deltaTime;

		// mid movement:

		MidMovementPhase();

		// handle vertical movement:
		
		this.grounded = false;

		Vector2 deltaY = Vector2.up * this.velocity.y * Time.deltaTime;

		dist = Mathf.Abs(deltaY.y);

		c = this.rigid.Cast(deltaY, this.contactFilter, this.raycastResults, dist + LOOK_AHEAD_DIST);

		for(int i = 0; i < c; i++) {
			RaycastHit2D hit = this.raycastResults[i];
			Vector2 normal = hit.normal;

			if(normal.y > MIN_GROUND_NORMAL_Y) {
				this.grounded = true;
				this.lastWasWall = false;
				this.groundFrames = 0;
				this.groundNormal = normal;

				normal.x = 0;
			}

			float p = Vector2.Dot(velocity, normal);
			if(p < 0) {
				this.velocity -= p * normal;
			}

			if(hit.distance - LOOK_AHEAD_DIST < dist) {
				dist = hit.distance - LOOK_AHEAD_DIST;
			}
		}

		this.rigid.position += deltaY.normalized * dist;

		// facing & cam:

		if(this.velocity.x > float.Epsilon && !facingRight) {
			this.facingRight = true;
			this.transform.eulerAngles = new Vector3(0, 0, 0);
		} else if(this.velocity.x < -float.Epsilon && facingRight) {
			this.facingRight = false;
			this.transform.eulerAngles = new Vector3(0, 180f, 0);
		}

		AfterMovementPhase();
	}

	// called from FixedUpdate before any movement is resolved
	protected virtual void BeforeMovementPhase() {
		
	}

	// called from FixedUpdate in the middle of movement phase (after horizontal & before vertical)
	protected virtual void MidMovementPhase() {
		
	}

	// called from FixedUpdate in after movement resolution
	protected virtual void AfterMovementPhase() {
		
	}
}
