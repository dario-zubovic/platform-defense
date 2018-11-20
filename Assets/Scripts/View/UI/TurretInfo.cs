using TMPro;
using UnityEngine;

public class TurretInfo : MonoBehaviour {
    public TextMeshPro[] statTexts;

    private string[] stats;

    public void SetStats(params string[] stats) {
        this.stats = stats;

        for(int i = 0; i < stats.Length; i++) {
            this.statTexts[i].text = stats[i];
        }
    }

    public void SetTempStat(int id, string stat) {
        this.statTexts[id].text = "<color=#7F00DBFF>" + stat + "</color>";
    }

    public void ResetTempStats() {
        for(int i = 0; i < this.stats.Length; i++) {
            this.statTexts[i].text = this.stats[i];
        }
    }
}