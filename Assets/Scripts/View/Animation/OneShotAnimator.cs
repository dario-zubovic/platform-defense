using UnityEngine;

public class OneShotAnimator : SpriteAnimator {
    public void OnEnable() {
        if(this.animations.Count > 0) {
            SetActive(this.animations[0].name);
            SetFrame(0);
            this.finishedSingleShot = false;
        }
    }

    public void Update() {
        if(this.finishedSingleShot) {
            Pool.instance.Return(this.gameObject);
        }
    }
}