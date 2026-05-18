using System.Collections;
using UnityEngine;

public class Boss1Tutorial : BossBehavior
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
        // Smooth sine wave sweeping (WIDENED GAPS)
        float time = 0f;
        while (time < attackDuration)
        {
            float sweepAngle = Mathf.Sin(time * 2f) * 70f; // Slower sweep, wider angle

            // 3 bullets in a trident that waves (wider spacing)
            SpawnBullet(sweepAngle);
            SpawnBullet(sweepAngle - 35f);
            SpawnBullet(sweepAngle + 35f);

            yield return new WaitForSeconds(0.3f); // Much slower fire rate for more escape space
            time += 0.3f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternB()
    {
        // Burst Star
        float time = 0f;
        while (time < attackDuration)
        {
            int starPoints = 8;
            float angleStep = 360f / starPoints;

            for (int i = 0; i < starPoints; i++)
            {
                SpawnBullet(i * angleStep);
            }

            yield return new WaitForSeconds(1f); // Extremely slow 8-way bursts
            time += 1f;
        }
        yield return new WaitForSeconds(1f);
    }
}
