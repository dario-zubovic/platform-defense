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
    
    protected override void AffectEnemy(Enemy enemy, float dist) {
        enemy.TakeDamage(
            Mathf.Lerp(this.maxDamage, this.minDamage, dist)
        );
    }
}