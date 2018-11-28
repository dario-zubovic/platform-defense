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

    private List<EnemyEffect> effects;

    protected override void Init() {
        this.health = this.startHealth;
        
        if(this.effects == null) {
            this.effects = new List<EnemyEffect>(64);
        } else {
            this.effects.Clear();
        }

        this.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    public void Update() {
        this.healthIndicator.localScale = new Vector3(this.health / this.startHealth, 1, 1);
    }

    public void TakeDamage(float damage) {        
        this.health -= damage;

        if(this.health <= 0) {
            Die();
        }
    }
    
    public void AddEffect(EnemyEffect effect) {
        this.effects.Add(effect);
    }
    
	protected override void BeforeMovementPhase() {
        HandleEffects();
    }

    protected virtual void Die() {
        this.gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
    }

    private void HandleEffects() {
        // reset all modifiers:
        this.speedMultipler = 1f;

        // apply all effects and remove expired ones:
        int c = this.effects.Count;
        for(int i = 0; i < c; i++) {
            if(!this.effects[i].isActive) {
                this.effects.RemoveAt(i);
                i--;
                c--;
                continue;
            }

            if(this.effects[i].isLateEffect) {
                continue;
            }

            this.effects[i].Apply(this);
        }

        // normalize modifiers:
        this.speedMultipler = Mathf.Max(0f, this.speedMultipler);

        // apply late effect:
        for(int i = 0; i < this.effects.Count; i++) {
            if(this.effects[i].isLateEffect) {
                this.effects[i].Apply(this);
            }
        }
    }
}