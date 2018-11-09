using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Platform : MonoBehaviour {
	public abstract PlatformType type {
		get;
	}

	public void Awake() {
		this.tag = "Platform";
	}

	public abstract void Contact(Actor actor, RaycastHit2D hit, bool vertical);
}

public enum PlatformType {
	Moving,
	Disappearing,
	Trampoline,
}