using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GroundEnemy : Actor {
    [Header("Enemy settings")]
    public bool moveRight;

    // [Header("Enemy stats")]


    protected override void Init() {
        this.input = this.moveRight ? Vector2.right : Vector2.left;
    }
}