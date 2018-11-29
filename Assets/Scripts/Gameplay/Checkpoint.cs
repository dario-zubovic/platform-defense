using UnityEngine;

public class Checkpoint : MonoBehaviour {
    public int startLives = 1;

    [HideInInspector]
    public int lives = 1;

    public void OnEnable() {
        this.lives = this.startLives;
    }
}