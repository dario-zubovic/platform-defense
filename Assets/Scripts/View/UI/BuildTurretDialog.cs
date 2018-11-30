using UnityEngine;
public class BuildTurretDialog : CustomDialog {
    public TurretStand stand;

    protected override void Pressed(int selection) {
        this.stand.BuildTurret(selection);
        
        SoundManager.instance.PlaySfx(SoundId.UISelect);
    }

    protected override void Close() {
        this.stand.CloseBuildDialog();
        
        SoundManager.instance.PlaySfx(SoundId.UIBack);
    }

    protected override void SelectionChanged(int selection) {
        this.stand.PreviewTurret(selection);
        
        SoundManager.instance.PlaySfx(SoundId.UIMove);
    }
}