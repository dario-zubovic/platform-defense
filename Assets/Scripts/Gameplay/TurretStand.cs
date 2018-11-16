using UnityEngine;

public class TurretStand : MonoBehaviour {
    public GameObject indicator;
    public Turret turretPrefab;
    public SpriteRenderer standSprite;

    private Level level;
    private Turret turret;

    public void Awake() {
        this.level = GameObject.FindObjectOfType<Level>();
    }

    public void Build() {
        if(this.turret != null) {
            return;
        }

        if(!this.level.TakeToken()) {
            return;
        }

        this.indicator.SetActive(false);
        this.standSprite.enabled = false;

        this.turret = GameObject.Instantiate<Turret>(this.turretPrefab, this.transform.position, Quaternion.identity);    
        
        this.turret.ShowInfo();
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
}