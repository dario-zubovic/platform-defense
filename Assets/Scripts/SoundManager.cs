using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class SoundManager : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioSource musicSource;

    public List<Sound> sounds;
    // The mixer that routes to global environmental effects such as reverb
    public AudioMixerGroup sfxOutput;

    public static SoundManager instance
    {
        get;
        private set;
    }

    private MaterialZone.Zone materialZone;

    // private Player player;

    public void Awake()
    {
        SoundManager.instance = this;

        // set the correct audio output for each effect.
        sfxSource.outputAudioMixerGroup = sfxOutput;

        // begin playing music
        musicSource.Play();
        // this.player = GameObject.FindObjectOfType<Player>();
    }

    public void SetZone(MaterialZone.Zone zone)
    {
        this.materialZone = zone;
    }

    public void PlayPlayerJumpSfx()
    {
        // Figure out which sound should be played based on the MaterialZone
        SoundId id = SoundId.None;
        switch (this.materialZone)
        {
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

    public void PlayPlayerStepSfx()
    {
        SoundId id = SoundId.None;
        switch (this.materialZone)
        {
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

    public Sound GetSound(SoundId id)
    {
        return this.sounds.Find(o => o.id == id);
    }

    private void PlaySfx(SoundId id)
    {
        Sound sound = this.sounds.Find(o => o.id == id);
        if (sound == null)
        {
            Debug.LogWarning("Couldn't find sound with id " + id.ToString());
            return;
        }

        this.sfxSource.PlayOneShot(sound.clip, sound.volume);
    }
}

[System.Serializable]
public class Sound
{
    public SoundId id;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
}

[System.Serializable]
public enum SoundId
{
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
    Placeholder2,
    Placeholder3,
    Placeholder4,
    Placeholder5,
    Placeholder6,
    Placeholder7,
    Placeholder8,
    Placeholder9,
}