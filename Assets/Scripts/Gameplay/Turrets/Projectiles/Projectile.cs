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