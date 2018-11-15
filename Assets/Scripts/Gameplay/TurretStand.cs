using UnityEngine;

public class TurretStand : MonoBehaviour {
    public GameObject indicator;
    public Turret turretPrefab;

    private bool built;
    private Level level;

    public void Awake() {
        this.level = GameObject.FindObjectOfType<Level>();
    }

    public void Build() {
        if(this.built) {
            return;
        }

        if(!this.level.TakeToken()) {
            return;
        }

        this.indicator.SetActive(false);
        Turret turret = GameObject.Instantiate<Turret>(this.turretPrefab, this.transform.position, Quaternion.identity);
        this.built = true;
    
    }
}