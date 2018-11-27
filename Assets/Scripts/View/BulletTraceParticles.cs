using UnityEngine;

public class BulletTraceParticles : MonoBehaviour {
    public int countPerUnit; 

    public static BulletTraceParticles instance {
        get;
        private set;
    }

    private ParticleSystem particles;
    private ParticleSystem.EmitParams p;
	private ParticleSystem.ShapeModule shape;

    public void Awake() {
        BulletTraceParticles.instance = this;

        this.particles = this.gameObject.GetComponent<ParticleSystem>();
        this.shape = this.particles.shape;

        this.p = new ParticleSystem.EmitParams();
        this.p.applyShapeToPosition = true;
    }

    public void Emit(Vector2 start, Vector2 end) {
        Vector2 midPoint = Vector2.Lerp(start, end, 0.5f);
        Vector2 delta = end - start;
        float dist = Vector2.Distance(start, end);

        this.shape.position = new Vector3(midPoint.x, 0, midPoint.y);
        this.shape.rotation = new Vector3(0, -1f * Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg, 0);
        this.shape.radius = dist * 0.5f;

        this.particles.Emit(this.p, Mathf.RoundToInt(dist * this.countPerUnit));
    }
}