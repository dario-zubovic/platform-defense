using TMPro;
using UnityEngine;

public class CostIndicator : MonoBehaviour {
    public TextMeshPro crystalCostText, goldCostText;

    public void SetCost(int crystal, int gold) {
        int availableCrystal = Level.instance.GetTokenCount();
        int availableGold = Level.instance.GetGoldCount();
    
        if(crystal > availableCrystal) {
            this.crystalCostText.text = "<color=#800000ff>" + crystal + "</color>";
        } else {
            this.crystalCostText.text = crystal.ToString();
        }
        
        if(gold > availableGold) {
            this.goldCostText.text = "<color=#800000ff>" + gold + "</color>";
        } else {
            this.goldCostText.text = gold.ToString();
        }
    }
}