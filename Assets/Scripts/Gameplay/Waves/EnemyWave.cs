using System.Collections;
using UnityEngine;

public abstract class EnemyWave : ScriptableObject {
    public string description;
    
    public abstract IEnumerator Spawn(Level level);
}