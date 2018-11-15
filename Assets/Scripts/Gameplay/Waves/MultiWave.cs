using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "MultiEnemyWave", order = 1)]
public class MultiWave : EnemyWave {
    public List<SingleWave> waves;
    public List<float> delays;

    public override IEnumerator Spawn(Level level) {
        float totalWait = 0f;
        for(int i = 0; i < this.waves.Count; i++) {
            SingleWave wave = this.waves[i];
            float delay = this.delays[i];

            Util.Coroutine(wave.SpawnDelayed(delay, level));
            
            float wait = wave.period * wave.count + delay;
            totalWait = Mathf.Max(wait, totalWait);
        }

        yield return new WaitForSeconds(totalWait);
    }
}