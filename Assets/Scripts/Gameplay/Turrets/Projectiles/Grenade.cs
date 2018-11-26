using UnityEngine;

public class Grenade : Projectile {
    public float minDamage {
        get;
        set;
    }
    public float maxDamage {
        get;
        set;
    }
    
    public void Update() {
        Vector2 vel = this.rigid.velocity;
        float angle = Mathf.Atan2(vel.y, vel.x);
        this.transform.localEulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
    }

    protected override void AffectEnemy(Enemy enemy, float dist) {
        enemy.TakeDamage(
            Mathf.Lerp(this.maxDamage, this.minDamage, dist)
        );
    }
}