using UnityEngine;

public class BuildTurretDialog : CustomDialog {
    public TurretStand stand;

    protected override void Pressed(int selection) {
        this.stand.BuildTurret(selection);
    }
    
    protected override void Close() {
        this.stand.CloseBuildDialog();
    }
}