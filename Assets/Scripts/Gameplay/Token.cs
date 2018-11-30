using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

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
            if (this.isActive) {
                return false;
            }

            return Time.time - this.lastPickupTime > this.delay;
        }
    }

    private float lastPickupTime;

    private SpriteRenderer rend;
    private Level level;
    public AudioSource pickupAudioSource;
    public AudioSource ambientAudioSource;

    public void Awake() {
        this.rend = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        this.ambientAudioSource = this.gameObject.GetComponent<AudioSource>();
        this.level = GameObject.FindObjectOfType<Level>();

        SetupAudio();

        if (this.isDropToken) {
            return;
        }

        if (this.isStarting) {
            Activate();
        } else {
            Hide();
            this.lastPickupTime = -1000;
        }
    }
    
    public void SetupAudio() {
        Sound ambientSound = SoundManager.instance.GetSound(SoundId.TokenAmbient);
        this.ambientAudioSource.clip = ambientSound.clip;
        this.ambientAudioSource.volume = ambientSound.volume;
        this.ambientAudioSource.loop = true;
        this.ambientAudioSource.spatialBlend = 1;
        this.ambientAudioSource.rolloffMode = AudioRolloffMode.Linear;
        this.ambientAudioSource.maxDistance = 20;
        this.ambientAudioSource.dopplerLevel = 0.15f;
        this.ambientAudioSource.outputAudioMixerGroup = ambientSound.outputChannel;
    }

    public void OnEnable() {
        // play our ambient loop
        this.ambientAudioSource.Play();

        if (this.isDropToken) {
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
        if (!this.isActive || !this.gameObject.activeSelf || (this.isStarting && Time.time - this.lastPickupTime < 2f)) {
            return;
        }

        if (coll.gameObject.layer != LayerMask.NameToLayer("Player")) {
            return;
        }

        Player player = coll.gameObject.GetComponent<Player>();
        if (player.isDead) {
            return;
        }

        // Player picks up token
        SoundManager.instance.PlaySfx(SoundId.TokenPickup);
        this.ambientAudioSource.Stop();

        this.lastPickupTime = Time.time;
        Hide();
        this.level.Pickup(this.isDropToken == false);
    }

    private IEnumerator Move(Vector3 start, Vector3 destination, float duration) {
        float startTime = Time.time;
        while (Time.time < startTime + duration) {
            this.transform.position = Vector3.Lerp(start, destination, (Time.time - startTime) / duration);
            yield return null;
        }

        this.dropTokenPingPong.enabled = true;
    }

    private void Hide() {
        this.isActive = false;
        this.rend.enabled = false;

        if (this.isDropToken) {
            Pool.instance.Return(this.gameObject);
        }
    }
}