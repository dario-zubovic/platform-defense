using UnityEngine;

public class RunningEnemyAnimator : SpriteAnimator {
    public float fallSpeedThreshold;

    private State state;

    private RunningEnemy enemy;

    public void Start() {
        this.enemy = this.gameObject.GetComponentInParent<RunningEnemy>();
    }

    public void OnEnable() {
        this.state = State.Ground;
    }

    public void Refresh(Vector2 velocity, bool grounded, bool dead) {
        // change state:

        switch(this.state) {
            case State.Ground:
                {
                    if(!grounded && Mathf.Abs(velocity.y) > this.fallSpeedThreshold) {
                        this.state = State.Fall;
                    } else if(dead) {
                        SoundManager.instance.PlayAtPosition(SoundId.RunningEnemyDeath, this.transform.position);
                        this.state = State.Dead;
                    }
                }
                break;

            case State.Fall:
                {
                    if(grounded) {
                        this.state = State.Ground;
                    }
                }
                break;
        }

        // state logic:

        switch(this.state) {

            case State.Dead:
                {
                    if(this.finishedSingleShot) {
                        this.enemy.FullDeath();
                    }
                }
                break;
        }

        // set animation according to state:
        switch(this.state) {
            case State.Ground:
                {
                    SetActive("Ground");
                }
                break;

            case State.Fall:
                {
                    SetActive("Fall");
                }
                break;

            case State.Dead:
                {
                    SetActive("Death");
                }
                break;
        }
    }

    private enum State {
        Ground,
        Fall,
        Dead,
    }
}