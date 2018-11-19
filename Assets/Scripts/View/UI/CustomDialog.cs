using UnityEngine;

public abstract class CustomDialog : MonoBehaviour {
    [Header("Dialog")]
    public Transform[] buttons;
    public Transform selection;

    private const float MOVE_DELAY = 0.15f;

    private int selected;
    
    protected Transform activeButton {
        get {
            return this.buttons[this.selected];
        }
    }

    private float lastMoveTime;

    public void OnEnable() {
        this.selected = this.buttons.Length - 1;
        this.selection.position = this.activeButton.position;
        this.lastMoveTime = Time.time;
    }

    public void Update() {
        HandleMove();

        if(Input.GetButtonDown("Jump")) {
            if(this.selected == this.buttons.Length - 1) {
                Close();
            } else {
                Pressed(this.selected);
            }
        } else if(Input.GetButtonDown("Cancel") || Input.GetButtonDown("Build")) {
            Close();
        }
    }

    protected abstract void Pressed(int selection);

    protected abstract void Close();

    private void HandleMove() {
        if(Time.time - this.lastMoveTime < MOVE_DELAY) {
            return;
        }
    
        float x = Input.GetAxis("Horizontal");
        if(Mathf.Abs(x) < 0.1f) {
            return;
        }

        int dir = x > 0 ? 1 : -1;

        for(int i = 0; i < this.buttons.Length; i++) {
            this.selected += dir;
            if(this.selected >= this.buttons.Length) {
                this.selected = 0;
            } else if(this.selected < 0) {
                this.selected = this.buttons.Length - 1;
            }

            if(this.activeButton.gameObject.activeSelf) {
                break;
            }
        }
        
        this.selection.position = this.activeButton.position;
        this.lastMoveTime = Time.time;
    }
}