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
		this.p.velocity = Random.Range(this.speedFrom, this.speedTo) * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		this.p.position = position + Vector2.up;
		this.particles.Emit(p, 1);
	}
}
