using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour {
    public static Pool instance {
        get;
        private set;
    }

    private Dictionary<GameObject, List<GameObject>> unusedObjects; // prefab -> objects
    private Dictionary<GameObject, GameObject> usedObjects; // object -> prefab

    public void Awake() {
        this.unusedObjects = new Dictionary<GameObject, List<GameObject>>();
        this.usedObjects = new Dictionary<GameObject, GameObject>();

        Pool.instance = this;
    }

    public T Grab<T>(T prefab) where T : Component {
        return Grab(prefab.gameObject).GetComponent<T>();
    }

    public GameObject Grab(GameObject prefab) {
        List<GameObject> objects;

        if(this.unusedObjects.ContainsKey(prefab)) {
            objects = this.unusedObjects[prefab];
        } else {
            objects = new List<GameObject>(8);
            this.unusedObjects[prefab] = objects;
        }

        GameObject obj;

        if(objects.Count > 0) {
            obj = objects[objects.Count - 1];
            objects.Remove(obj);
        } else {
            obj = GameObject.Instantiate(prefab);
        }

        this.usedObjects.Add(obj, prefab);

        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj) {
        obj.SetActive(false);
        
        GameObject prefab = this.usedObjects[obj];
        this.usedObjects.Remove(obj);
        this.unusedObjects[prefab].Add(obj);
    }
}