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

    private SortedList<float, GameObject> sortedOverlapResults;

	protected float lastFireTime;
	private float lastAimTime;

	public void Awake() {
		this.overlapResults = new Collider2D[128];
        this.sortedOverlapResults = new SortedList<float, GameObject>(this.overlapResults.Length);

		this.lastFireTime = Time.time;
	
		Init();
	}

	public virtual void FixedUpdate() {
		if(Time.time - this.lastFireTime > this.fireRate) {
			Aim();
		}
	}

	public virtual void ShowInfo() {
		CircleDrawer.instance.Draw(this.transform.position, this.radius);
	}

	public virtual void HideInfo() {
		CircleDrawer.instance.DontDraw();
	}

	protected virtual void Init() {

	}

	protected abstract void Fire(Enemy target);

	protected void FindTarget() {
		int c = Physics2D.OverlapCircleNonAlloc(this.transform.position, this.radius, this.overlapResults, this.enemyLayers);
		
        this.sortedOverlapResults.Clear();

		for(int i = 0; i < c; i++) {
            if(this.sortedOverlapResults.ContainsKey(this.overlapResults[i].transform.position.x)) {
                continue;
            }

            this.sortedOverlapResults.Add(this.overlapResults[i].transform.position.x, this.overlapResults[i].gameObject);
		}

		Enemy selected = Filter(this.sortedOverlapResults);
		if(selected == null) {
			return;
		}

		Fire(selected);
		this.lastFireTime = Time.time;
	}

	protected virtual Enemy Filter(SortedList<float, GameObject> sortedOverlapResults) {
		if(sortedOverlapResults.Count == 0) {
			return null;
		}

		return sortedOverlapResults[0].GetComponent<Enemy>();
	}

	private void Aim() {
		if(Time.time - this.lastAimTime < AIM_TIME) {
			return; // don't aim every frame since it uses casting
		}
		this.lastAimTime = Time.time;

		FindTarget();
	}

#if UNITY_EDITOR
    
	public virtual void OnDrawGizmosSelected() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(this.transform.position, this.radius);
    }

#endif
}
