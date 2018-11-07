using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : Platform {
	public Vector3 localDestination;
	public float travelTime;

	public override PlatformType type {
		get {
			return PlatformType.Moving;
		}
	}

	public Vector2 velocity {
		get {
			return this.speed * (this.ping ? 1f : -1f);
		}
	}

	private Vector3 start, end;
	private bool ping;
	private float startTime;

	private Vector2 speed;

	public void Start() {
		this.start = this.transform.position;
		this.end = this.transform.position + this.localDestination;
	
		this.speed = new Vector2(this.localDestination.x, this.localDestination.y) / this.travelTime;

		this.startTime = Time.time;
		this.ping = true;
	}

	public void FixedUpdate() {
		float t = (Time.time - this.startTime) / this.travelTime;
		if(t >= 1) {
			t = 0;
			this.ping = !this.ping;
			this.startTime = Time.time;
		}

		this.transform.position = Vector3.Lerp(
			this.ping ? this.start : this.end,
			this.ping ? this.end : this.start,
			t
		);
	}
}
