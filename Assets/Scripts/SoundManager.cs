using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class SoundManager : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioMixerGroup environmentOutputChannel;

    public List<Sound> sounds;

    public float spatialSoundsMinDistance, spatialSoundsMaxDistance;
    public AnimationCurve spatialSoundFalloffCurve;

    public static SoundManager instance {
        get;
        private set;
    }

    private MaterialZone.Zone materialZone;

    private Player player;

    public void Awake() {
        SoundManager.instance = this;

        // begin playing music
        this.musicSource.Play();
        this.sfxSource.outputAudioMixerGroup = this.environmentOutputChannel;
    }

    public void SetZone(MaterialZone.Zone zone) {
        this.materialZone = zone;
    }

    public void SetPlayer(Player player) {
        this.player = player;
    }

    public void PlayPlayerJumpSfx() {
        SoundId id = SoundId.None;
        switch (this.materialZone) {
            case MaterialZone.Zone.Grass:
                id = SoundId.PlayerJumpGrass;
                break;
            case MaterialZone.Zone.Rock:
                id = SoundId.PlayerJumpRock;
                break;
            case MaterialZone.Zone.Wood:
                id = SoundId.PlayerJumpWood;
                break;

            default:
                return;
        }

        PlaySfx(id);
    }

    public void PlayPlayerLandSfx() {
        SoundId id = SoundId.None;
        switch (this.materialZone) {
            case MaterialZone.Zone.Grass:
                id = SoundId.PlayerLandGrass;
                break;
            case MaterialZone.Zone.Rock:
                id = SoundId.PlayerLandRock;
                break;
            case MaterialZone.Zone.Wood:
                id = SoundId.PlayerLandWood;
                break;

            default:
                return;
        }

        PlaySfx(id);
    }

    public void PlayPlayerStepSfx() {
        SoundId id = SoundId.None;
        switch (this.materialZone) {
            case MaterialZone.Zone.Grass:
                id = SoundId.PlayerStepGrass;
                break;
            case MaterialZone.Zone.Rock:
                id = SoundId.PlayerStepRock;
                break;
            case MaterialZone.Zone.Wood:
                id = SoundId.PlayerStepWood;
                break;

            default:
                return;
        }

        PlaySfx(id);
    }

    public Sound GetSound(SoundId id) {
        return this.sounds.Find(o => o.id == id);
    }

    public void PlaySfx(SoundId id) {
        Sound sound = this.sounds.Find(o => o.id == id);
        if (sound == null) {
            Debug.LogWarning("Couldn't find sound with id " + id.ToString());
            return;
        }

        sound.ApplyPitch(this.sfxSource);

        this.sfxSource.PlayOneShot(sound.clip, sound.volume);
    }

    public void PlayAtPosition(SoundId id, Vector2 pos) {
        float dist = Vector2.Distance(this.player.transform.position, pos);
        if(dist > this.spatialSoundsMaxDistance) {
            return;
        }

        Sound sound = this.sounds.Find(o => o.id == id);
        if (sound == null) {
            Debug.LogWarning("Couldn't find sound with id " + id.ToString());
            return;
        }

        sound.ApplyPitch(this.sfxSource);
    
        float t = (dist - this.spatialSoundsMinDistance) / (this.spatialSoundsMaxDistance - this.spatialSoundsMinDistance);
        t = Mathf.Clamp01(t);

        this.sfxSource.PlayOneShot(sound.clip, sound.volume * this.spatialSoundFalloffCurve.Evaluate(t));
    }
}

[System.Serializable]
public class Sound {
    public SoundId id;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;

    public AudioMixerGroup outputChannel;

    public bool randomPitch;
    [Range(0, 2f)]
    public float pitchMin = 1f, pitchMax = 1;

    public void ApplyPitch(AudioSource source) {
        if(this.randomPitch) {
            source.pitch = Random.Range(this.pitchMin, this.pitchMax);
        } else {
            source.pitch = 1f;
        }
    }
}

[System.Serializable]
public enum SoundId {
    None,

    PlayerJumpGrass,
    PlayerJumpRock,
    PlayerJumpWood,
    PlayerStepGrass,
    PlayerStepRock,
    PlayerStepWood,

    BulletTurretShot,
    BombTurretShot,
    IceTurretShot,

    TokenPickup,
    TokenAmbient,
    UIMove,
    UIBack,
    UISelect,

    BombTurretHit,
    IceTurretHit,

    Placeholder1,
    PlayerSpikesDeath,

    JumpyPlatform,

    BouncyEnemyJump,
    BouncyEnemyDeath,
    
    RunningEnemyDeath,

    PlayerLandGrass,
    PlayerLandRock,
    PlayerLandWood,

    Placeholder2,
    Placeholder3,
    Placeholder4,
    Placeholder5,
    Placeholder6,
    Placeholder7,
    Placeholder8,
    Placeholder9,

}