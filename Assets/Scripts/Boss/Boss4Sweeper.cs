using System.Collections;
using UnityEngine;

public class Boss4Sweeper : BossBehavior
{
    protected override IEnumerator AttackRoutine()
    {
        if (patternIndex == 0) yield return StartCoroutine(PatternA());
        else if (patternIndex == 1) yield return StartCoroutine(PatternB());
        else if (patternIndex == 2) yield return StartCoroutine(PatternC());
        else if (patternIndex == 3) yield return StartCoroutine(PatternD());
        else if (patternIndex == 4) yield return StartCoroutine(PatternE());
        else yield return StartCoroutine(PatternF());
        
        patternIndex = (patternIndex + 1) % 6;
        if (stateMachine != null) stateMachine.TransitionToState(BossState.Moving);
    }

    private IEnumerator PatternA()
    {
        // Double Sine Wave Web (EXTREME)
        float time = 0f;
        float waveSpeed = 150f; // Faster wave sweep
        
        while (time < attackDuration)
        {
            float angle1 = Mathf.Sin(time * 2f) * waveSpeed;
            float angle2 = Mathf.Cos(time * 2f) * waveSpeed;

            SpawnBullet(angle1);
            SpawnBullet(angle2);
            
            // Aimed burst every 1.2 seconds (3-way spread)
            if (time > 0f && Mathf.FloorToInt(time * 10) % 12 == 0)
            {
                Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (player != null)
                {
                    Vector3 direction = player.position - transform.position;
                    float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                    baseAngle -= (transform.eulerAngles.z + 180f);
                    
                    SpawnBullet(baseAngle);
                    SpawnBullet(baseAngle - 10f);
                    SpawnBullet(baseAngle + 10f);
                }
            }

            yield return new WaitForSeconds(0.08f); // Very fast spawn rate
            time += 0.08f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternB()
    {
        // Acid Rain (Diagonal & Fast)
        float time = 0f;

        while (time < attackDuration)
        {
            int bulletsInWall = 14;
            float startAngle = -65f; // Spread
            int safeZone = Random.Range(2, bulletsInWall - 2); 

            for (int i = 0; i < bulletsInWall; i++)
            {
                if (Mathf.Abs(i - safeZone) <= 1) continue; // Gap of 3 bullets

                SpawnBullet(startAngle + (i * 10f));
            }

            yield return new WaitForSeconds(0.4f); // Rains twice as fast
            time += 0.4f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternC()
    {
        // Lissajous Math Flower
        float time = 0f;
        while (time < attackDuration)
        {
            float a = 3f;
            float b = 2f;
            float x = Mathf.Sin(a * time * 2f);
            float y = Mathf.Sin(b * time * 2f);
            
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

            // Spawn 3 interlocking lissajous streams
            SpawnBullet(angle);
            SpawnBullet(angle + 120f);
            SpawnBullet(angle + 240f);

            yield return new WaitForSeconds(0.05f); // Very fast continuous stream
            time += 0.05f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternD()
    {
        // Boomerang Rose Curve
        float time = 0f;
        float baseRotation = 0f;
        while (time < attackDuration)
        {
            int petals = 8;
            int bulletsPerPetal = 12;
            
            for (int p = 0; p < petals; p++)
            {
                float petalBaseAngle = p * (360f / petals) + baseRotation;
                for (int i = 0; i < bulletsPerPetal; i++)
                {
                    float spread = -10f + (i * (20f / bulletsPerPetal));
                    float finalAngle = petalBaseAngle + spread;
                    
                    GameObject b = SpawnBullet(finalAngle);
                    if (b != null)
                    {
                        Bullet script = b.GetComponent<Bullet>();
                        if (script != null)
                        {
                            script.isBoomerang = true;
                            script.boomerangTime = 2.8f;
                        }
                    }
                }
            }

            baseRotation += 25f; // Rotate slowly
            yield return new WaitForSeconds(2.0f);
            time += 2.0f;
        }
        yield return new WaitForSeconds(2.8f);
    }

    private IEnumerator PatternE()
    {
        // Laser Mesh (Wall-Sweep)
        float time = 0f;
        float sweepAngle = -45f;
        bool sweepRight = true;
        
        while (time < attackDuration)
        {
            // Thick line
            for (int i = 0; i < 3; i++)
            {
                SpawnBullet(sweepAngle + (i * 2f));
            }
            
            if (sweepRight)
            {
                sweepAngle += 15f;
                if (sweepAngle > 45f) sweepRight = false;
            }
            else
            {
                sweepAngle -= 15f;
                if (sweepAngle < -45f) sweepRight = true;
            }

            // Mesh scattering
            if (Mathf.FloorToInt(time * 8) % 2 == 0)
            {
                SpawnBullet(Random.Range(-100f, 100f));
            }

            yield return new WaitForSeconds(0.1f);
            time += 0.1f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternF()
    {
        // Double Helix Boomerang
        float time = 0f;
        while (time < attackDuration)
        {
            float sineAngle = Mathf.Sin(time * 5f) * 60f;
            
            GameObject b1 = SpawnBullet(sineAngle);
            GameObject b2 = SpawnBullet(-sineAngle);

            if (b1 != null && b2 != null)
            {
                Bullet s1 = b1.GetComponent<Bullet>();
                Bullet s2 = b2.GetComponent<Bullet>();
                if (s1 != null) { s1.isBoomerang = true; s1.boomerangTime = 2.5f; }
                if (s2 != null) { s2.isBoomerang = true; s2.boomerangTime = 2.5f; }
            }

            yield return new WaitForSeconds(0.08f);
            time += 0.08f;
        }
        yield return new WaitForSeconds(2.5f);
    }
}
