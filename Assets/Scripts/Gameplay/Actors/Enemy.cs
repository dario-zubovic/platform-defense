using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : Actor {
    [Header("Enemy stats")]
    public float health;

    protected override void Init() {
    }

    public void TakeDamage(float damage) {        
        this.health -= damage;

        if(this.health < 0) {
            Debug.Log("dead");
            GameObject.Destroy(this.gameObject);
        }
    }
}