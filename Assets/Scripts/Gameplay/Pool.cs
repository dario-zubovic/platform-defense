using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour {
    public static Pool instance {
        get;
        private set;
    }

    private Dictionary<Object, Object[]> unusedObjects;

    public void Awake() {
        Pool.instance = this;
    }

    public GameObject Grab(GameObject prefab) {
        return null;
    }

    public T Grab<T>(T prefab) where T : Component {
        return null;

    }
}