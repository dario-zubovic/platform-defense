using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	private Camera cam;
	
	private float horizontalOffset;
	private float offset;

	private float verticalZoneWorld;

	private Transform target;
	private bool followY;
	private bool focusRight, focusLeft;

	private Vector3 velocity;
	
	private Vector2 lastTargetPosition;

	public void Awake() {
		this.cam = this.gameObject.GetComponent<Camera>();

		this.horizontalOffset = this.cam.ViewportToWorldPoint(new Vector3(0.5f + this.lookInFront, 0.5f)).x - this.cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)).x;
		this.verticalZoneWorld = this.cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f + this.verticalZone)).y - this.cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)).y;
	}

	public void SetTarget(Transform target, bool followY) {
		this.target = target;
		this.followY = followY;
		this.lastTargetPosition = target.transform.position;
		if(!followY) {
			this.targetY = target.transform.position.y;
		}
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

		Vector2 delta = Vector2.zero;

		Vector2 targetPos = this.target.position;
		Vector2 currentPos = this.transform.position;

		float lowerX = 0.5f;
		float upperX = 0.5f;

		if(this.focusRight) {
			upperX += this.horizontalZone;
			targetPos.x += offset * this.horizontalOffset;
		} else if(this.focusLeft) {
			lowerX -= this.horizontalZone;
			targetPos.x += offset * this.horizontalOffset;
		} else {
			lowerX -= this.horizontalZone / 2f;
			upperX += this.horizontalZone / 2f;
		}

		Vector2 targetVelocity = (targetPos - this.lastTargetPosition) / Time.deltaTime;
		this.lastTargetPosition = targetPos;

		Vector2 targetViewPos = this.cam.WorldToViewportPoint(targetPos);	

		// Debug.DrawLine(new Vector3(targetPos.x, -100), new Vector3(targetPos.x, 100), Color.red);
		// Debug.DrawLine(this.cam.ViewportToWorldPoint(new Vector3(lowerX, 0, 10)), this.cam.ViewportToWorldPoint(new Vector3(lowerX, 1, 10)), Color.green);
		// Debug.DrawLine(this.cam.ViewportToWorldPoint(new Vector3(upperX, 0, 10)), this.cam.ViewportToWorldPoint(new Vector3(upperX, 1, 10)), Color.green);

		// Debug.DrawLine(new Vector3(-100, this.followY ? this.targetY : targetPos.y), new Vector3(100, this.followY ? this.targetY : targetPos.y), Color.red);
		// Debug.DrawLine(new Vector3(-100, this.targetY + this.verticalZoneWorld, 0), new Vector3(100, this.targetY + this.verticalZoneWorld, 0), Color.blue);
		// Debug.DrawLine(new Vector3(-100, this.targetY - this.verticalZoneWorld, 0), new Vector3(100, this.targetY - this.verticalZoneWorld, 0), Color.blue);

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
	
		this.transform.position = this.transform.position + new Vector3(delta.x, delta.y) * Time.deltaTime;
	}
}