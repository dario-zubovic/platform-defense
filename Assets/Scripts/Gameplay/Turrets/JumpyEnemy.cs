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
    private bool dead;

    protected override void Init() {
        base.Init();

        this.groundNormal = Vector2.up;

        this.nextJumpTime = Time.time + Random.Range(this.jumpWaitTimeMin, this.jumpWaitTimeMax);
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
                this.prepareForJump = true;
            } else if(Time.time - this.groundedTime <= this.landTime) {
                this.landed = true;
            }
        }
        
        this.spriteTransform.localEulerAngles = new Vector3(0, 0, 90f - Mathf.Atan2(this.groundNormal.y, this.groundNormal.x) * Mathf.Rad2Deg);
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