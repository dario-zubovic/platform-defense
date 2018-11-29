using UnityEngine;

public class PingPongMove : MonoBehaviour {
    public Vector3 delta;
    public float time;
    public bool smooth;
    public bool local;

    private Vector3 start, end;

    public void OnEnable() {
        if(this.local) {
            this.start = this.transform.localPosition;
        } else {
            this.start = this.transform.position;
        }
        
        this.end = this.start + this.delta;
    }

    public void Update() {
        float t = Mathf.PingPong(Time.time / this.time, 1f);
        if(this.smooth) {
            t = Mathf.SmoothStep(0f, 1f, t);
        }

        if(this.local) {
            this.transform.localPosition = Vector3.Lerp(this.start, this.end, t);
        } else {
            this.transform.position = Vector3.Lerp(this.start, this.end, t);
        }
    }
}