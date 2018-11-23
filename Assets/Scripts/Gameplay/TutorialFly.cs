using UnityEngine;

public class TutorialFly : MonoBehaviour {
    public Transform[] waypoints;
    public float flightTime;
    public SpriteRenderer spriteRend, spriteRend2;
    public ParticleSystem particles;

    private int waypointId = -1;
    private float flightStartTime = -100;
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
            this.spriteRend2.enabled = false;
            this.particles.Stop();
            return;
        }

        if(this.destination.position.x > this.transform.position.x) {
            this.spriteRend.transform.localScale = new Vector3(1, 1, 1);
        } else {
            this.spriteRend.transform.localScale = new Vector3(-1, 1, 1);
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