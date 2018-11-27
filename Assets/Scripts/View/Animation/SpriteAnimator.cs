using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class SpriteAnimator : MonoBehaviour {
    public List<SpriteAnimation> animations;

    public int frame {
        get;
        private set;
    }
    
    public string animationName {
        get {
            if(this.activeAnimation == null) {
                return null;
            }

            return this.activeAnimation.name;
        }
    }

    public bool finishedSingleShot {
        get;
        private set;
    }

    private SpriteRenderer rend;

    private SpriteAnimation activeAnimation;
    private string activeAnimationName;
    private float lastFrameChangeTime;

    private Dictionary<string, SpriteAnimation> animationsDict;

    public void Awake() {
        this.rend = this.gameObject.GetComponent<SpriteRenderer>();

        this.animationsDict = new Dictionary<string, SpriteAnimation>(this.animations.Count);
        foreach(var animation in this.animations) {
            this.animationsDict.Add(animation.name, animation);
        }
    }

    public void LateUpdate() {
        if(this.activeAnimation == null) {
            return;
        }

        if(Time.time - this.lastFrameChangeTime > this.activeAnimation.framerate) {
            this.lastFrameChangeTime = Time.time;

            this.finishedSingleShot = false;

            this.frame++;
            if(this.frame >= this.activeAnimation.frames.Count) {
                if(this.activeAnimation.loop) {
                    this.frame = 0;
                } else {
                    this.frame--;
                    this.finishedSingleShot = true;
                    return;
                }
            }

            this.rend.sprite = this.activeAnimation.frames[this.frame];
        }
    }

    public void SetFrame(int frame) {
        this.frame = frame;
    }

    protected void SetActive(string name, bool resetFrames = true, bool resetTime = true) {
        if(this.activeAnimation != null && this.activeAnimationName == name) {
            return;
        }

        this.activeAnimation = this.animationsDict[name];
        this.activeAnimationName = name;

        if(resetFrames) {
            this.frame = 0;
        }

        if(resetTime) {
            if(resetFrames) {
                this.frame = -1;
            } else {
                this.frame--;
            }
            this.lastFrameChangeTime = -100;
        }
    }
}