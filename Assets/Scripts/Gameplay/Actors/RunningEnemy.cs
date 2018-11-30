using System.Collections;
using UnityEngine;

public class RunningEnemy : GroundEnemy {

    [Header("Visuals")]
    public float waitAfterDeath;
    public RunningEnemyAnimator animator;

    private bool didFullDie;

    public void FullDeath() {
        if(this.didFullDie) {
            return;
        }

        Level.instance.AddGold(5);

        GoldDropParticles.instance.Drop(this.transform.position);

        this.didFullDie = true;

        StartCoroutine(DisposeOfBody());
    }
    
    protected override void Die() {
        base.Die();

        this.didFullDie = false;
    }

    protected override void AfterMovementPhase() {
        base.AfterMovementPhase();

        this.animator.Refresh(this.velocity, this.grounded, this.dead);
    }

    private IEnumerator DisposeOfBody() {
        yield return new WaitForSeconds(this.waitAfterDeath);

        Pool.instance.Return(this.gameObject);
    }
}