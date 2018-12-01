using System.Collections;
using UnityEngine;

public abstract class EnemyWave : ScriptableObject {
    public string description;

    public bool upSpawn, downSpawn;
    
    public abstract IEnumerator Spawn(Level level);
}