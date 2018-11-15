using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
	public float horizontalZone;
	public float verticalZone;
	public float verticalZoneLowerMultiplier;
	public float lookInFront;
	public float horizontalSpeed;
	public float verticalSpeed;
	public float verticalOffset;

	[Header("Bg")]
	public Transform background;
	public Vector2 parallaxMul;

	[Header("Drama effect")]
	public Gradient dramaColors;

	private float _targetY;
	public float targetY {
		get {
			return this._targetY + this.lookOffset.y;
		}
		set {
			this._targetY = value;
		}
	}

	private Vector2 _targetVelocity;
	public Vector2 targetVelocity {
		get {
			return this._targetVelocity;
		}
		set {
			this._targetVelocity = value;
			this.lastTargetUpdateTime = Time.time;
		}
	}

	private Bounds _bounds;
	public Bounds bounds {
		get {
			return this._bounds;
		}
		set {
			this._bounds = value;
			this._bounds.extents = this._bounds.extents * 0.5f - this.cameraExtents;
		}
	}

	public Vector2 lookOffset {
		get;
		set;
	}

	private Camera cam;
	private PixelPerfectCamera pixelPerfectCamera;

	private Vector3 cameraExtents;
	
	private float horizontalOffset;
	private float offset;

	private float verticalZoneWorld;

	private Transform target;
	private bool followY;
	private bool focusRight, focusLeft;

	private float lastTargetUpdateTime;
	private float horizontalVelocityOffset;

	private Vector2 startPosition;
	private Vector3 backgroundStartPosition;

	// drama:
	private Color defaultClearColor;
	private float dramaStartTime;
	private float dramaDuration;
	private bool isDrama {
		get {
			return Time.realtimeSinceStartup - this.dramaStartTime <= this.dramaDuration;
		}
	}

	public void Awake() {
		this.cam = this.gameObject.GetComponent<Camera>();
		this.pixelPerfectCamera = this.gameObject.GetComponent<PixelPerfectCamera>();
		
		this.defaultClearColor = this.cam.backgroundColor;

		this.cameraExtents = new Vector2(this.pixelPerfectCamera.refResolutionX, this.pixelPerfectCamera.refResolutionY) / this.pixelPerfectCamera.assetsPPU * 0.5f;

		this.horizontalOffset = this.cam.ViewportToWorldPoint(new Vector3(0.5f + this.lookInFront, 0.5f)).x - this.cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)).x;
		this.verticalZoneWorld = this.cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f + this.verticalZone)).y - this.cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)).y;
	
		this.startPosition = this.transform.position;
		this.backgroundStartPosition = this.background.position;
	}

	public void SetTarget(Transform target, bool followY) {
		this.target = target;
		this.followY = followY;
		if(!followY) {
			this.targetY = target.transform.position.y;
		}

		this.targetVelocity = Vector2.zero;
		this.horizontalVelocityOffset = 0;
	}

	public void SetFocus(bool right, bool left) {
		if(right && !this.focusRight) {
			this.offset = 1;
		} else if(left && !this.focusLeft) {
			this.offset = -1;
		} else if(!left && !right) {
			this.offset = 0;
		}

		this.focusRight = right;
		this.focusLeft = left;
	}

	public void Update() {
		if(this.target == null) {
			return;
		}

		HandleDramaEffect();

		Vector2 delta = Vector2.zero;

		Vector2 targetPos = this.target.position;
		targetPos += this.targetVelocity * (Time.time - this.lastTargetUpdateTime);

		if(Mathf.Sign(this.targetVelocity.x) != Mathf.Sign(this.horizontalVelocityOffset)) {
			this.horizontalVelocityOffset = 0;
		} 
		this.horizontalVelocityOffset = 0.85f * this.horizontalVelocityOffset + 0.15f * this.targetVelocity.x;

		Vector2 currentPos = this.transform.position;

		float lowerX = 0.5f;
		float upperX = 0.5f;

		if(this.focusRight) {
			upperX += this.horizontalZone;
			targetPos.x += this.offset * this.horizontalOffset;// + this.horizontalVelocityOffset;
		} else if(this.focusLeft) {
			lowerX -= this.horizontalZone;
			targetPos.x += this.offset * this.horizontalOffset;// + this.horizontalVelocityOffset;
		} else {
			lowerX -= this.horizontalZone / 2f;
			upperX += this.horizontalZone / 2f;
		}

		targetPos += this.lookOffset;

		Vector2 targetViewPos = this.cam.WorldToViewportPoint(targetPos);

		// Debug.DrawLine(new Vector3(targetPos.x, -100), new Vector3(targetPos.x, 100), Color.red);
		// Debug.DrawLine(this.cam.ViewportToWorldPoint(new Vector3(lowerX, 0, 10)), this.cam.ViewportToWorldPoint(new Vector3(lowerX, 1, 10)), Color.green);
		// Debug.DrawLine(this.cam.ViewportToWorldPoint(new Vector3(upperX, 0, 10)), this.cam.ViewportToWorldPoint(new Vector3(upperX, 1, 10)), Color.green);

		// Debug.DrawLine(new Vector3(-10000, this.followY ? this.targetY : targetPos.y), new Vector3(10000, this.followY ? this.targetY : targetPos.y), Color.red);
		// Debug.DrawLine(new Vector3(-10000, this.targetY + this.verticalZoneWorld, 0), new Vector3(10000, this.targetY + this.verticalZoneWorld, 0), Color.blue);
		// Debug.DrawLine(new Vector3(-10000, this.targetY - this.verticalZoneWorld * this.verticalZoneLowerMultiplier, 0), new Vector3(10000, this.targetY - this.verticalZoneWorld * this.verticalZoneLowerMultiplier, 0), Color.blue);

		if(targetViewPos.x < lowerX || targetViewPos.x > upperX) {
			delta.x += (targetPos.x - currentPos.x) * this.horizontalSpeed;
		}

		delta.y += this.verticalOffset;

		if(this.followY) {
			delta.y += (targetPos.y - currentPos.y) * this.verticalSpeed;
		} else {
			if(targetPos.y > this.targetY + this.verticalZoneWorld) {
				this.targetY += (targetPos.y - this.targetY - this.verticalZoneWorld) * this.verticalSpeed;
			} else if(targetPos.y < this.targetY - this.verticalZoneWorld * this.verticalZoneLowerMultiplier) {
				this.targetY += (targetPos.y - this.targetY + this.verticalZoneWorld * this.verticalZoneLowerMultiplier) * this.verticalSpeed;
			}

			delta.y += (this.targetY - currentPos.y) * this.verticalSpeed;
		}

		if(this.isDrama) {
			delta *= 10f;
		}

		Vector3 pos = this.transform.position + new Vector3(delta.x, delta.y) * Time.deltaTime;
		MoveTo(pos);
	}

	public void StartDrama(float duration) {
		this.background.gameObject.SetActive(false);
	
		this.dramaStartTime = Time.realtimeSinceStartup;
		this.dramaDuration = duration;

		// Vector2 targetPos = new Vector2(
		// 	this.transform.position.x,
		// 	this.target.position.y + this.verticalOffset * Time.deltaTime
		// );

		// if(this.followY
		// 	|| targetPos.y > this.targetY + this.verticalZoneWorld
		// 	|| targetPos.y < this.targetY - this.verticalZoneWorld * this.verticalZoneLowerMultiplier) {
			
		// 	MoveTo(targetPos);
		// }
	}

	public void StopDrama() {
		this.dramaStartTime = -100;
		this.background.gameObject.SetActive(true);

		this.cam.backgroundColor = this.defaultClearColor;
	}

	private void HandleDramaEffect() {
		if(!this.isDrama) {
			return;
		}

		float d = (Time.realtimeSinceStartup - this.dramaStartTime) / this.dramaDuration;
		this.cam.backgroundColor = this.dramaColors.Evaluate(d);
	}

	private void MoveTo(Vector2 pos) {
		this.transform.position = this.bounds.ClosestPoint(new Vector3(pos.x, pos.y, this.transform.position.z));
	
		Vector3 bgLocalPos = this.backgroundStartPosition;

		Vector2 d = new Vector2(this.transform.position.x, this.transform.position.y) - this.startPosition;
		bgLocalPos.x += d.x * this.parallaxMul.x;
		bgLocalPos.y += d.y * this.parallaxMul.y;

		this.background.position = bgLocalPos;
	}
}