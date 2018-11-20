using UnityEngine;

public class UpgradeTurretDialog : CustomDialog {
    public Turret turret;

    protected override void Pressed(int selection) {
        this.turret.Upgrade(selection);
    }
    
    protected override void Close() {
        this.turret.CloseUpgradeDialog();
    }

    protected override void SelectionChanged(int selection) {
        this.turret.PreviewUpgrade(selection);
    }
}