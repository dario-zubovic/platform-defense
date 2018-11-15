using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : Actor {
    [Header("Enemy stats")]
    public float startHealth;

    [Header("Visuals")]
    public Transform healthIndicator;

    private float health;

    protected override void Init() {
        this.health = this.startHealth;
    }

    public void Update() {
        this.healthIndicator.localScale = new Vector3(this.health / this.startHealth, 1, 1);
    }

    public void TakeDamage(float damage) {        
        this.health -= damage;

        if(this.health < 0) {
            GameObject.Destroy(this.gameObject);
        }
    }
}