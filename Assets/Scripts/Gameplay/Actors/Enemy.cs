using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : Actor {
    [Header("Enemy stats")]
    public float health;

    protected override void Init() {
    }
}