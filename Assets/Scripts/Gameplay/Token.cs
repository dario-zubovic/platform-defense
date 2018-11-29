using System.Collections;
using UnityEngine;

public class Token : MonoBehaviour {
    public float weight;
    public float delay;
    public bool isStarting;

    public bool isDropToken;
    public PingPongMove dropTokenPingPong;

    public bool isActive {
        get;
        private set;
    }

    public bool canBeActivated {
        get {
            if(this.isActive) {
                return false;
            }

            return Time.time - this.lastPickupTime > this.delay;
        }
    }

    private float lastPickupTime;

    private SpriteRenderer rend;
    private Level level;

    public void Awake() {
        this.rend = this.gameObject.GetComponent<SpriteRenderer>();
        this.level = GameObject.FindObjectOfType<Level>();

        if(this.isDropToken) {
            return;
        }

        if(this.isStarting) {
            Activate();
        } else {
            Hide();
            this.lastPickupTime = -1000;
        }
    }

    public void OnEnable() {
        if(this.isDropToken) {
            this.lastPickupTime = Time.time;
            Activate();
        }
    }

    public void Activate() {
        this.isActive = true;
        this.rend.enabled = true;
    }

    public void MoveTo(Vector3 pos) {
        StartCoroutine(Move(this.transform.position, pos, 0.05f));
    }

    public void OnTriggerEnter2D(Collider2D coll) {
        if(!this.isActive || !this.gameObject.activeSelf || (this.isStarting && Time.time - this.lastPickupTime < 2f)) {
            return;
        }

        if(coll.gameObject.layer != LayerMask.NameToLayer("Player")) {
            return;
        }

        Player player = coll.gameObject.GetComponent<Player>();
        if(player.isDead) {
            return;
        }

        this.lastPickupTime = Time.time;
        Hide();
        this.level.Pickup(this.isDropToken == false);
    }

    private IEnumerator Move(Vector3 start, Vector3 destination, float duration) {
        float startTime = Time.time;
        while(Time.time < startTime + duration) {
            this.transform.position = Vector3.Lerp(start, destination, (Time.time - startTime) / duration);
            yield return null;
        }

        this.dropTokenPingPong.enabled = true;
    }

    private void Hide() {
        this.isActive = false;
        this.rend.enabled = false;
    
        if(this.isDropToken) {
            Pool.instance.Return(this.gameObject);
        }
    }
}