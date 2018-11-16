using UnityEngine;

public class SlowdownProjectile : Projectile {
    public float minDuration {
        get;
        set;
    }
    public float maxDuration {
        get;
        set;
    }
    public float amount {
        get;
        set;
    }
    public bool late {
        get;
        set;
    }

    protected override void AffectEnemy(Enemy enemy, float dist) {
        enemy.AddEffect(new SlowdownEffect(Mathf.Lerp(this.maxDuration, this.minDuration, dist), this.amount, late));
    }
}