using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour {
	public int startLives;

	[Header("Spawn")]
	public Transform respawn;
	public float dramaTime;
	public float respawnTime;
	public Bounds levelBounds;

	[Header("Tokens")]
	public LayerMask tokenCollisionMask;
	
	[Header("Prefabs")]
	public Player playerPrefab;
	public Token dropTokenPrefab;
	public Checkpoint checkpointPrefab;

	[Header("UI")]
	public Text nextWaveText;
	public Text tokensText;
	public Text goldText;
	public Image[] lifeIndicators;

	[Header("Enemies and waves")]
	public Transform[] enemySpawns;
	public WaveDescription[] waves;

	public static Level instance {
		get;
		private set;
	}

	private CameraController cameraController;
	private Player player;

	private int collectedTokens = 999;
	private int collectedGold = 9999;
	private List<Token> tokens;
	private List<Token> droppedTokens;

	private Checkpoint checkpoint;

	private int lives;

	public void Awake() {
		this.cameraController = Camera.main.GetComponent<CameraController>();

		this.player = GameObject.Instantiate<Player>(this.playerPrefab);

		this.tokens = new List<Token>(GameObject.FindObjectsOfType<Token>());
		this.droppedTokens = new List<Token>();

		Level.instance = this;
	}

	public void Start() {
		this.player.Spawn(this.respawn.position);
		this.cameraController.SetTarget(this.player.transform, false);
		this.cameraController.bounds = this.levelBounds;

		this.lives = this.startLives;
		ChangeLivesNum(0);

		StartCoroutine(SpawnWaves());
	}

	public void PlayerDied() {
		DropTokens();

		ChangeLivesNum(-1);

		StartCoroutine(WaitForRespawn());
	}

	public void Pickup(bool spawnNew) {
		ChangeTokensNum(1);

		if(spawnNew) {
			StartCoroutine(SpawnNewToken());
		}
	}

	public bool TakeToken() {
		if(this.collectedTokens == 0) {
			return false;
		}

		ChangeTokensNum(-1);
		return true;
	}

	public bool TakeTokensAndGold(int tokens, int gold) {
		if(this.collectedTokens < tokens || this.collectedGold < gold) {
			return false;
		}

		ChangeTokensNum(-tokens);
		ChangeGoldNum(-gold);
		return true;
	}

	public void AddGold(int delta) {
		ChangeGoldNum(delta);
	}

	public int GetTokenCount() {
		return this.collectedTokens;
	}

	public int GetGoldCount() {
		return this.collectedGold;
	}

	public void BuildCheckpoint(Vector2 position) {
		if(this.checkpoint != null && this.checkpoint.gameObject != null && this.checkpoint.gameObject.activeSelf) {
        	Pool.instance.Return(this.checkpoint.gameObject);
		}

		this.checkpoint = Pool.instance.Grab<Checkpoint>(this.checkpointPrefab);
		this.checkpoint.transform.position = position;
	}

	private void ChangeTokensNum(int delta) {
		this.collectedTokens += delta;
		this.tokensText.text = this.collectedTokens.ToString();
	}

	private void ChangeGoldNum(int delta) {
		this.collectedGold += delta;
		this.goldText.text = this.collectedGold.ToString();
	}

	private void ChangeLivesNum(int delta) {
		this.lives += delta;

		for(int i = 0; i < this.lifeIndicators.Length; i++) {
			this.lifeIndicators[i].gameObject.SetActive(i < this.lives);
		}
	}

	private IEnumerator WaitForRespawn() {
		Time.timeScale = 0.025f;
		this.cameraController.StartDrama(this.dramaTime);

		yield return new WaitForSecondsRealtime(this.dramaTime);
		
		Time.timeScale = 1f;
		this.cameraController.StopDrama();

		Transform respawnPoint = this.checkpoint == null ? this.respawn : this.checkpoint.transform;

		this.cameraController.SetFocus(false, false);
		this.cameraController.SetTarget(respawnPoint, true);

		yield return new WaitForSeconds(this.respawnTime - this.dramaTime);
		
		this.player.Spawn(respawnPoint.position);
		this.cameraController.SetTarget(this.player.transform, false);

		if(this.checkpoint != null) {
			this.checkpoint.lives--;
			if(this.checkpoint.lives == 0) {
        		Pool.instance.Return(this.checkpoint.gameObject);
				this.checkpoint = null;
			}
		}
	}

	private IEnumerator SpawnNewToken() {
		yield return new WaitForSeconds(5f);

		Token selected = null;
		List<Token> availableTokens = new List<Token>(this.tokens.Count);
		float totalWeights = 0;

		while(selected == null) {
			totalWeights = 0;
			availableTokens.Clear();

			foreach(var token in this.tokens) {
				if(!token.canBeActivated) {
					continue;
				}

				availableTokens.Add(token);
				totalWeights += token.weight;
			}

			if(availableTokens.Count == 0) {
				yield return new WaitForSeconds(0.5f);
				continue;
			}

			float r = Random.value * totalWeights;
			float i = 0;
			foreach(var token in availableTokens) {
				if(r > i && r <= i + token.weight) {
					selected = token;
					break;
				} else {
					i += token.weight;
				}
			}
		}

		selected.Activate();
	}

	private void DropTokens() {
		foreach(var token in this.droppedTokens) {
			if(token == null || token.gameObject == null || !token.gameObject.activeSelf) {
				continue;
			}

        	Pool.instance.Return(token.gameObject);
		}
		this.droppedTokens.Clear();

		int leftovers = Mathf.CeilToInt(this.collectedTokens / 2f);
		this.collectedTokens = 0;
		ChangeTokensNum(0);

		for(int i = 0; i < leftovers; i++) {
			
			Token token = Pool.instance.Grab<Token>(this.dropTokenPrefab);
			token.transform.position = this.player.transform.position;

			Vector2 safePos = FindDropTokenSafeLocation(token);
			token.transform.position = this.player.transform.position + Vector3.up;
			token.MoveTo(safePos);

			this.droppedTokens.Add(token);
		}
	}

	private Vector2 FindDropTokenSafeLocation(Token token) {
		Collider2D[] results = new Collider2D[8];

		Vector2 startPos = token.transform.position;

		for(int i = 0; i < 64; i++) {
			Vector2 pos = startPos + Random.insideUnitCircle * 6f;

			int c = Physics2D.OverlapCircleNonAlloc(pos, 0.8f, results, this.tokenCollisionMask);
			if(c == 0) {
				return pos;
			}
		}

		return startPos;
	}

	private IEnumerator SpawnWaves() {
		float lastSpawnTime = Time.time;

		WaitForSeconds delay = new WaitForSeconds(0.5f);

		foreach(WaveDescription wave in this.waves) {
			this.nextWaveText.enabled = true;
			while(Time.time - lastSpawnTime < wave.delay) {
				this.nextWaveText.text = Mathf.RoundToInt(wave.delay - Time.time + lastSpawnTime).ToString();
				yield return delay;
			}
			this.nextWaveText.enabled = false;

			yield return wave.wave.Spawn(this);
			lastSpawnTime = Time.time;
		}

		yield return null;
	}

#if UNITY_EDITOR
    
	public void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
		Gizmos.DrawWireCube(this.levelBounds.center, this.levelBounds.extents);	
	}

#endif
}

[System.Serializable]
public class WaveDescription {
	public float delay;
	public EnemyWave wave;
}