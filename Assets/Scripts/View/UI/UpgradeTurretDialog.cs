using UnityEngine;

public class UpgradeTurretDialog : CustomDialog {
    public Turret turret;

    protected override void Pressed(int selection) {
        this.turret.Upgrade(selection);
        
        SoundManager.instance.PlaySfx(SoundId.UISelect);
    }
    
    protected override void Close() {
        this.turret.CloseUpgradeDialog();
        
        SoundManager.instance.PlaySfx(SoundId.UIBack);
    }

    protected override void SelectionChanged(int selection) {
        this.turret.PreviewUpgrade(selection);
        
        SoundManager.instance.PlaySfx(SoundId.UIMove);
    }
}