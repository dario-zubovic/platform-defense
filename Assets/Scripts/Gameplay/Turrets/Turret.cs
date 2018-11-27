using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Turret : MonoBehaviour {
	public float radius;
	public float fireRate;

	[Header("Generic settings")]
	public LayerMask enemyLayers;

	[Header("Visuals")]
	public TurretInfo turretInfo;
	public UpgradeTurretDialog upgradeDialog;
    public CostIndicator costIndicator;
	public Transform barrel;
	public float slewHomeWait;
	public float slewHomeTime;

	private const float AIM_TIME = 0.1f;

	protected Collider2D[] overlapResults;

    private SortedList<float, GameObject> sortedOverlapResults;

	protected float lastFireTime;
	private float lastAimTime;

	private float lastBarrelAngle;

	private Player player;

	public void Awake() {
		this.overlapResults = new Collider2D[128];
        this.sortedOverlapResults = new SortedList<float, GameObject>(this.overlapResults.Length);

		this.lastFireTime = Time.time;

		SetBarrelRotation(0);
	
		Init();
	}

	public virtual void Update() {
		if(Time.time - this.lastFireTime > this.slewHomeWait) {
			float t = (Time.time - this.lastFireTime - this.slewHomeWait) / this.slewHomeTime;
			if(t <= 1) {
				float oldAngle = this.lastBarrelAngle;
				SetBarrelRotation(Mathf.LerpAngle(oldAngle, 0, t) * Mathf.Deg2Rad);
				this.lastBarrelAngle = oldAngle;
			}
		}
	}

	public virtual void FixedUpdate() {
		if(Time.time - this.lastFireTime > this.fireRate) {
			Aim();
		}
	}

	public virtual void ShowInfo() {
		CircleDrawer.instance.Draw(this.transform.position, this.radius);

		if(this.turretInfo != null) {
			this.turretInfo.gameObject.SetActive(true);
		}
	}

	public virtual void HideInfo() {
		CircleDrawer.instance.DontDraw();

		if(this.turretInfo != null) {
			if(this.upgradeDialog.gameObject.activeSelf) {
				CloseUpgradeDialog();
			}

			this.turretInfo.gameObject.SetActive(false);
		}
	}

    public void Build(Player player) {
		if(this.turretInfo == null) {
			return;
		}

		this.player = player;
		this.player.locked = true;
		
		this.upgradeDialog.gameObject.SetActive(true);
	}

    public virtual void Upgrade(int id) {
	}

    public virtual void PreviewUpgrade(int id) {
	}
    
	public virtual void CloseUpgradeDialog() {
        StartCoroutine(UnlockPlayer());

		this.turretInfo.ResetTempStats();
		CircleDrawer.instance.DisableSecondary();
		this.upgradeDialog.gameObject.SetActive(false);
		this.costIndicator.gameObject.SetActive(false);
	}


	protected virtual void Init() {

	}

	protected abstract void Fire(Enemy target);

	protected void FindTarget() {
		int c = Physics2D.OverlapCircleNonAlloc(this.transform.position, this.radius, this.overlapResults, this.enemyLayers);
		
        this.sortedOverlapResults.Clear();

		for(int i = 0; i < c; i++) {
            if(this.sortedOverlapResults.ContainsKey(this.overlapResults[i].transform.position.x)) {
                continue;
            }

            this.sortedOverlapResults.Add(this.overlapResults[i].transform.position.x, this.overlapResults[i].gameObject);
		}

		Enemy selected = Filter(this.sortedOverlapResults);
		if(selected == null) {
			return;
		}

		RotateBarrelTo(selected);
		Fire(selected);
		this.lastFireTime = Time.time;
	}

	protected virtual Enemy Filter(SortedList<float, GameObject> sortedOverlapResults) {
		if(sortedOverlapResults.Count == 0) {
			return null;
		}

		return sortedOverlapResults[0].GetComponent<Enemy>();
	}

	protected void RotateBarrelTo(Enemy target) {
		if(this.barrel == null) {
			return;
		}

		Vector2 delta = target.transform.position - this.barrel.position;

		float angle = Mathf.Atan2(delta.y, delta.x);
		SetBarrelRotation(angle);
	}

	protected void SetBarrelRotation(float angle) {
		if(this.barrel == null) {
			return;
		}
		angle *= Mathf.Rad2Deg;
		
		this.lastBarrelAngle = angle;

		bool flip = angle > -90 && angle < 90;
		this.barrel.localScale = new Vector3(flip ? -1 : 1, 1, 1);
		this.barrel.localEulerAngles = new Vector3(0, 0, angle + (flip ? 0 : 180));
	}

	private void Aim() {
		if(Time.time - this.lastAimTime < AIM_TIME && this.fireRate > AIM_TIME) {
			return; // don't aim every frame since it uses casting
		}
		this.lastAimTime = Time.time; 

		FindTarget();
	}

    protected IEnumerator UnlockPlayer() {
        yield return null;

        this.player.locked = false;
    }

#if UNITY_EDITOR
    
	public virtual void OnDrawGizmosSelected() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(this.transform.position, this.radius);
    }

#endif
}
