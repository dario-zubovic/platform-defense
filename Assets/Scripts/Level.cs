using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
	[Header("Spawn")]
	public Transform respawn;
	public float dramaTime;
	public float respawnTime;
	public Bounds levelBounds;
	
	[Header("Prefabs")]
	public Player playerPrefab;

	private CameraController cameraController;
	private Player player;

	public void Awake() {
		this.cameraController = Camera.main.GetComponent<CameraController>();

		this.player = GameObject.Instantiate<Player>(this.playerPrefab);
	}

	public void Start() {
		this.player.Spawn(this.respawn.position);
		this.cameraController.SetTarget(this.player.transform, false);
		this.cameraController.bounds = this.levelBounds;
	}

	public void PlayerDied() {
		StartCoroutine(WaitForRespawn());
	}

	private IEnumerator WaitForRespawn() {
		yield return new WaitForSeconds(this.dramaTime);
		
		this.cameraController.SetFocus(false, false);
		this.cameraController.SetTarget(this.respawn, true);

		yield return new WaitForSeconds(this.respawnTime - this.dramaTime);

		this.player.Spawn(this.respawn.position);
		this.cameraController.SetTarget(this.player.transform, false);
	}

#if UNITY_EDITOR
    
	public void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
		Gizmos.DrawWireCube(this.levelBounds.center, this.levelBounds.extents);	
	}

#endif
}
