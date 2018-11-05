using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
	[Header("Spawn")]
	public Transform respawn;
	public float dramaTime;
	public float respawnTime;
	
	[Header("Prefabs")]
	public Player playerPrefab;

	private Player player;

	public void Awake() {
		this.player = GameObject.Instantiate<Player>(this.playerPrefab);
		this.player.Spawn(this.respawn.position);
	}

	public void PlayerDied() {
		StartCoroutine(WaitForRespawn());
	}

	private IEnumerator WaitForRespawn() {
		yield return new WaitForSeconds(this.dramaTime);

		// TODO: change camera's target to respawn location

		yield return new WaitForSeconds(this.respawnTime - this.dramaTime);

		this.player.Spawn(this.respawn.position);
	}
}
