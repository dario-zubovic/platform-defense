using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CircleDrawer : MonoBehaviour {
    public static CircleDrawer instance {
        get; 
        private set;
    }

    public Shader shader;
    public Color color;
    public float thickness;

    private Material mat;

    private Vector2 center;
    private float radius;

    public void Awake() {
        this.mat = new Material(this.shader);

        instance = this;
    }

    public void Draw(Vector2 center, float radius) {
        this.enabled = true;
        this.center = center;
        this.radius = radius;
    }

    public void DontDraw() {
        this.enabled = false;
    }

    public void OnRenderImage(RenderTexture src, RenderTexture dest) {
        this.mat.SetColor("_Color", this.color);
        this.mat.SetVector("_CircleParams", new Vector4(this.center.x, this.center.y, radius, radius + this.thickness));
        Graphics.Blit(src, dest, this.mat);
    }
}