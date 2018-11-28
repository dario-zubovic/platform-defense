using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Projectile : MonoBehaviour {
    public Rigidbody2D rigid;
    public LayerMask enemyLayers;

    public float blastRadius {
        get;
        set;
    }

    private bool exploaded;
    
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

        this.exploaded = true;
        GameObject.Destroy(this.gameObject); // TODO: pool
    }

    protected abstract void AffectEnemy(Enemy enemy, float dist);
}