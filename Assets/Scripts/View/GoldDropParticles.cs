using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDropParticles : MonoBehaviour {
	public float speedFrom, speedTo;
	public float angleFrom, angleTo;

	public static GoldDropParticles instance {
		get;
		private set;
	}

	private ParticleSystem particles;
	private ParticleSystem.EmitParams p;

	public void Awake() {
		GoldDropParticles.instance = this;
		
		this.particles = this.gameObject.GetComponent<ParticleSystem>();

		this.p = new ParticleSystem.EmitParams();
		this.p.startSize = 1;
		this.p.startLifetime = 4f;

	}

	public void Drop(Vector2 position) {
		float angle = Random.Range(this.angleFrom, this.angleTo) * Mathf.Deg2Rad;
		Vector2 vel = Random.Range(this.speedFrom, this.speedTo) * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		Vector2 pos = position + Vector2.up;

		int t = Random.Range(2, 4);

		for(int i = 0; i < t; i++) {
			this.p.position = pos + Random.insideUnitCircle * 0.2f;
			this.p.velocity = vel + Random.insideUnitCircle * 1.5f;
			this.particles.Emit(this.p, 1);
		}
	}
}
