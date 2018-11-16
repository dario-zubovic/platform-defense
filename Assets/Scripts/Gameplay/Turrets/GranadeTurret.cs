using System.Collections.Generic;
using UnityEngine;

public class GranadeTurret : Turret {
    [Header("Target lock")]
    public float lockingTime = 0.3f;
    public float fireTimeAfterLockFailure;


    [Header("Projectile")]
    public float speed;
    public Grenade projectilePrefab; // TODO: pool
    public LayerMask wallLayer;
    public float blastRadius;
    public float minDamage, maxDamage;

    [Header("Visuals")]
    public Transform aim;
    public List<SpriteRenderer> aimRends;
    
    private const int PROJECTILE_CHECK_SUBDIVISIONS = 16;

    private RaycastHit2D[] raycastResults;
    
    private Enemy lockedEnemy;
    private float lockedTime;

    protected override void Init() {
        this.raycastResults = new RaycastHit2D[16];

        // this.radius = this.speed * this.speed / Mathf.Abs(Physics2D.gravity.y) * 2;
    }

    public void Update() {
        if(this.lockedEnemy == null) {
            foreach(var rend in this.aimRends) {
                Color c = rend.color;
                if(Time.time - this.lockedTime - this.lockingTime < 0.8f) {
                    c.a = 1;
                } else {
                    c.a = 0;
                }
                rend.color = c;
            }
            return;
        }

        float t = (Time.time - this.lockedTime) / this.lockingTime;

        foreach(var rend in this.aimRends) {
            Color c = rend.color;

            if(t > 0.6f) {
                c.a = 1f;
            } else {
                c.a = Mathf.RoundToInt(t * 5) % 2 == 0 ? 0.6f : 0f;
            }


            rend.color = c;
        }
    }

    public override void FixedUpdate() {
        base.FixedUpdate();

        if(this.lockedEnemy != null) {
            this.aim.position = this.lockedEnemy.transform.position;
        
            if(Time.time - this.lockedTime > this.lockingTime) {
                float targetAngle = 0;
                CanBeReached(this.lockedEnemy.transform.position, out targetAngle, true);
                    FireOnLocked(targetAngle);
                // } else {
                //     // failed to shoot on locked target...
                //     // TODO: lock failed animation

                //     this.lastFireTime = Time.time - this.lastFireTime + this.fireTimeAfterLockFailure;
                // }
                
                this.lockedEnemy = null;
            }
        }
    }

    protected override void Fire(Enemy target) {
        this.lockedEnemy = target;
        this.lockedTime = Time.time;
    }

    private void FireOnLocked(float targetAngle) {
        Grenade projectile = GameObject.Instantiate<Grenade>(this.projectilePrefab, this.transform.position, Quaternion.identity);
        projectile.rigid.velocity = this.speed * new Vector2(Mathf.Cos(targetAngle), Mathf.Sin(targetAngle));
        projectile.blastRadius = this.blastRadius;
        projectile.minDamage = this.minDamage;
        projectile.maxDamage = this.maxDamage;
    }

	protected override Enemy Filter(SortedList<float, GameObject> sortedOverlapResults) {
        GameObject targetGo = null;

        float reachAngle;
        for(int i = 0; i < sortedOverlapResults.Count; i++) {
            if(CanBeReached(sortedOverlapResults.Values[i].transform.position, out reachAngle)) {
                targetGo = sortedOverlapResults.Values[i];
                break;
            }
        }
        // this.targetAngle = reachAngle;

		if(targetGo == null) {
			return null;
		}

		return targetGo.GetComponent<Enemy>();
    }

    private bool CanBeReached(Vector2 target, out float reachAngle, bool force = false) {
        Vector2 delta = target - new Vector2(this.transform.position.x, this.transform.position.y);
        bool flip = delta.x < 0;
        delta.x = Mathf.Abs(delta.x);

        float g = -1 * Physics2D.gravity.y;

        float underRoot = this.speed*this.speed*this.speed*this.speed - g * (g * delta.x * delta.x + 2 * delta.y * speed * speed);
        if(underRoot < 0) {
            if(force) {
                underRoot = 0;
            } else {
                reachAngle = 0;
                return false;
            }
        }


        float angle1 = Mathf.Atan( ( this.speed*this.speed + Mathf.Sqrt(underRoot) ) / ( g * delta.x ) );
        float angle2 = Mathf.Atan( ( this.speed*this.speed - Mathf.Sqrt(underRoot) ) / ( g * delta.x ) );
        if(flip) {
            angle1 = Mathf.PI - angle1;
            angle2 = Mathf.PI - angle2;
        }
        float angle = Mathf.Min(angle1, angle2);
        reachAngle = angle;

        if(force) {
            return true;
        }

        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        // Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1)), Color.red, 0.1f);
        // Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2)), Color.green, 0.1f);
        // Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)), Color.blue, 0.1f);
        
        float timeOfFlight = ( this.speed * sin + Mathf.Sqrt( this.speed*this.speed*sin*sin - 2*g*delta.y ) ) / g;

        Vector2 previousPos = this.transform.position;
        Vector2 newPos; 

        for(int i = 1; i <= PROJECTILE_CHECK_SUBDIVISIONS; i++) {
            float t = ((float)i) / ((float)PROJECTILE_CHECK_SUBDIVISIONS) * timeOfFlight;
        
            newPos = new Vector2(
                this.speed * t * cos,
                this.speed * t * sin - 0.5f * g * t * t
            );
            newPos.x += this.transform.position.x;
            newPos.y += this.transform.position.y;

            // Debug.DrawLine(previousPos, newPos, Color.red, 0.1f);
            
            int c = Physics2D.RaycastNonAlloc(previousPos, newPos - previousPos, this.raycastResults, (newPos - previousPos).magnitude, this.wallLayer);
            for(int j = 0; j < c; j++) {
                if(this.raycastResults[j].collider != null && !this.raycastResults[j].collider.isTrigger) {
                    return false;
                }
            }

            previousPos = newPos;
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