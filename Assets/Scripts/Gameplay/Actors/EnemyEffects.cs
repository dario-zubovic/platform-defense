using UnityEngine;

public abstract class EnemyEffect {

    public bool isActive {
        get {
            return Time.time - this.startTime < this.duration;
        }
    }

    public bool isLateEffect {
        get;
        protected set;
    }
    
    private float duration;
    private float startTime;

    public EnemyEffect(float duration) {
        this.duration = duration;
        this.startTime = Time.time;
    }

    public abstract void Apply(Enemy enemy);
}

public class SlowdownEffect : EnemyEffect {
    private float amount;

    public SlowdownEffect(float duration, float amount, bool late) : base(duration) {
        this.amount = amount;
        this.isLateEffect = late;
    }

    public override void Apply(Enemy enemy) {
        enemy.speedMultipler -= this.amount;
    }
}