using UnityEngine;

// wanna be state machine for player animations
public class PlayerAnimator : SpriteAnimator {
    public float runningHorizontalSpeedThreshold;

    public float fallSpeedThreshold;
    public float jumpMidPointSpeedThreshold;
    public float jumpDownSlowSpeedThreshold;

    private State state;
    
    private JumpPhase jumpPhase; 

    private string idleAnimName = "Idle1"; // DEBUG, remove this
    private string wallslideAnimName = "Wallslide2"; // DEBUG, remove this


    public void Refresh(Vector2 velocity, bool grounded, bool wallSliding, bool jumped, bool wallJumped) {
        // DEBUG, remove this:

        if(Input.GetKeyUp(KeyCode.Alpha1)) {
            this.idleAnimName = "Idle1";
        } else if(Input.GetKeyUp(KeyCode.Alpha2)) {
            this.idleAnimName = "Idle2";
        } else if(Input.GetKeyUp(KeyCode.Alpha3)) {
            this.idleAnimName = "Idle3";
        } else if(Input.GetKeyUp(KeyCode.Alpha4)) {
            this.idleAnimName = "Idle4";
        } else if(Input.GetKeyUp(KeyCode.Alpha5)) {
            this.idleAnimName = "Idle5";
        } else if(Input.GetKeyUp(KeyCode.Alpha6)) {
            this.idleAnimName = "Idle6";
        } else if(Input.GetKeyUp(KeyCode.Alpha7)) {
            this.idleAnimName = "Idle7";
        }

        if(Input.GetKeyUp(KeyCode.Alpha9)) {
            this.wallslideAnimName = "Wallslide1";
        } else if(Input.GetKeyUp(KeyCode.Alpha0)) {
            this.wallslideAnimName = "Wallslide2";
        }

        // change state if needed:

        bool wasWallJumping = false;

        switch(this.state) {
            case State.Idle:
                {
                    if(jumped || (!grounded && Mathf.Abs(velocity.y) > this.fallSpeedThreshold)) {
                        this.state = State.Jump;
                    } else {
                        if(Mathf.Abs(velocity.x) > this.runningHorizontalSpeedThreshold) {
                            this.state = State.Run;
                        }
                    }
                }
                break;

            case State.Jump:
                {
                    if(grounded) {
                        if(Mathf.Abs(velocity.x) > this.runningHorizontalSpeedThreshold) {
                            this.state = State.Run;
                        } else {
                            this.state = State.Idle;
                        }
                    } else if(wallSliding && velocity.y < 0) {
                        this.state = State.WallSlide;
                    }
                }
                break;

            case State.Run:
                {
                    if(jumped || (!grounded && Mathf.Abs(velocity.y) > this.fallSpeedThreshold)) {
                        this.state = State.Jump;
                    } else if (wallSliding && velocity.y < 0) {
                        this.state = State.WallSlide;
                    } else if(Mathf.Abs(velocity.x) < this.runningHorizontalSpeedThreshold) {
                        this.state = State.Idle;
                    }
                }
                break;

            case State.WallSlide:
                {
                    if(wallJumped) {
                        this.state = State.WallJump;
                    } else {
                        if(!wallSliding) {
                            this.state = State.Jump; // falling down
                        }
                    }
                }
                break;

            case State.WallJump:
                {
                    wasWallJumping = true;
                    this.state = State.Jump;
                }
                break;
        }


        // state logic:

        switch(this.state) {
            case State.Idle:
                {

                }
                break;

            case State.Jump:
                {
                    if(Mathf.Abs(velocity.y) < this.jumpMidPointSpeedThreshold) {
                        this.jumpPhase = JumpPhase.MidPoint;
                    } else if(velocity.y > 0) {
                        this.jumpPhase = JumpPhase.Up;
                    } else if(velocity.y > -this.jumpDownSlowSpeedThreshold) {
                        this.jumpPhase = JumpPhase.DownSlow;
                    } else {
                        this.jumpPhase = JumpPhase.DownFast;
                    }
                }
                break;

            case State.Run:
                {
                    
                }
                break;

            case State.WallSlide:
                {
                    
                }
                break;
        }


        // set active animation according to state:
        
        switch(this.state) {
            case State.Idle:
                SetActive(this.idleAnimName);
                break;
            case State.Run:
                SetActive("Run");
                break;
            case State.WallSlide:
                SetActive(this.wallslideAnimName);
                break;
            case State.WallJump:
                SetActive("Walljump");
                break;
            case State.Jump:
                switch(this.jumpPhase) {
                    case JumpPhase.Up:
                        SetActive("JumpUp", true, !wasWallJumping); // don't reset timer in case we're coming from wall jump
                        break;
                    case JumpPhase.MidPoint:
                        SetActive("JumpMidPoint");
                        break;
                    case JumpPhase.DownSlow:
                        SetActive("JumpDownSlow");
                        break;
                    case JumpPhase.DownFast:
                        SetActive("JumpDownFast");
                        break;
                }
                break;
        }
    }

    private enum State {
        Idle,
        Run,
        WallSlide,
        WallJump,
        Jump,
    }

    private enum JumpPhase {
        Up,
        MidPoint,
        DownSlow,
        DownFast,
    }
}