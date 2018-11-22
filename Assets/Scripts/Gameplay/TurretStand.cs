using System.Collections;
using UnityEngine;

public class TurretStand : MonoBehaviour {
    public GameObject indicator;
    public SpriteRenderer standSprite;
    public BuildTurretDialog buildDialog;
    public CostIndicator costIndicator;
    
    [Header("Build")]
    public Turret[] turretPrefabs;
    public int[] goldCosts;

    private Level level;
    private Turret turret;
    private Player player;

    public void Awake() {
        this.level = GameObject.FindObjectOfType<Level>();
    }

    public void Build(Player player) {
        if(this.turret != null) {
            this.turret.Build(player);
            return;
        }

        this.player = player;
        this.player.locked = true;

        this.indicator.SetActive(false);
        this.buildDialog.gameObject.SetActive(true);
    }

    public void Hover() {
        if(this.turret != null) {
            this.turret.ShowInfo();
            return;
        }
    }

    public void Unhover() {
        if(this.turret != null) {
            this.turret.HideInfo();
            return;
        }

        if(this.buildDialog.gameObject.activeSelf) {
            CloseBuildDialog();
        }
    }

    public void BuildTurret(int id) {
        if(!this.level.TakeTokensAndGold(1, this.goldCosts[id])) {
            return;
        }

        CloseBuildDialog();

        this.turret = GameObject.Instantiate<Turret>(this.turretPrefabs[id], this.transform.position, Quaternion.identity);    
        this.turret.ShowInfo();

        this.standSprite.enabled = false;
        this.indicator.SetActive(false); // turn off indicator for good
    }

    public void PreviewTurret(int id) {
        if(id >= this.turretPrefabs.Length) {
    		CircleDrawer.instance.DontDraw();
            this.costIndicator.gameObject.SetActive(false);
            return;
        }

        CircleDrawer.instance.Draw(this.transform.position, -1);
        CircleDrawer.instance.DrawSecondary(this.turretPrefabs[id].radius);
        this.costIndicator.SetCost(1, this.goldCosts[id]);

        if(!this.costIndicator.gameObject.activeSelf) {
            this.costIndicator.gameObject.SetActive(true);
        }
    }

    public void CloseBuildDialog() {
        StartCoroutine(UnlockPlayer());

        this.indicator.SetActive(true);
        this.buildDialog.gameObject.SetActive(false);
        this.costIndicator.gameObject.SetActive(false);
        
        CircleDrawer.instance.DontDraw();
    }

    private IEnumerator UnlockPlayer() {
        yield return null;

        this.player.locked = false;
    }
}