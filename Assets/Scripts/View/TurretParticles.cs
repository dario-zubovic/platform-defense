using UnityEngine;

public class TurretParticles : MonoBehaviour {
    public float lineCountPerUnit;
    public float circleCountPerUnit;

    public static TurretParticles instance {
        get;
        private set;
    }

    private ParticleSystem particles;
    private ParticleSystem.EmitParams p;
	private ParticleSystem.ShapeModule shape;

    public void Awake() {
        TurretParticles.instance = this;

        this.particles = this.gameObject.GetComponent<ParticleSystem>();
        this.shape = this.particles.shape;

        this.p = new ParticleSystem.EmitParams();
        this.p.applyShapeToPosition = true;
    }

    public void EmitLine(Vector2 start, Vector2 end, Color color) {
        Vector2 midPoint = Vector2.Lerp(start, end, 0.5f);
        Vector2 delta = end - start;
        float dist = Vector2.Distance(start, end);

        if(this.shape.shapeType != ParticleSystemShapeType.SingleSidedEdge) {
            this.shape.shapeType = ParticleSystemShapeType.SingleSidedEdge;
        }
        this.shape.position = new Vector3(midPoint.x, 0, midPoint.y);
        this.shape.rotation = new Vector3(0, -1f * Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg, 0);
        this.shape.radius = dist * 0.5f;

        this.p.startColor = color;

        this.particles.Emit(this.p, Mathf.RoundToInt(dist * this.lineCountPerUnit));
    }

    public void EmitCircle(Vector2 pos, float radius, Color color) {
        if(this.shape.shapeType != ParticleSystemShapeType.Circle) {
            this.shape.shapeType = ParticleSystemShapeType.Circle;
        }
        this.shape.position = new Vector3(pos.x, 0, pos.y);
        this.shape.rotation = new Vector3(90, 0, 0);
        this.shape.radius = radius;

        this.p.startColor = color;

        this.particles.Emit(this.p, Mathf.RoundToInt(radius * radius * Mathf.PI * this.circleCountPerUnit));
    }
}