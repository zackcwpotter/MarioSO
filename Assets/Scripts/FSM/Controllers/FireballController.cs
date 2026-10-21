using UnityEngine;

// Put this on the fireball prefab (needs a Rigidbody2D and a Collider2D)
public class FireballController : MonoBehaviour
{
    public float lifetime = 2f;

    private float elapsed = 0f;
    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        // Shrink over its lifetime, then disappear
        elapsed += Time.deltaTime;
        transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / lifetime);
        if (elapsed >= lifetime) Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        HitEnemy(col.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HitEnemy(other.gameObject);
    }

    private void HitEnemy(GameObject other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy == null && !other.CompareTag("Enemy")) return;

        if (enemy != null) enemy.Defeat();
        Destroy(gameObject);
    }
}
