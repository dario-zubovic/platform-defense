using UnityEngine;

public class PingPongColor : MonoBehaviour {
    public Color target;
    public float time;
    public bool smooth;

    private SpriteRenderer rend;
    private Color start;

    public void Start() {
        this.rend = this.gameObject.GetComponent<SpriteRenderer>();
        this.start = this.rend.color;
    }

    public void Update() {
        float t = Mathf.PingPong(Time.time / this.time, 1f);
        if(this.smooth) {
            t = Mathf.SmoothStep(0f, 1f, t);
        }

        this.rend.color = Color.Lerp(this.start, this.target, t);
    }
}