using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
	public float horizontalZone;
	public float verticalZone;
	public float lookInFront;
	public float horizontalSpeed;
	public float verticalSpeed;

	public float targetY {
		get;
		set;
	}

	public Vector2 targetVelocity {
		get;
		set;
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

	private Camera cam;
	private PixelPerfectCamera pixelPerfectCamera;

	private Vector3 cameraExtents;
	
	private float horizontalOffset;
	private float offset;

	private float verticalZoneWorld;

	private Transform target;
	private bool followY;
	private bool focusRight, focusLeft;
	private Vector2 currentTargetVelocity;

	private Vector3 velocity;

	public void Awake() {
		this.cam = this.gameObject.GetComponent<Camera>();
		this.pixelPerfectCamera = this.gameObject.GetComponent<PixelPerfectCamera>();

		this.cameraExtents = new Vector2(this.pixelPerfectCamera.refResolutionX, this.pixelPerfectCamera.refResolutionY) / this.pixelPerfectCamera.assetsPPU * 0.5f;

		this.horizontalOffset = this.cam.ViewportToWorldPoint(new Vector3(0.5f + this.lookInFront, 0.5f)).x - this.cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)).x;
		this.verticalZoneWorld = this.cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f + this.verticalZone)).y - this.cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)).y;
	}

	public void SetTarget(Transform target, bool followY) {
		this.target = target;
		this.followY = followY;
		if(!followY) {
			this.targetY = target.transform.position.y;
		}

		this.currentTargetVelocity = Vector2.zero;
		this.targetVelocity = Vector2.zero;
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

		if(this.targetVelocity.x > 0.1f && Mathf.Sign(this.targetVelocity.x) != Mathf.Sign(this.currentTargetVelocity.x)) {
			this.currentTargetVelocity = Vector2.zero;
		}
		this.currentTargetVelocity = Vector2.Lerp(this.targetVelocity, this.currentTargetVelocity, Mathf.Exp(-3f*Time.deltaTime));

		Vector2 delta = Vector2.zero;

		Vector2 targetPos = this.target.position;
		Vector2 currentPos = this.transform.position;

		float lowerX = 0.5f;
		float upperX = 0.5f;

		if(this.focusRight) {
			upperX += this.horizontalZone;
			targetPos.x += this.offset * this.horizontalOffset + Mathf.Clamp(this.currentTargetVelocity.x, -8, 8f) * 0.5f;
		} else if(this.focusLeft) {
			lowerX -= this.horizontalZone;
			targetPos.x += this.offset * this.horizontalOffset + Mathf.Clamp(this.currentTargetVelocity.x, -8, 8f) * 0.5f;
		} else {
			lowerX -= this.horizontalZone / 2f;
			upperX += this.horizontalZone / 2f;
		}

		Vector2 targetViewPos = this.cam.WorldToViewportPoint(targetPos);

		Debug.DrawLine(new Vector3(targetPos.x, -100), new Vector3(targetPos.x, 100), Color.red);
		Debug.DrawLine(this.cam.ViewportToWorldPoint(new Vector3(lowerX, 0, 10)), this.cam.ViewportToWorldPoint(new Vector3(lowerX, 1, 10)), Color.green);
		Debug.DrawLine(this.cam.ViewportToWorldPoint(new Vector3(upperX, 0, 10)), this.cam.ViewportToWorldPoint(new Vector3(upperX, 1, 10)), Color.green);

		Debug.DrawLine(new Vector3(-100, this.followY ? this.targetY : targetPos.y), new Vector3(100, this.followY ? this.targetY : targetPos.y), Color.red);
		Debug.DrawLine(new Vector3(-100, this.targetY + this.verticalZoneWorld, 0), new Vector3(100, this.targetY + this.verticalZoneWorld, 0), Color.blue);
		Debug.DrawLine(new Vector3(-100, this.targetY - this.verticalZoneWorld, 0), new Vector3(100, this.targetY - this.verticalZoneWorld, 0), Color.blue);

		if(targetViewPos.x < lowerX || targetViewPos.x > upperX) {
			delta.x += (targetPos.x - currentPos.x) * this.horizontalSpeed;
		}

		if(this.followY) {
			delta.y += (targetPos.y - currentPos.y) * this.verticalSpeed;
		} else {
			if(targetPos.y > this.targetY + this.verticalZoneWorld) {
				this.targetY += (targetPos.y - this.targetY - this.verticalZoneWorld) * this.verticalSpeed;
			} else if(targetPos.y < this.targetY - this.verticalZoneWorld) {
				this.targetY += (targetPos.y - this.targetY + this.verticalZoneWorld) * this.verticalSpeed;
			}

			delta.y += (this.targetY - currentPos.y) * this.verticalSpeed;
		}

		Vector3 pos = this.transform.position + new Vector3(delta.x, delta.y) * Time.deltaTime;
		this.transform.position = this.bounds.ClosestPoint(pos);
	}
}