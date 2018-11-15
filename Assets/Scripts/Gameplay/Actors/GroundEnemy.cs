using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GroundEnemy : Enemy {
    [Header("Enemy settings")]
    public bool moveRight;

    protected override void Init() {
        base.Init();
        
        this.input = this.moveRight ? Vector2.right : Vector2.left;
    }
}