using UnityEngine;

public class CircleDrawer : MonoBehaviour {
    public static CircleDrawer instance {
        get; 
        private set;
    }

    private SpriteRenderer rend;
    private Material mat;

    private Vector2 center;
    private float radius;
    private float radius2;
    private bool multi;

    public void Awake() {
        this.rend = this.gameObject.GetComponent<SpriteRenderer>();
        this.mat = this.rend.material;

        instance = this;
    }

    public void Draw(Vector2 center, float radius) {
        this.enabled = true;
        this.center = center;
        this.radius = radius;

        MoveAndResize();
        SetParams();
    }

    public void DrawSecondary(float radius) {
        this.radius2 = radius;
        this.multi = true;

        MoveAndResize();
        SetParams();
    }

    public void DontDraw() {
        this.enabled = false;
        this.multi = false;
        this.radius2 = 0;

        SetParams();
    }

    public void DisableSecondary() {
        this.multi = false;
        this.radius2 = 0;

        MoveAndResize();
        SetParams();
    }

    private void SetParams() {
        if(!this.enabled) {
            this.rend.enabled = false;
            return;
        }

        this.rend.enabled = true;

        this.mat.SetVector("_CircleParams", new Vector4(this.center.x, this.center.y, this.radius, this.radius2));
        
        if(this.multi) {
            this.mat.EnableKeyword("TWO_CIRCLES");
        } else {
            this.mat.DisableKeyword("TWO_CIRCLES");
        }
    }

    private void MoveAndResize() {
        float r = this.radius > this.radius2 ? this.radius : this.radius2;
        this.transform.localScale = new Vector3(r * 2f + 0.5f, r * 2f + 0.5f, 0);
        this.transform.position = this.center;
    }
}