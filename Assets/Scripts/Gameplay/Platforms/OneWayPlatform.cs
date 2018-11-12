using UnityEngine;

public class OneWayPlatform : Platform {
    public override PlatformType type {
        get {
            return PlatformType.OneWay;
        }
    }

    public bool skip {
        get;
        private set;
    }

    public override void Contact(Actor actor, RaycastHit2D hit, bool vertical) {
        if(!vertical) {
            skip = true;
            return;
        }

        if(hit.normal.y < 0.85f) {
            skip = true;
            return;
        }

        if(actor.GetVelocity().y > 0f) {
            skip = true;
            return;
        }

        if((hit.centroid - hit.point).y < 1.3f) {
            skip = true;
            return;
        }
        
        skip = false;
    }
}