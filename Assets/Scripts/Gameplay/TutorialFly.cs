using UnityEngine;

public class TutorialFly : MonoBehaviour {
    public Transform[] waypoints;
    public float flightTime;
    public SpriteRenderer spriteRend;
    public ParticleSystem particles;

    private int waypointId = -1;
    private float flightStartTime;
    private Vector2 flightStartPos;

    private Transform destination {
        get {
            if(this.waypointId < 0 || this.waypointId >= this.waypoints.Length) {
                return null;
            }

            return this.waypoints[this.waypointId];
        }
    }

    public void OnTriggerStay2D(Collider2D collider) {
        if(collider.tag != "Actor" || collider.gameObject.layer != LayerMask.NameToLayer("Player")) {
            return;
        }

        if(Time.time - this.flightStartTime < this.flightTime) {
            return;
        }

        this.waypointId++;
        this.flightStartTime = Time.time;
        this.flightStartPos = this.transform.position;

        if(this.waypointId >= this.waypoints.Length) {
            this.enabled = false;
            this.spriteRend.enabled = false;
            this.particles.Stop();
        }
    }

    public void Update() {
        if(this.destination == null) {
            return;
        }

        float t = Mathf.SmoothStep(0, 1, (Time.time - this.flightStartTime) / this.flightTime);
        this.transform.position = Vector2.Lerp(this.flightStartPos, this.destination.position, t);
    }
}