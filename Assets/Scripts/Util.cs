using System.Collections;
using UnityEngine;

public class Util : MonoBehaviour {
    private static Util _instance;
    private static Util instance {
        get {
            if(_instance == null) {
                GameObject go = new GameObject("Util");
                _instance = go.AddComponent<Util>();
            }

            return _instance;
        }
    }

    public static Coroutine Coroutine(IEnumerator routine) {
        return instance.StartCoroutine(routine);
    }
}