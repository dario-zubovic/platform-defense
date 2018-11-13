using UnityEngine;

public class GenericAnimator : SpriteAnimator {
    public void Start() {
        if(this.animations.Count > 0) {
            SetActive(this.animations[0].name);
        }
    }
}