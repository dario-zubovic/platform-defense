using System.Collections;
using UnityEngine;

public abstract class EnemyWave : ScriptableObject {
    public abstract IEnumerator Spawn(Level level);
}