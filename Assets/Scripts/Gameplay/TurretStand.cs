using System.Collections;
using UnityEngine;

public class TurretStand : MonoBehaviour {
    public GameObject indicator;
    public SpriteRenderer standSprite;
    public BuildTurretDialog buildDialog;
    
    [Header("Build")]
    public Turret[] turretPrefabs;

    private Level level;
    private Turret turret;
    private Player player;

    public void Awake() {
        this.level = GameObject.FindObjectOfType<Level>();
    }

    public void Build(Player player) {
        if(this.turret != null) {
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
    }

    public void BuildTurret(int id) {
        if(!this.level.TakeToken()) {
            return;
        }

        CloseBuildDialog();

        this.turret = GameObject.Instantiate<Turret>(this.turretPrefabs[id], this.transform.position, Quaternion.identity);    
        this.turret.ShowInfo();

        this.standSprite.enabled = false;
        this.indicator.SetActive(false); // turn off indicator for good
    }

    public void CloseBuildDialog() {
        StartCoroutine(UnlockPlayer());

        this.indicator.SetActive(true);
        this.buildDialog.gameObject.SetActive(false);
    }

    private IEnumerator UnlockPlayer() {
        yield return null;

        this.player.locked = false;
    }
}