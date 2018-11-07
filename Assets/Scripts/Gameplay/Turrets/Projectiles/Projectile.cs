using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {
    public Rigidbody2D rigid;
    public LayerMask enemyLayers;

    public float blastRadius {
        get;
        set;
    }
    
    public float minDamage {
        get;
        set;
    }
    public float maxDamage {
        get;
        set;
    }

    private bool exploaded;

    public void Awake() {

    }

    public void OnCollisionEnter2D(Collision2D coll) {
        if(this.exploaded) {
            return;
        }
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, this.blastRadius, this.enemyLayers);
        
        for(int i = 0; i < colliders.Length; i++) {
            colliders[i].gameObject.GetComponent<Enemy>().TakeDamage(
                Mathf.Lerp(this.maxDamage, this.minDamage, Vector2.Distance(this.transform.position, colliders[i].transform.position) / this.blastRadius)
            );
        }

        this.exploaded = true;
        GameObject.Destroy(this.gameObject); // TODO: pool
    }
}