using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class SpriteAnimator : MonoBehaviour {
    public List<SpriteAnimation> animations;
    
    private SpriteRenderer rend;

    private SpriteAnimation activeAnimation;
    private int frame;
    private float lastFrameChangeTime;

    public void Awake() {
        this.rend = this.gameObject.GetComponent<SpriteRenderer>();

        this.activeAnimation = this.animations[0];
    }

    public void Update() {
        if(this.activeAnimation == null) {
            return;
        }

        if(Time.time - this.lastFrameChangeTime > this.activeAnimation.framerate) {
            this.frame = (this.frame + 1) % this.activeAnimation.frames.Count;
            this.lastFrameChangeTime = Time.time;

            this.rend.sprite = this.activeAnimation.frames[this.frame];
        }
    }

    protected void SetActive(string name, bool resetFrames = true, bool resetTime = true) {
        if(this.activeAnimation != null && this.activeAnimation.name == name) {
            return;
        }

        this.activeAnimation = this.animations.Find(o => o.name == name);

        if(resetFrames) {
            this.frame = 0;
        }

        if(resetTime) {
            this.lastFrameChangeTime = -100;
        }
    }
}