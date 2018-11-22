using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingVine : MonoBehaviour {
	public Transform point1, point2, point3;
	[Range(0, 0.5f)]
	public float multiplier;
	public float frequency;
	public float offset;

	private SpriteRenderer rend;

	private Vector2 targetVelocity;
	private Vector2 velocity;
	private float lastTime;

	private Vector2 skew;

	public void Start() {
		this.rend = this.gameObject.GetComponent<SpriteRenderer>();
	}

	public void Update() {
		this.velocity = Vector2.Lerp(this.velocity, this.targetVelocity, 0.075f);
		this.targetVelocity *= 0.985f;

		Vector2 targetSkew = this.velocity * Mathf.Sin(this.frequency * (Time.time - this.lastTime));
		this.skew = Vector2.Lerp(this.skew, targetSkew, 0.075f);

		this.rend.material.SetVector("_P0", this.point1.position);
		this.rend.material.SetVector("_P1", this.point2.position + (Vector3)this.skew);
		this.rend.material.SetVector("_P2", this.point3.position);
		this.rend.material.SetFloat("_Offset", this.offset);
	}

	public void OnTriggerEnter2D(Collider2D trigger) {
		if(trigger.gameObject.tag != "Actor") {
			return;
		}

		Actor actor = trigger.gameObject.GetComponent<Actor>();
		Vector2 vel = actor.GetVelocity() * this.multiplier;
		if(vel.sqrMagnitude > this.targetVelocity.sqrMagnitude) {
			this.targetVelocity = vel;
			this.lastTime = Time.time;
		}
	}
}
