using UnityEngine;

public class PingPongMove : MonoBehaviour {
    public Vector3 delta;
    public float time;
    public bool smooth;

    private Vector3 start, end;

    public void Start() {
        this.start = this.transform.position;
        this.end = this.start + this.delta;
    }

    public void Update() {
        float t = Mathf.PingPong(Time.time / this.time, 1f);
        if(this.smooth) {
            t = Mathf.SmoothStep(0f, 1f, t);
        }

        this.transform.position = Vector3.Lerp(this.start, this.end, t);
    }
}