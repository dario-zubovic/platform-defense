using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Turret : MonoBehaviour {
	public float radius;
	public float fireRate;

	[Header("Generic settings")]
	public LayerMask enemyLayers;

	private const float AIM_TIME = 0.1f;

	protected Collider2D[] overlapResults;

	protected float lastFireTime;
	private float lastAimTime;

	public void Awake() {
		this.overlapResults = new Collider2D[128];

		this.lastFireTime = Time.time;
	
		Init();
	}

	public void FixedUpdate() {
		if(Time.time - this.lastFireTime > this.fireRate) {
			Aim();
		}
	}

	protected virtual void Init() {

	}

	protected abstract void Fire(Enemy target);

	protected virtual void FindTarget() {
		int c = Physics2D.OverlapCircleNonAlloc(this.transform.position, this.radius, this.overlapResults, this.enemyLayers);
		
		GameObject leftMostGo = null;
		float leftMost = float.MaxValue;

		for(int i = 0; i < c; i++) {
			if(this.overlapResults[i].transform.position.x < leftMost) {
				leftMost = this.overlapResults[i].transform.position.x;
				leftMostGo = this.overlapResults[i].gameObject;
			}
		}

		if(leftMostGo = null) {
			return;
		}

		Fire(leftMostGo.GetComponent<Enemy>());
		this.lastFireTime = Time.time;
	}

	private void Aim() {
		if(Time.time - this.lastAimTime < AIM_TIME) {
			return; // don't aim every frame since it uses casting
		}
		this.lastAimTime = Time.time;

		FindTarget();
	}

#if UNITY_EDITOR
    
	public void OnDrawGizmosSelected() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(this.transform.position, radius);
    }

#endif
}
