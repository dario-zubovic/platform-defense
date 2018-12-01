using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour {
	private int enemyLayer;

	public void Awake() {
		this.enemyLayer = LayerMask.NameToLayer("Enemy");
	}

	public void OnTriggerEnter2D(Collider2D trigger) {
		if(!trigger.CompareTag("Actor")) {
			return;
		}

		if(trigger.gameObject.layer == this.enemyLayer) {
			Pool.instance.Return(trigger.gameObject);

			Level.instance.ChangeLivesNum(-1);
		}
	}
}
