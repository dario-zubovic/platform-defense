using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveyGrass : MonoBehaviour {
	[Range(0, 0.5f)]
	public float multiplier;
	public float frequency;

	private Material mat;

	private float targetSpeed;
	private float speed;
	private float lastTime;

	private float skew;

	public void Awake() {
		this.mat = this.gameObject.GetComponent<SpriteRenderer>().material;
	}

	public void Update() {
		this.speed = Mathf.Lerp(this.speed, this.targetSpeed, 0.075f);
		this.targetSpeed *= 0.99f;

		float targetSkew = this.speed * Mathf.Sin(this.frequency * (Time.time - this.lastTime));
		this.skew = Mathf.Lerp(this.skew, targetSkew, 0.075f);

		this.mat.SetFloat("_Skew", this.skew);
		this.transform.localPosition = new Vector3(this.skew * 0.5f, 0, 0);
	}

	public void OnTriggerEnter2D(Collider2D trigger) {
		if(trigger.gameObject.tag != "Actor") {
			return;
		}

		Actor actor = trigger.gameObject.GetComponent<Actor>();
		Vector2 vel = actor.GetVelocity() * this.multiplier;
		if(Mathf.Abs(vel.x) > Mathf.Abs(this.targetSpeed)) {
			this.targetSpeed = vel.x;
			this.lastTime = Time.time;
		}
	}
}
