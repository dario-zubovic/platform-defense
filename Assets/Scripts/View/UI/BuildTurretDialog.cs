using UnityEngine;
public class BuildTurretDialog : CustomDialog
{
    public TurretStand stand;
    private AudioSource audioSource;
    private Sound uiMove;

    void Start()
    {
        // set up various UI sounds
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SoundManager.instance.uiOutputChannel;
    }

    protected override void Pressed(int selection)
    {
        this.stand.BuildTurret(selection);
        // play UI sound
        Sound sound = SoundManager.instance.GetSound(SoundId.UISelect);
        audioSource.PlayOneShot(sound.clip, sound.volume);
    }

    protected override void Close()
    {
        this.stand.CloseBuildDialog();
        // play UI sound
        // TODO: this should play on "not enough money" too
        Sound sound = SoundManager.instance.GetSound(SoundId.UIBack);
        audioSource.PlayOneShot(sound.clip, sound.volume);
    }

    protected override void SelectionChanged(int selection)
    {
        this.stand.PreviewTurret(selection);
        // play UI sound
        Sound sound = SoundManager.instance.GetSound(SoundId.UIMove);
        audioSource.PlayOneShot(sound.clip, sound.volume);
    }
}