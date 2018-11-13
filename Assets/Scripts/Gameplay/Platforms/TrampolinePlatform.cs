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
		if(!vertical || hit.normal.y < 0.5f) {
			return;
		}

		Vector2 v = actor.GetVelocity();
		if(v.y > -0.2f) {
			return;
		}

		v = Vector2.Reflect(v, Vector2.up);

		if(v.y < this.minVerticalSpeed) {
			v.y = this.minVerticalSpeed;
		}

		actor.SetVelocity(v);
		actor.ignoreGround = true;

		Player player = actor as Player;
		if(player != null) {
			player.bounced = true;
		}
	}
}
