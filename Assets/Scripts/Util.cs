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

    public static float BounceEaseOut(float a, float b, float t) {
        if(t < 1f / 2.75f) {
            return b * ( 7.5625f * t * t ) + a;
        } else if(t <  2f / 2.75f) {
            return b * ( 7.5625f * ( t -= ( 1.5f / 2.75f ) ) * t + .75f ) + a;
        } else if(t < 2.5f / 2.75f) {
            return b * ( 7.5625f * ( t -= ( 2.25f / 2.75f ) ) * t + .9375f ) + a;
        } else {
            return b * ( 7.5625f * ( t -= ( 2.625f / 2.75f ) ) * t + .984375f ) + a;
        }
    }
}