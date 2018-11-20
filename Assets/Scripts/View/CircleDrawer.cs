using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CircleDrawer : MonoBehaviour {
    public static CircleDrawer instance {
        get; 
        private set;
    }

    public Shader shader;
    public Color color, color2;

    private Material mat;

    private Vector2 center;
    private float radius;
    private float radius2;
    private bool multi;

    public void Awake() {
        this.mat = new Material(this.shader);

        instance = this;
    }

    public void Draw(Vector2 center, float radius) {
        this.enabled = true;
        this.center = center;
        this.radius = radius;
    }

    public void DrawSecondary(float radius) {
        this.radius2 = radius;
        this.multi = true;
    }

    public void DontDraw() {
        this.enabled = false;
        this.multi = false;
    }

    public void DisableSecondary() {
        this.multi = false;
    }

    public void OnRenderImage(RenderTexture src, RenderTexture dest) {
        this.mat.SetColor("_Color", this.color);
        this.mat.SetVector("_CircleParams", new Vector4(this.center.x, this.center.y, this.radius, this.radius2));
        
        if(this.multi) {
            this.mat.EnableKeyword("TWO_CIRCLES");
            this.mat.SetColor("_Color2", this.color2);
        } else {
            this.mat.DisableKeyword("TWO_CIRCLES");
        }

        Graphics.Blit(src, dest, this.mat);
    }
}