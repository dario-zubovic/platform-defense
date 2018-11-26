using UnityEngine;

public class JumpyEnemy : GroundEnemy {
    public float jumpWaitTimeMin, jumpWaitTimeMax;
    public float jumpSpeed;

    [Header("Visuals")]
    public Transform spriteTransform;

    private float nextJumpTime;

    protected override void Init() {
        base.Init();

        this.groundNormal = Vector2.up;

        this.nextJumpTime = Time.time + Random.Range(this.jumpWaitTimeMin, this.jumpWaitTimeMax);
    }

    
	protected override void MidMovementPhase() {
        base.MidMovementPhase();

        if(this.grounded) {

            if(Time.time > this.nextJumpTime) {
                Jump();
            }
        }
        
            this.spriteTransform.localEulerAngles = new Vector3(0, 0, 90f - Mathf.Atan2(this.groundNormal.y, this.groundNormal.x) * Mathf.Rad2Deg);
    }

    private void Jump() {
        this.velocity.y = this.jumpSpeed;
        this.nextJumpTime = Time.time + Random.Range(this.jumpWaitTimeMin, this.jumpWaitTimeMax);
    }
}