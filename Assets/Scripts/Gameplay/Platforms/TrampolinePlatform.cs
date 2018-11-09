using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolinePlatform : Platform {
	public float minVerticalSpeed;
	
	public override PlatformType type {
		get {
			return PlatformType.Trampoline;
		}
	}

	public override void Contact(Actor actor, RaycastHit2D hit, bool vertical) {
		if(hit.normal.y < 0.5f) {
			return;
		}

		Vector2 v = Vector2.Reflect(actor.GetVelocity(), Vector2.up);

		if(v.y < this.minVerticalSpeed) {
			v.y = this.minVerticalSpeed;
		}

		actor.SetVelocity(v);
	}
}
