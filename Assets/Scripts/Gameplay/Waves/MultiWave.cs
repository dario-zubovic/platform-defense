using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "MultiEnemyWave", order = 1)]
public class MultiWave : EnemyWave {
    public List<EnemyWave> waves;

    public override IEnumerator Spawn(Level level) {
        foreach(var wave in this.waves) {
            Util.Coroutine(wave.Spawn(level));
        }

        yield return null;
    }
}