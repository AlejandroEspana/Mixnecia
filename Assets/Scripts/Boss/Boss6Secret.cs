using System.Collections;
using UnityEngine;

public class Boss6Secret : BossBehavior
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
        // Mandala + Double Chaos Walls (EXTREME)
        float time = 0f;
        int mandalaPetals = 8; 
        float mandalaRotation = 0f;

        while (time < attackDuration)
        {
            // Fast Mandala
            for (int i = 0; i < mandalaPetals; i++)
            {
                float baseAngle = i * (360f / mandalaPetals) + mandalaRotation;
                float oscillate = Mathf.Sin(time * 5f) * 45f;
                SpawnBullet(baseAngle + oscillate);
            }
            mandalaRotation += 15f; 

            // Double Wall lines every 0.5s
            if (Mathf.FloorToInt(time * 10) % 5 == 0) 
            {
                int wallBullets = 10;
                float gapPos = Mathf.Sin(time * 2f) * 4f + 4f; 
                
                // Wall 1 (Left side arc)
                float wallStart1 = -70f;
                for (int i = 0; i < wallBullets; i++)
                {
                    if (Mathf.Abs(i - gapPos) > 1.5f) SpawnBullet(wallStart1 + (i * 8f));
                }

                // Wall 2 (Right side arc)
                float wallStart2 = 10f;
                for (int i = 0; i < wallBullets; i++)
                {
                    if (Mathf.Abs(i - gapPos) > 1.5f) SpawnBullet(wallStart2 + (i * 8f));
                }
            }

            yield return new WaitForSeconds(0.1f);
            time += 0.1f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternB()
    {
        // Double Counter-Rotating Pentagrams
        float time = 0f;
        float pentagramRotation1 = 0f;
        float pentagramRotation2 = 180f;

        while (time < attackDuration)
        {
            // Pentagram 1 (Clockwise)
            for (int i = 0; i < 5; i++)
            {
                SpawnBullet(pentagramRotation1 + (i * 72f));
            }
            pentagramRotation1 += 12f;

            // Pentagram 2 (Counter-Clockwise)
            for (int i = 0; i < 5; i++)
            {
                SpawnBullet(pentagramRotation2 + (i * 72f));
            }
            pentagramRotation2 -= 18f;

            // Constant Homing shot
            if (Mathf.FloorToInt(time * 10) % 3 == 0) 
            {
                Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (player != null)
                {
                    Vector3 direction = player.position - transform.position;
                    float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                    SpawnBullet(baseAngle - transform.eulerAngles.z);
                }
            }

            yield return new WaitForSeconds(0.15f);
            time += 0.15f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternC()
    {
        // Maurer Rose
        float time = 0f;
        float n = 6f; // Petals
        float d = 71f; // Angle step
        int i = 0;

        while (time < attackDuration)
        {
            // Connect points on a rose curve
            float k = i * d;
            float r = Mathf.Sin(n * k * Mathf.Deg2Rad);
            // Convert to polar angle
            float angle = k;
            
            // Spawn 2 symmetrical points to make it chaotic but beautiful
            SpawnBullet(angle);
            SpawnBullet(-angle);

            i++;
            yield return new WaitForSeconds(0.02f); // Extremely fast spawn to draw the web
            time += 0.02f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternD()
    {
        // Boomerang Fractal Arms
        float time = 0f;
        while (time < attackDuration)
        {
            int arms = 8;
            float rotationOffset = time * 20f; // Slowly rotating base

            for (int a = 0; a < arms; a++)
            {
                float armBaseAngle = (a * (360f / arms)) + rotationOffset;
                
                GameObject b = SpawnBullet(armBaseAngle);
                if (b != null)
                {
                    Bullet script = b.GetComponent<Bullet>();
                    if (script != null)
                    {
                        script.isBoomerang = true;
                        script.boomerangTime = 2.2f;
                    }
                }
            }

            yield return new WaitForSeconds(0.1f); // Rapid fire arms that all reverse
            time += 0.1f;
        }
        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator PatternE()
    {
        // Sine Web Prison (Boss 4 + 5 Combo)
        float time = 0f;
        while (time < attackDuration)
        {
            // Prison walls
            if (Mathf.FloorToInt(time * 10) % 15 == 0) // Every 1.5s
            {
                for (int i = 0; i < 36; i++)
                {
                    SpawnBullet(i * 10f);
                }
            }

            // Sine/Cosine web
            float sineAngle = Mathf.Sin(time * 3f) * 45f;
            float cosAngle = Mathf.Cos(time * 3f) * 45f;

            SpawnBullet(180f + sineAngle); // Shoot down
            SpawnBullet(180f - sineAngle); // Opposing sine
            SpawnBullet(180f + cosAngle); // Out of phase
            SpawnBullet(180f - cosAngle); 

            yield return new WaitForSeconds(0.05f);
            time += 0.05f;
        }
        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator PatternF()
    {
        // Butterfly Curve Boomerang
        float time = 0f;
        while (time < attackDuration)
        {
            int points = 45;
            for (int i = 0; i < points; i++)
            {
                float t = i * (Mathf.PI * 2f / points);
                // Math for butterfly curve
                float r = Mathf.Exp(Mathf.Sin(t)) - 2f * Mathf.Cos(4f * t) + Mathf.Pow(Mathf.Sin((2f * t - Mathf.PI) / 24f), 5f);
                float x = r * Mathf.Sin(t);
                float y = r * Mathf.Cos(t);
                
                float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

                GameObject b = SpawnBullet(angle);
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

            yield return new WaitForSeconds(2.0f);
            time += 2.0f;
        }
        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator PatternG()
    {
        // Burst Star + Pinwheel (Boss 1 + 3 combo)
        float time = 0f;
        float pinwheelAngle = 0f;
        while (time < attackDuration)
        {
            // Burst Star
            if (Mathf.FloorToInt(time * 10) % 10 == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        SpawnBullet((i * 45f) + (j * 5f));
                    }
                }
            }

            // Pinwheel constant streams
            for (int i = 0; i < 3; i++)
            {
                SpawnBullet(pinwheelAngle + (i * 120f));
            }
            pinwheelAngle += 15f;

            yield return new WaitForSeconds(0.1f);
            time += 0.1f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternH()
    {
        // Epitrochoid Swarm
        float time = 0f;
        while (time < attackDuration)
        {
            // R = 3, r = 1, d = 0.5
            float R = 3f;
            float r = 1f;
            float d = 0.5f;
            
            float x = (R + r) * Mathf.Cos(time * 4f) - d * Mathf.Cos(((R + r) / r) * time * 4f);
            float y = (R + r) * Mathf.Sin(time * 4f) - d * Mathf.Sin(((R + r) / r) * time * 4f);
            
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

            SpawnBullet(angle);
            SpawnBullet(angle + 120f);
            SpawnBullet(angle + 240f);

            // Targeted bullets
            if (Mathf.FloorToInt(time * 10) % 5 == 0)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Vector3 dir = player.transform.position - transform.position;
                    float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    targetAngle -= transform.eulerAngles.z;
                    SpawnBullet(targetAngle);
                    SpawnBullet(targetAngle - 10f);
                    SpawnBullet(targetAngle + 10f);
                }
            }

            yield return new WaitForSeconds(0.05f);
            time += 0.05f;
        }
        yield return new WaitForSeconds(1.5f);
    }
}
