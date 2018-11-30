using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Token : MonoBehaviour
{
    public float weight;
    public float delay;
    public bool isStarting;

    public bool isDropToken;
    public PingPongMove dropTokenPingPong;

    public bool isActive
    {
        get;
        private set;
    }

    public bool canBeActivated
    {
        get
        {
            if (this.isActive)
            {
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
    public void SetupAudio()
    {
        this.pickupAudioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        this.ambientAudioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

        // enable pickup sound
        Sound pickupSound = SoundManager.instance.GetSound(SoundId.TokenPickup);
        this.pickupAudioSource.clip = pickupSound.clip;
        this.pickupAudioSource.volume = pickupSound.volume;

        // enable ambient sound loop
        Sound ambientSound = SoundManager.instance.GetSound(SoundId.TokenAmbient);
        this.ambientAudioSource.clip = ambientSound.clip;
        this.ambientAudioSource.volume = ambientSound.volume;
        this.ambientAudioSource.loop = true;
        this.ambientAudioSource.spatialBlend = 1;
        this.ambientAudioSource.rolloffMode = AudioRolloffMode.Linear;
        this.ambientAudioSource.maxDistance = 20;
        this.ambientAudioSource.dopplerLevel = 0.15f;

        // play our ambient loop
        ambientAudioSource.Play();
    }
    public void Start()
    {
        this.SetupAudio();

    }
    public void Awake()
    {
        this.rend = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        this.level = GameObject.FindObjectOfType<Level>();
        if (this.isDropToken)
        {
            return;
        }

        if (this.isStarting)
        {
            Activate();
        }
        else
        {
            Hide();
            this.lastPickupTime = -1000;
        }
    }

    public void OnEnable()
    {
        if (this.isDropToken)
        {
            this.lastPickupTime = Time.time;
            Activate();
        }
    }

    public void Activate()
    {
        this.isActive = true;
        this.rend.enabled = true;
    }

    public void MoveTo(Vector3 pos)
    {
        StartCoroutine(Move(this.transform.position, pos, 0.05f));
    }

    public void OnTriggerEnter2D(Collider2D coll)
    {
        if (!this.isActive || !this.gameObject.activeSelf || (this.isStarting && Time.time - this.lastPickupTime < 2f))
        {
            return;
        }

        if (coll.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            return;
        }

        Player player = coll.gameObject.GetComponent<Player>();
        if (player.isDead)
        {
            return;
        }

        // Player picks up token
        pickupAudioSource.Play();
        ambientAudioSource.Stop();
        this.lastPickupTime = Time.time;
        Hide();
        this.level.Pickup(this.isDropToken == false);
    }

    private IEnumerator Move(Vector3 start, Vector3 destination, float duration)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            this.transform.position = Vector3.Lerp(start, destination, (Time.time - startTime) / duration);
            yield return null;
        }

        this.dropTokenPingPong.enabled = true;
    }

    private void Hide()
    {
        this.isActive = false;
        this.rend.enabled = false;

        if (this.isDropToken)
        {
            Pool.instance.Return(this.gameObject);
        }
    }
}