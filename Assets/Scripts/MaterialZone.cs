using UnityEngine;

public class MaterialZone : MonoBehaviour {
    public Zone zone;

    private LayerMask playerLayer;

    public void Awake() {
        this.playerLayer = LayerMask.NameToLayer("Player");
    }

    public void OnTriggerStay2D(Collider2D coll) {
		if(!coll.gameObject.CompareTag("Actor") || coll.gameObject.layer != this.playerLayer) {
			return;
		}

        SoundManager.instance.SetZone(this.zone);
    }

    [System.Serializable]
    public enum Zone {
        None,

        Grass,
        Rock,
        Wood,
    }
}