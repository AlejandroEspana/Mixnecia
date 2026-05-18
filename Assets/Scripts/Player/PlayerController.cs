using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector2 movement;

    [Header("Combat")]
    public string bulletTag = "PlayerBullet";
    public Transform firePoint;
    public float fireRate = 0.2f;
    public int bulletDamage = 10;
    private float nextFireTime;
    private bool hasTripleShot = false;

    [Header("Precision Mode (Slow Mo)")]
    public float slowMotionScale = 0.5f;
    public float precisionSpeedMultiplier = 0.6f;
    public float maxStamina = 100f;
    public float staminaDrainRate = 33f; // ~3 seconds to drain fully
    public float staminaRechargeRate = 15f; // ~6.6 seconds to recharge fully
    
    public float currentStamina { get; private set; }
    private bool isPrecisionMode = false;
    private bool isStaminaExhausted = false;
    public bool HasPrecisionUnlocked { get; private set; } = false;

    private Rigidbody2D rb;

    public event System.Action<float, float> OnStaminaChanged;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ApplyProgressionUnlocks();
        currentStamina = maxStamina;
    }

    private void ApplyProgressionUnlocks()
    {
        HasPrecisionUnlocked = false;
        maxStamina = 100f;
        fireRate = 0.2f;
        hasTripleShot = false;
        bulletDamage = 10;

        if (SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
        {
            bool[] completed = SaveManager.Instance.CurrentSaveData.completedLevels;
            
            // Level 1 (Boss 1) Beaten
            if (completed.Length > 0 && completed[0]) HasPrecisionUnlocked = true;
            // Level 2 (Boss 2) Beaten
            if (completed.Length > 1 && completed[1]) maxStamina = 150f;
            // Level 3 (Boss 3) Beaten
            if (completed.Length > 2 && completed[2]) fireRate = 0.12f;
            // Level 4 (Boss 4) Beaten
            if (completed.Length > 3 && completed[3]) hasTripleShot = true;
            // Level 5 (Boss 5) Beaten
            if (completed.Length > 4 && completed[4]) bulletDamage = 20;
        }
    }

    private void Update()
    {
        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Shooting
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        HandlePrecisionMode();
    }

    private void HandlePrecisionMode()
    {
        if (!HasPrecisionUnlocked) return; // Feature locked

        bool holdingKey = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (!holdingKey)
        {
            isStaminaExhausted = false; // Reset exhaust state when key is released
        }

        if (holdingKey && currentStamina > 0 && !isStaminaExhausted)
        {
            if (!isPrecisionMode)
            {
                isPrecisionMode = true;
                Time.timeScale = slowMotionScale;
                Time.fixedDeltaTime = 0.02f * Time.timeScale; // Keep physics smooth
            }

            currentStamina -= staminaDrainRate * Time.unscaledDeltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isStaminaExhausted = true;
                ExitPrecisionMode();
            }
        }
        else
        {
            if (isPrecisionMode)
            {
                ExitPrecisionMode();
            }

            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRechargeRate * Time.unscaledDeltaTime;
                if (currentStamina > maxStamina) currentStamina = maxStamina;
            }
        }

        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    private void ExitPrecisionMode()
    {
        isPrecisionMode = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    private void FixedUpdate()
    {
        // Movement
        float currentSpeed = isPrecisionMode ? (moveSpeed * precisionSpeedMultiplier) : moveSpeed;
        Vector3 targetPos = transform.position + new Vector3(movement.x, movement.y, 0f).normalized * currentSpeed * Time.fixedDeltaTime;
        
        // Simple Screen Clamping
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(targetPos);
        viewportPos.x = Mathf.Clamp(viewportPos.x, 0.05f, 0.95f);
        viewportPos.y = Mathf.Clamp(viewportPos.y, 0.05f, 0.95f);
        targetPos = Camera.main.ViewportToWorldPoint(viewportPos);

        if (rb != null)
        {
            rb.MovePosition(targetPos);
        }
        else
        {
            transform.position = targetPos;
        }
    }

    private void Shoot()
    {
        if (ObjectPooler.Instance != null && firePoint != null)
        {
            if (hasTripleShot)
            {
                SpawnPlayerBullet(firePoint.position);
                SpawnPlayerBullet(firePoint.position + Vector3.left * 0.4f);
                SpawnPlayerBullet(firePoint.position + Vector3.right * 0.4f);
            }
            else
            {
                SpawnPlayerBullet(firePoint.position);
            }
        }
    }

    private void SpawnPlayerBullet(Vector3 spawnPos)
    {
        GameObject bulletObj = ObjectPooler.Instance.SpawnFromPool(bulletTag, spawnPos, firePoint.rotation);
        if (bulletObj != null)
        {
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = bulletDamage;
            }
        }
    }

    private void OnDestroy()
    {
        // Reset time scale if the player is destroyed (dies or level changes)
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
