using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Actor {
	// settings:

	[Header("Jump")]
	public float jumpSpeed;
	public float jumpExtendSpeed;
	public float jumpExtendTime;
	public int coyoteFrames, jumpAheadFrames;

	[Header("Wall jump")]
	public float wallJumpAngle;
	public float wallJumpDuration = 0.2f;

	[Header("Visual")]
	public PlayerAnimator animator;

	// properties:

	public bool canDoubleJump {
		get;
		set;
	}

	// state:

	private bool dead;
	private bool didDoubleJump;
	private float jumpTime;

	// inputs:

	private bool jump;
	private bool holdingJump;

	// animation state:

	private bool jumped, wallJumped;

	// misc:

	private CameraController cameraController;
	private Level level;

	// visuals:

	private float lastFixedUpdateTime; 

	protected override void Init() {
		this.level = GameObject.FindObjectOfType<Level>();
		this.cameraController = GameObject.FindObjectOfType<CameraController>();
	}

	public override void Spawn(Vector3 location) {
		base.Spawn(location);

		this.dead = false;
	}

	public void Update() {
		HandleInput();

		Vector2 localPos = this.animator.transform.localPosition;
		localPos.x = Mathf.Abs(this.velocity.x) * (Time.time - this.lastFixedUpdateTime);
		localPos.y = this.velocity.y * (Time.time - this.lastFixedUpdateTime);
		this.animator.transform.localPosition = localPos;
	}

	public void OnTriggerEnter2D(Collider2D trigger) {
		if(trigger.tag == "Spikes") {
			Die();
		}
	}

	protected override void BeforeMovementPhase() {
		ResetAnimation();

		if(this.dead) {
			ResetInput();
			this.jumpFrames = 100;
			this.groundFrames = 100;
			this.input = Vector2.zero;
		}

		if(this.grounded) {
			this.didDoubleJump = false;
		}
	}

	protected override void MidMovementPhase() {
		// jump:

		if(this.grounded || this.wallSliding) {
			if(this.jump || this.jumpFrames < this.jumpAheadFrames) { // normal jump or wall jump
				Jump();
			}
		} else {
			if(this.jump && this.groundFrames < this.coyoteFrames) { // normal jump or wall jump
				Jump();
			} else if(this.jump && this.canDoubleJump && !this.didDoubleJump) { // double jump
				this.didDoubleJump = true;
				Jump();
			} else if(this.holdingJump) {
				ExtendJump();
			}
		}
	}

	protected override void AfterMovementPhase() {
		// camera:

		if(this.facingRight) {
			this.cameraController.SetFocus(false, true);
		} else {
			this.cameraController.SetFocus(true, false);
		}

		if(this.grounded || this.wallSliding) {
			this.cameraController.targetY = this.transform.position.y;
		}

		this.cameraController.targetVelocity = this.velocity;// + this.inheritedVelocity;

		if(Mathf.Abs(this.input.y) > 0.5f && this.velocity.magnitude < 0.1f) {
			if(this.input.y > 0) {
				this.cameraController.lookOffset = Vector2.up * 5f;
			} else {
				this.cameraController.lookOffset = Vector2.down * 20f;
			}
		} else {
			this.cameraController.lookOffset = Vector2.zero;
		}

		// animation:

		UpdateAnimation();

		// misc:

		ResetInput();
		
		this.jumpFrames++;
		this.groundFrames++;

		this.lastFixedUpdateTime = Time.time;
	}

	private void Jump() {
		if(this.lastWasWall) { // wall jump
			this.velocity.x = Mathf.Sign(this.wallNormalX) * Mathf.Cos(this.wallJumpAngle * Mathf.Deg2Rad) * this.jumpSpeed;
			this.velocity.y = Mathf.Sin(this.wallJumpAngle * Mathf.Deg2Rad) * this.jumpSpeed;
		
			this.forceMoveTimer = this.wallJumpDuration;
			this.forceMoveX = Mathf.Sign(this.wallNormalX);

			this.wallJumped = true;
		} else {
			this.velocity.y = this.jumpSpeed;
		
			this.jumped = true;
		}

		this.jumpTime = Time.time;

		this.jumpFrames += 100;
		this.groundFrames += 100;
	}

	private void ExtendJump() {
		if(Time.time - this.jumpTime > this.jumpExtendTime) {
			return;
		}

		float d = 1f - (Time.time - this.jumpTime) / this.jumpExtendTime;

		this.velocity.y += this.jumpExtendSpeed * d;
	}

	private void ResetAnimation() {
		this.jumped = false;
		this.wallJumped = false;
	}

	private void UpdateAnimation() {
		this.animator.Refresh(this.velocity, this.input, this.grounded, this.wallSliding, this.jumped, this.wallJumped);
	}

	private void Die() {
		if(this.dead) {
			return;
		}

		this.dead = true;
		this.level.PlayerDied();
	}

	private void HandleInput() {
		if(Input.GetButtonDown("Jump")) {
			this.jump = true;
			
			this.jumpFrames = 0;
		} else {
			this.jump = false;
		}

		this.holdingJump = Input.GetButton("Jump");

		this.input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

		// deadzone (30%):
		this.input.x = Mathf.Abs(this.input.x) > 0.3f ? this.input.x : 0;
		this.input.y = Mathf.Abs(this.input.y) > 0.3f ? this.input.y : 0; 
	}

	private void ResetInput() {
		this.jump = false;
	}
}
