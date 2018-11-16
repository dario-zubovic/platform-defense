using System.Collections.Generic;
using UnityEngine;

public class IceTurret : Turret {
    [Header("Projectile")]
    public float speed;
    public SlowdownProjectile projectilePrefab;
    public LayerMask wallLayer;
    public float blastRadius;
    public float minDuration, maxDuration;
    public float amount;

    private RaycastHit2D[] raycastResults;

    protected override void Init() {
        this.raycastResults = new RaycastHit2D[32];
    }

    public void Update() {
    }

    protected override void Fire(Enemy target) {
        Vector2 dir = (target.transform.position - this.transform.position).normalized;
        SlowdownProjectile projectile = GameObject.Instantiate<SlowdownProjectile>(this.projectilePrefab, this.transform.position, Quaternion.identity);
        projectile.rigid.gravityScale = 0f;
        projectile.rigid.velocity = this.speed * dir;
        projectile.blastRadius = this.blastRadius;
        projectile.minDuration = this.minDuration;
        projectile.maxDuration = this.maxDuration;
        projectile.amount = this.amount;
    }

	protected override Enemy Filter(SortedList<float, GameObject> sortedOverlapResults) {
        GameObject targetGo = null;

        for(int i = 0; i < sortedOverlapResults.Count; i++) {
            if(CanBeReached(sortedOverlapResults.Values[i].transform.position)) {
                targetGo = sortedOverlapResults.Values[i];
                break;
            }
        }

		if(targetGo == null) {
			return null;
		}

		return targetGo.GetComponent<Enemy>();
    }

    private bool CanBeReached(Vector2 target) {
        Vector2 dir = target - (Vector2)this.transform.position;
        
        int c = Physics2D.RaycastNonAlloc(this.transform.position, dir, this.raycastResults, dir.magnitude, this.wallLayer);

        for(int i = 0; i < c; i++) {
            if(this.raycastResults[i].collider != null && !this.raycastResults[i].collider.isTrigger) {
                return false;
            }
        }

        return true;
    }

#if UNITY_EDITOR
    
	public override void OnDrawGizmosSelected() {
		base.OnDrawGizmosSelected();

        Gizmos.color = new Color(0.9f, 0.5f, 0f, 1f);
		Gizmos.DrawWireSphere(this.transform.position + Vector3.right * this.radius, this.blastRadius);
    }

#endif
}