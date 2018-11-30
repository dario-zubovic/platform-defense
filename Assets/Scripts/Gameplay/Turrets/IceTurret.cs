using System.Collections.Generic;
using UnityEngine;

public class IceTurret : Turret {
    [Header("Projectile")]
    public float speed;
    public SlowdownProjectile projectilePrefab;
    public LayerMask wallLayer;
    public float blastRadius;
    public float minDuration, maxDuration;
    public float amount;

    [Header("Upgrades")]
    public TurretUpgrade[] amountUpgrades;
    public TurretUpgrade[] durationLowUpgrades;
    [Tooltip("Cost is ignored for this one")] public TurretUpgrade[] durationHighUpgrades;

    private RaycastHit2D[] raycastResults;

    // upgrades:
	private int amountLevel = -1;
	private int durationLevel = -1;
    private int turretLevel = 0;

    public void Start() {
        this.raycastResults = new RaycastHit2D[32];
        
        Sound sound = SoundManager.instance.GetSound(SoundId.IceTurretShot);
        this.audioSource.clip = sound.clip;
        this.audioSource.volume = sound.volume;
        this.audioSource.outputAudioMixerGroup = sound.outputChannel;
    }

    public override void Update() {
        base.Update();
    }

    protected override void Fire(Enemy target) {
        this.audioSource.Play();

        Vector2 dir = (target.transform.position - this.transform.position).normalized;
        SlowdownProjectile projectile = Pool.instance.Grab<SlowdownProjectile>(this.projectilePrefab);
        projectile.transform.position = this.transform.position;
        projectile.rigid.gravityScale = 0f;
        projectile.rigid.velocity = this.speed * dir;
        projectile.blastRadius = this.blastRadius;
        projectile.minDuration = this.minDuration;
        projectile.maxDuration = this.maxDuration;
        projectile.amount = this.amount;
        projectile.owner = this;
    }

	protected override Enemy Filter(SortedList<float, GameObject> sortedOverlapResults) {
        GameObject targetGo = null;

        for(int i = 0; i < sortedOverlapResults.Count; i++) {
            if(CanBeReached(sortedOverlapResults.Values[i].transform.position)) {
                targetGo = sortedOverlapResults.Values[i];
                break;
            }
        }

		if(targetGo == null) {
			return null;
		}

		return targetGo.GetComponent<Enemy>();
    }
    
    public override void Upgrade(int id) {
        TurretUpgrade upgrade = GetUpgradeById(id);
        if(upgrade == null) {
            return;
        }

        if(!Level.instance.TakeTokensAndGold(upgrade.tokenCost, upgrade.goldCost)) {
            return;
        }

        if(id == 0) {
            this.amountLevel++;
            this.amount = this.amountUpgrades[this.amountLevel].value;
            SetStats();

            if(this.amountLevel == this.amountUpgrades.Length - 1) {
                this.upgradeDialog.buttons[0].gameObject.SetActive(false);
            }
        } else if(id == 1) {
            this.durationLevel++;
            this.minDuration = this.durationLowUpgrades[this.durationLevel].value;
            this.maxDuration = this.durationHighUpgrades[this.durationLevel].value;
            SetStats();

            if(this.durationLevel == this.durationLowUpgrades.Length - 1) {
                this.upgradeDialog.buttons[1].gameObject.SetActive(false);
            }
        }

        this.turretLevel++;
        this.turretInfo.SetLevel(this.turretLevel);
        if(this.turretLevel == 3) {
            this.upgradeDialog.buttons[0].gameObject.SetActive(false);
            this.upgradeDialog.buttons[1].gameObject.SetActive(false);
        }

        CloseUpgradeDialog();
    }

    public override void ShowInfo() {
        base.ShowInfo();

        SetStats();
    }
    
    public override void PreviewUpgrade(int id) {
		this.turretInfo.ResetTempStats();

		TurretUpgrade upgrade = GetUpgradeById(id);

        if(id == 0) {
            this.turretInfo.SetTempStat(0, Mathf.RoundToInt(upgrade.value * 100f) + "%");
        } else if(id == 1) {
            this.turretInfo.SetTempStat(1, Mathf.RoundToInt(upgrade.value) + "-" + Mathf.RoundToInt(this.durationHighUpgrades[this.durationLevel + 1].value) + "s");
        }

        if(upgrade == null) {
            this.costIndicator.gameObject.SetActive(false);
        } else {
            this.costIndicator.gameObject.SetActive(true);
            this.costIndicator.SetCost(upgrade.tokenCost, upgrade.goldCost);
        }
    }

    
    private TurretUpgrade GetUpgradeById(int id) {
        switch(id) {
            case 0:
                return this.amountUpgrades[this.amountLevel + 1];
            case 1:
                return this.durationLowUpgrades[this.durationLevel + 1];
        }

        return null;
    }
    
    private void SetStats() {
        this.turretInfo.SetStats(
            Mathf.RoundToInt(this.amount * 100f) + "%",
            Mathf.RoundToInt(this.minDuration) + "-" + Mathf.RoundToInt(this.maxDuration) + "s"
        );
    }

    private bool CanBeReached(Vector2 target) {
        Vector2 dir = target - (Vector2)this.barrel.position;
        
        int c = Physics2D.RaycastNonAlloc(this.barrel.position, dir, this.raycastResults, dir.magnitude, this.wallLayer);

        for(int i = 0; i < c; i++) {
            if(this.raycastResults[i].collider != null && !this.raycastResults[i].collider.isTrigger) {
                return false;
            }
        }

        return true;
    }

#if UNITY_EDITOR
    
	public override void OnDrawGizmosSelected() {
		base.OnDrawGizmosSelected();

        Gizmos.color = new Color(0.9f, 0.5f, 0f, 1f);
		Gizmos.DrawWireSphere(this.transform.position + Vector3.right * this.radius, this.blastRadius);
    }

#endif
}