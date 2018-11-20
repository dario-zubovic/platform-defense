using System.Collections.Generic;
using UnityEngine;

public class BulletTurret : Turret {
    [Header("Projectile")]
    public LayerMask wallLayer;
    public float damage;

	[Header("Upgrades")]
	public float[] damageUpgrades;
	public float[] fireRateUpgrades;
	public float[] rangeUpgrades;

    private RaycastHit2D[] raycastResults;

	// upgrades:
	private int damageLevel = -1;
	private int fireRateLevel = -1;
	private int rangeLevel = -1;

    protected override void Init() {
        this.raycastResults = new RaycastHit2D[32];
    }

    public override void Update() {
        base.Update();
    }

    public override void ShowInfo() {
        base.ShowInfo();

        SetStats();
    }

    protected override void Fire(Enemy target) {
        target.TakeDamage(this.damage);
    }

    public override void Upgrade(int id) {
		if(id == 0) {
			this.damageLevel++;
			this.damage = this.damageUpgrades[this.damageLevel];
            SetStats();

            if(this.damageLevel == this.damageUpgrades.Length - 1) {
                this.upgradeDialog.buttons[0].gameObject.SetActive(false);
            }
		} else if(id == 1) {
            this.fireRateLevel++;
            this.fireRate = this.fireRateUpgrades[this.fireRateLevel];
            SetStats();

            if(this.fireRateLevel == this.fireRateUpgrades.Length - 1) {
                this.upgradeDialog.buttons[1].gameObject.SetActive(false);
            }
        } else if(id == 2) {
            this.rangeLevel++;
            this.radius = this.rangeUpgrades[this.rangeLevel];
            CircleDrawer.instance.Draw(this.transform.position, this.radius);

            if(this.rangeLevel == this.rangeUpgrades.Length - 1) {
                this.upgradeDialog.buttons[2].gameObject.SetActive(false);
            }
        }

        CloseUpgradeDialog();
	}

    public override void PreviewUpgrade(int id) {
		this.turretInfo.ResetTempStats();
		CircleDrawer.instance.DisableSecondary();

		if(id == 0) {
			this.turretInfo.SetTempStat(0, this.damageUpgrades[this.damageLevel + 1].ToString("0.0"));
		} else if(id == 1) {
			this.turretInfo.SetTempStat(1, this.fireRateUpgrades[this.fireRateLevel + 1].ToString("0.0") + "s");
		} else if(id == 2) {
			CircleDrawer.instance.DrawSecondary(this.rangeUpgrades[this.rangeLevel + 1]);
		}
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

    private bool CanBeReached(Vector2 target) {
        Vector2 dir = target - (Vector2)this.transform.position;
        
        int c = Physics2D.RaycastNonAlloc(this.transform.position, dir, this.raycastResults, dir.magnitude, this.wallLayer);

        for(int i = 0; i < c; i++) {
            if(this.raycastResults[i].collider != null && !this.raycastResults[i].collider.isTrigger) {
                return false;
            }
        }

        return true;
    }

    private void SetStats() {
        this.turretInfo.SetStats(this.damage.ToString("0.0"), this.fireRate.ToString("0.0") + "s");
    }
}