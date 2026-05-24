using System.Collections;
using UnityEngine;

public class Boss5Final : BossBehavior
{
    protected override IEnumerator AttackRoutine()
    {
        if (patternIndex == 0) yield return StartCoroutine(PatternA());
        else if (patternIndex == 1) yield return StartCoroutine(PatternB());
        else if (patternIndex == 2) yield return StartCoroutine(PatternC());
        else if (patternIndex == 3) yield return StartCoroutine(PatternD());
        else if (patternIndex == 4) yield return StartCoroutine(PatternE());
        else if (patternIndex == 5) yield return StartCoroutine(PatternF());
        else if (patternIndex == 6) yield return StartCoroutine(PatternG());
        else yield return StartCoroutine(PatternH());
        
        patternIndex = (patternIndex + 1) % 8;
        if (stateMachine != null) stateMachine.TransitionToState(BossState.Moving);
    }

    private IEnumerator PatternA()
    {
        // Golden Fibonacci Spiral (DOUBLE EXTREME)
        float time = 0f;
        float goldenAngle = 137.5f;
        float currentAngle = 0f;
        
        while (time < attackDuration)
        {
            // Forward spiral
            SpawnBullet(currentAngle);
            SpawnBullet(currentAngle + 180f); // Double dense

            currentAngle += goldenAngle;
            
            // Fast shotgun blast every 1.5 seconds (5-way spread)
            if (time > 0f && Mathf.FloorToInt(time * 7) % 15 == 0)
            {
                Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (player != null)
                {
                    Vector3 direction = player.position - transform.position;
                    float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                    baseAngle -= (transform.eulerAngles.z + 180f);

                    for (int i = -2; i <= 2; i++)
                    {
                        SpawnBullet(baseAngle + (i * 13f));
                    }
                }
            }

            yield return new WaitForSeconds(0.06f); // Extreme spawn rate
            time += 0.06f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternB()
    {
        // The Prison (Triple Concentric)
        float time = 0f;

        while (time < attackDuration)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player != null)
            {
                Vector3 direction = player.position - transform.position;
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                targetAngle -= (transform.eulerAngles.z + 180f);

                int circleBullets = 30; // Denser circle

                // Spawn 3 circles with small delay between them, each with a different gap
                for (int c = 0; c < 3; c++)
                {
                    int gapIndex = Random.Range(0, circleBullets);
                    for (int i = 0; i < circleBullets; i++)
                    {
                        if (Mathf.Abs(i - gapIndex) <= 1 || Mathf.Abs((i - circleBullets) - gapIndex) <= 1) continue; 
                        
                        float angle = targetAngle + (i * (360f / circleBullets));
                        SpawnBullet(angle);
                    }
                    yield return new WaitForSeconds(0.4f); // Delay between concentric rings
                    time += 0.4f;
                }
            }

            yield return new WaitForSeconds(1.0f); 
            time += 1.0f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternC()
    {
        // Fermat's Spiral (sqrt(t))
        float time = 0f;
        float theta = 0f;
        while (time < attackDuration)
        {
            float angle = theta * Mathf.Rad2Deg;
            
            SpawnBullet(angle);
            SpawnBullet(angle + 180f); // Opposite arm
            
            theta += 0.5f / Mathf.Sqrt(theta + 1f); // Fermat curve approximation for angle step

            yield return new WaitForSeconds(0.04f);
            time += 0.04f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternD()
    {
        // Boomerang Supernova
        float time = 0f;
        while (time < attackDuration)
        {
            int ringBullets = 45; // Super dense ring
            for (int i = 0; i < ringBullets; i++)
            {
                float angle = i * (360f / ringBullets);
                GameObject b = SpawnBullet(angle);
                if (b != null)
                {
                    Bullet script = b.GetComponent<Bullet>();
                    if (script != null)
                    {
                        script.isBoomerang = true;
                        script.boomerangTime = 4.5f; // Expands huge, then crushes inward
                    }
                }
            }

            // Homing shot to prevent standing still
            Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player != null)
            {
                Vector3 direction = player.position - transform.position;
                float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                SpawnBullet(baseAngle - (transform.eulerAngles.z + 180f));
            }

            yield return new WaitForSeconds(2.5f);
            time += 2.5f;
        }
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator PatternE()
    {
        // Lotus Boomerang (Boss 3 Combo)
        float time = 0f;
        float lotusAngle = 0f;
        while (time < attackDuration)
        {
            int petals = 4;
            for (int i = 0; i < petals; i++)
            {
                float baseAngle = lotusAngle + (i * 90f);
                for (int j = -2; j <= 2; j++)
                {
                    GameObject b = SpawnBullet(baseAngle + (j * 15f));
                    if (b != null)
                    {
                        Bullet script = b.GetComponent<Bullet>();
                        if (script != null)
                        {
                            script.isBoomerang = true;
                            script.boomerangTime = 3.5f;
                        }
                    }
                }
            }
            lotusAngle += 20f;
            yield return new WaitForSeconds(0.4f);
            time += 0.4f;
        }
        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator PatternF()
    {
        // Crosshair Rain (Boss 2 + 4 Combo)
        float time = 0f;
        float crosshairRotation = 0f;
        while (time < attackDuration)
        {
            // Spinning Crosshair
            for (int i = 0; i < 4; i++)
            {
                SpawnBullet(crosshairRotation + (i * 90f));
            }
            crosshairRotation += 8f;

            // Diagonal Rain
            if (Mathf.FloorToInt(time * 10) % 5 == 0) // Every 0.5s
            {
                int bulletsInWall = 14;
                float startAngle = -65f;
                int safeZone = Random.Range(2, bulletsInWall - 2); 
                for (int i = 0; i < bulletsInWall; i++)
                {
                    if (Mathf.Abs(i - safeZone) <= 1) continue;
                    SpawnBullet(startAngle + (i * 10f));
                }
            }

            yield return new WaitForSeconds(0.08f);
            time += 0.08f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternG()
    {
        // Cardioid Heart Boomerang
        float time = 0f;
        while (time < attackDuration)
        {
            int heartPoints = 30;
            for (int i = 0; i < heartPoints; i++)
            {
                float t = i * (Mathf.PI * 2f / heartPoints);
                float x = 16f * Mathf.Pow(Mathf.Sin(t), 3f);
                float y = 13f * Mathf.Cos(t) - 5f * Mathf.Cos(2f * t) - 2f * Mathf.Cos(3f * t) - Mathf.Cos(4f * t);
                
                float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

                GameObject b = SpawnBullet(angle);
                if (b != null)
                {
                    Bullet script = b.GetComponent<Bullet>();
                    if (script != null)
                    {
                        script.isBoomerang = true;
                        script.boomerangTime = 2.5f; // Short heartbeat
                    }
                }
            }

            yield return new WaitForSeconds(1.8f);
            time += 1.8f;
        }
        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator PatternH()
    {
        // Spirograph Rings
        float time = 0f;
        while (time < attackDuration)
        {
            // R = 5, r = 3, d = 5 (Hypotrochoid)
            float R = 5f;
            float r = 3f;
            float d = 5f;
            
            float x = (R - r) * Mathf.Cos(time * 5f) + d * Mathf.Cos(((R - r) / r) * time * 5f);
            float y = (R - r) * Mathf.Sin(time * 5f) - d * Mathf.Sin(((R - r) / r) * time * 5f);
            
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

            SpawnBullet(angle);
            SpawnBullet(angle + 180f);

            yield return new WaitForSeconds(0.03f); // Insanely fast
            time += 0.03f;
        }
        yield return new WaitForSeconds(1f);
    }
}
