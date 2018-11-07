using System.Collections.Generic;
using UnityEngine;

public class GranadeTurret : Turret {
    [Header("Projectile")]
    public float speed;
    public Projectile projectilePrefab; // TODO: pool
    public LayerMask wallLayer;
    
    private const int PROJECTILE_CHECK_SUBDIVISIONS = 16;

    private SortedList<float, GameObject> sortedOverlapResults;

    private float targetAngle;

    protected override void Init() {
        this.sortedOverlapResults = new SortedList<float, GameObject>(this.overlapResults.Length);
    }

    protected override void Fire(Enemy target) {
        Projectile projectile = GameObject.Instantiate<Projectile>(this.projectilePrefab, this.transform.position, Quaternion.identity);
        projectile.rigid.velocity = this.speed * new Vector2(Mathf.Cos(this.targetAngle), Mathf.Sin(this.targetAngle));
    }

	protected override void FindTarget() {
		int c = Physics2D.OverlapCircleNonAlloc(this.transform.position, this.radius, this.overlapResults, this.enemyLayers);
		
        this.sortedOverlapResults.Clear();

		for(int i = 0; i < c; i++) {
            this.sortedOverlapResults.Add(this.overlapResults[i].transform.position.x, this.overlapResults[i].gameObject);
		}

        GameObject targetGo = null;

        float reachAngle = 0;
        for(int i = 0; i < c; i++) {
            if(CanBeReached(this.sortedOverlapResults.Values[i].transform.position, out reachAngle)) {
                targetGo = this.sortedOverlapResults.Values[i];
                break;
            }
        }
        this.targetAngle = reachAngle;

		if(targetGo == null) {
			return;
		}

		Fire(targetGo.GetComponent<Enemy>());
		this.lastFireTime = Time.time;
        
    }

    private bool CanBeReached(Vector2 target, out float reachAngle) {
        Vector2 delta = target - new Vector2(this.transform.position.x, this.transform.position.y);
        bool flip = delta.x < 0;
        delta.x = Mathf.Abs(delta.x);

        float g = -1 * Physics2D.gravity.y;

        float underRoot = this.speed*this.speed*this.speed*this.speed - g * (g * delta.x * delta.x + 2 * delta.y * speed * speed);
        if(underRoot < 0) {
            reachAngle = 0;
            return false;
        }


        float angle1 = Mathf.Atan( ( this.speed*this.speed + Mathf.Sqrt(underRoot) ) / ( g * delta.x ) );
        float angle2 = Mathf.Atan( ( this.speed*this.speed - Mathf.Sqrt(underRoot) ) / ( g * delta.x ) );
        if(flip) {
            angle1 = Mathf.PI - angle1;
            angle2 = Mathf.PI - angle2;
        }
        float angle = Mathf.Min(angle1, angle2);

        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        // Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1)), Color.red, 0.1f);
        // Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2)), Color.green, 0.1f);
        // Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)), Color.blue, 0.1f);
        
        // Vector2 previousPos = this.transform.position;
        // Vector2 newPos;

        // TODO: raycast along path to check 

        // for(int i = 1; i <= PROJECTILE_CHECK_SUBDIVISIONS; i++) {
        //     float t = ((float)i) / ((float)PROJECTILE_CHECK_SUBDIVISIONS);
        
        //     newPos = this.speed * t * 
        // }

        reachAngle = angle;
        return true;
    }
}