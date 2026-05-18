using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float playerSpeed = 15f;
    public float enemySpeed = 4f;
    public int damage = 1;
    public bool isPlayerBullet;

    [Header("Boomerang Settings")]
    public bool isBoomerang = false;
    public float boomerangTime = 2f;
    private float aliveTimer = 0f;
    private bool hasReversed = false;
    private float currentSpeed;

    private void OnEnable()
    {
        aliveTimer = 0f;
        hasReversed = false;
        isBoomerang = false;
        currentSpeed = isPlayerBullet ? playerSpeed : enemySpeed;
    }

    private void Update()
    {
        if (isBoomerang)
        {
            aliveTimer += Time.deltaTime;
            if (aliveTimer > boomerangTime && !hasReversed)
            {
                hasReversed = true;
                currentSpeed = -currentSpeed; // Reverse trajectory
            }
        }

        transform.Translate(Vector3.up * currentSpeed * Time.deltaTime);

        // Despawn off-screen (resolution independent bounds)
        // Expanded bounds slightly to allow boomerangs to return if just off-screen
        if (Camera.main != null)
        {
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewportPos.x < -0.4f || viewportPos.x > 1.4f ||
                viewportPos.y < -0.4f || viewportPos.y > 1.4f)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayerBullet && collision.CompareTag("Enemy"))
        {
            // Send damage to enemy
            var bossHealth = collision.GetComponent<BossHealth>();
            if (bossHealth != null) bossHealth.TakeDamage(damage);
            
            if (GameManager.Instance != null) GameManager.Instance.AddComboHit();

            gameObject.SetActive(false);
        }
        else if (!isPlayerBullet && collision.CompareTag("Player"))
        {
            // Send damage to player
            var playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(damage);
            gameObject.SetActive(false);
        }
        else if (collision.tag == "Obstacle" || collision.tag == "Barrier")
        {
            // Destroy bullet on hitting a wall or barrier (for hiding mechanics)
            gameObject.SetActive(false);
        }
    }
}
