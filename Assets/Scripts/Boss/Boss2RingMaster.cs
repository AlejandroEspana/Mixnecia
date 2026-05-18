using System.Collections;
using UnityEngine;

public class Boss2RingMaster : BossBehavior
{
    protected override IEnumerator AttackRoutine()
    {
        if (patternIndex == 0) yield return StartCoroutine(PatternA());
        else yield return StartCoroutine(PatternB());
        
        patternIndex = (patternIndex + 1) % 2;
        if (stateMachine != null) stateMachine.TransitionToState(BossState.Moving);
    }

    private IEnumerator PatternA()
    {
        // Breathing Rings (WIDENED)
        float time = 0f;
        int bulletsPerRing = 14; // Reduced from 18 for bigger gaps
        
        while (time < attackDuration)
        {
            float baseAngleOffset = Mathf.Sin(time * 1.5f) * 20f; 
            
            for (int i = 0; i < bulletsPerRing; i++)
            {
                float angle = (360f / bulletsPerRing) * i + baseAngleOffset;
                // Bigger breathing gaps
                if (Mathf.Abs(Mathf.Sin((angle + time * 40f) * Mathf.Deg2Rad)) > 0.7f) continue;

                SpawnBullet(angle);
            }
            
            yield return new WaitForSeconds(0.6f); // Slower rings
            time += 0.6f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternB()
    {
        // The Crosshair
        float time = 0f;
        float rotationAngle = 0f;

        while (time < attackDuration)
        {
            // 4 streams (Up, Down, Left, Right) that slowly rotate
            SpawnBullet(rotationAngle);
            SpawnBullet(rotationAngle + 90f);
            SpawnBullet(rotationAngle + 180f);
            SpawnBullet(rotationAngle + 270f);

            rotationAngle += 10f; // Spin rate
            
            yield return new WaitForSeconds(0.15f);
            time += 0.15f;
        }
        yield return new WaitForSeconds(1f);
    }
}
