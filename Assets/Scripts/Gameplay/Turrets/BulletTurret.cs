using System.Collections.Generic;
using UnityEngine;

public class BulletTurret : Turret {
    [Header("Projectile")]
    public LayerMask wallLayer;
    public float damage;

    private RaycastHit2D[] raycastResults;

    protected override void Init() {
        this.raycastResults = new RaycastHit2D[32];
    }

    public override void Update() {
        base.Update();
    }

    public override void ShowInfo() {
        base.ShowInfo();

        this.turretInfo.SetStats(this.damage.ToString("0.0"), this.fireRate.ToString("0.0") + "s");
    }

    protected override void Fire(Enemy target) {
        target.TakeDamage(this.damage);
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
}