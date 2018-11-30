using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Projectile : MonoBehaviour {
    public Rigidbody2D rigid;
    public LayerMask enemyLayers;
    public SoundId sfxOnHit;

    public Color particlesColor;

    public float blastRadius {
        get;
        set;
    }

    public Turret owner {
        get;
        set;
    }

    private bool exploaded;

    public void OnEnable() {
        this.exploaded = false;
    }
    
    public void Update() {
        Vector2 vel = this.rigid.velocity;
        float angle = Mathf.Atan2(vel.y, vel.x);
        this.transform.localEulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
    }

    public void OnCollisionEnter2D(Collision2D coll) {
        if(this.exploaded) {
            return;
        }
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, this.blastRadius, this.enemyLayers);
        
        for(int i = 0; i < colliders.Length; i++) {
            AffectEnemy(colliders[i].gameObject.GetComponent<Enemy>(), Vector2.Distance(this.transform.position, colliders[i].gameObject.transform.position) / this.blastRadius);
        }

        TurretParticles.instance.EmitCircle(this.transform.position, this.blastRadius, this.particlesColor);
        SoundManager.instance.PlayAtPosition(this.sfxOnHit, this.transform.position);

        this.exploaded = true;
        Pool.instance.Return(this.gameObject);
    }

    protected abstract void AffectEnemy(Enemy enemy, float dist);
}