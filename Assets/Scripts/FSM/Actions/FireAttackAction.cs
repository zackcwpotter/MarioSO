using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/FireAttack")]
public class FireAttackAction : Action
{
    public GameObject attackPrefab;
    public AudioClip fireSound;
    public int maxPrefabInScene = 3;
    public float impulseForce = 5f;
    public float degree = 45f;
    public Vector2 spawnOffset = new Vector2(0.5f, 0.3f);

    public override void Act(StateController controller)
    {
        MarioStateController m = controller as MarioStateController;
        if (m == null || attackPrefab == null) return;

        if (FindObjectsByType<FireballController>(FindObjectsSortMode.None).Length >= maxPrefabInScene) return;

        bool faceRight = !m.spriteRenderer.flipX;
        float side = faceRight ? 1f : -1f;
        Vector3 spawnPosition = m.transform.position + new Vector3(spawnOffset.x * side, spawnOffset.y, 0);

        GameObject fireball = Instantiate(attackPrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D body = fireball.GetComponent<Rigidbody2D>();
        if (body != null) body.AddForce(CalculateDirection(faceRight) * impulseForce, ForceMode2D.Impulse);

        if (fireSound != null && m.player.marioAudio != null)
            m.player.marioAudio.PlayOneShot(fireSound);
    }

    private Vector2 CalculateDirection(bool faceRight)
    {
        float radians = degree * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        if (!faceRight) direction.x = -direction.x;
        return direction;
    }
}
