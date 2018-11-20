using TMPro;
using UnityEngine;

public abstract class CustomDialog : MonoBehaviour {
    [Header("Dialog")]
    public Transform[] buttons;
    public Transform selection;
    public TextMeshPro[] shortcutLabels;

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

        int j = 1;
        for(int i = 0; i < this.buttons.Length; i++) {
            if(!this.buttons[i].gameObject.activeSelf) {
                continue;
            }

            if(i == this.buttons.Length - 1) {
                this.shortcutLabels[i].text = "0";    
            } else {
                this.shortcutLabels[i].text = j.ToString();
                j++;
            }
        }
    }

    public void Update() {
        HandleMove();
        HandleShortcuts();

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

    protected abstract void SelectionChanged(int selection);

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
        
        Select(this.selected);
    }

    private void HandleShortcuts() {
        if(Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) {
            Close();
            return;
        }
        
        int s = -1;

        if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) {
            s = 0;
        } else if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) {
            s = 1;
        } else if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) {
            s = 2;
        } else if(Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) {
            s = 3;
        } else if(Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) {
            s = 4;
        } else if(Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) {
            s = 5;
        } else if(Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) {
            s = 6;
        } else if(Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) {
            s = 7;
        } else if(Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) {
            s = 8;
        }

        if(s == -1) {
            return;
        }

        int c = -1;
        for(int i = 0; i < this.buttons.Length - 1; i++) {
            if(!this.buttons[i].gameObject.activeSelf) {
                continue;
            }

            c++;
            if(c == s) {
                Select(i);
                Pressed(this.selected);
                return;
            }
        }
    }

    private void Select(int id) {
        this.selected = id;
        this.selection.position = this.activeButton.position;
        this.lastMoveTime = Time.time;

        SelectionChanged(id);
    }
}