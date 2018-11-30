using UnityEngine;

public class JumpyEnemy : GroundEnemy {
    public float jumpWaitTimeMin, jumpWaitTimeMax;
    public float prepareForJumpTime;
    public float landTime;
    public float jumpSpeedMin, jumpSpeedMax;

    [Header("Visuals")]
    public Transform spriteTransform;
    public BouncyEnemyAnimator animator;

    private float nextJumpTime;
    private float groundedTime;
    private bool jumped;
    private bool prepareForJump;
    private bool landed;

    public void FullDeath() {
        Level.instance.AddGold(5);

        GoldDropParticles.instance.Drop(this.transform.position);
    
        Pool.instance.Return(this.gameObject);
    }

    protected override void Init() {
        base.Init();

        this.nextJumpTime = Time.time + Random.Range(this.jumpWaitTimeMin, this.jumpWaitTimeMax);
        this.groundedTime = 0;
        this.jumped = false;
        this.prepareForJump = false;
        this.landed = false;
    }
    
    protected override void Die() {
        base.Die();
    }

	protected override void MidMovementPhase() {
        base.MidMovementPhase();

        if(this.grounded) {
            if(this.jumped) {
                this.groundedTime = Time.time;
                this.jumped = false;
            }

            this.prepareForJump = false;
            this.landed = false;

            if(Time.time > this.nextJumpTime) {
                Jump();
                this.jumped = true;
            } else if(Time.time >= this.nextJumpTime - this.prepareForJumpTime) {
                if(this.groundNormal.y < 0.95f) {
                    this.nextJumpTime += this.jumpWaitTimeMin;
                } else {
                    this.prepareForJump = true;
                }
            } else if(Time.time - this.groundedTime <= this.landTime) {
                this.landed = true;
            }
        }
    }

    protected override void AfterMovementPhase() {
        base.AfterMovementPhase();

        this.animator.Refresh(this.velocity, this.grounded, this.jumped, this.prepareForJump, this.landed, this.dead);
    }

    private void Jump() {
        this.velocity.y = Random.Range(this.jumpSpeedMin, this.jumpSpeedMax);
        this.nextJumpTime = Time.time + Random.Range(this.jumpWaitTimeMin, this.jumpWaitTimeMax);
    }
}