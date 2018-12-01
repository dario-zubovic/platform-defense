using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GroundEnemy : Enemy {
    [Header("Enemy settings")]
    public bool moveRight;

    protected bool dead;

    protected override void Init() {
        base.Init();

        this.dead = false;
    }

    public override void Spawn(Vector3 location) {
        base.Spawn(location);

        this.dead = false;
    }

    protected override void Die() {
        base.Die();
        
        this.dead = true;
    }
    
	protected override void BeforeMovementPhase() {
        base.BeforeMovementPhase();

        if(this.dead) {
            this.input = Vector2.zero;
        } else {
            this.input = this.moveRight ? Vector2.right : Vector2.left;
        }
    }
}