using System.Collections;
using UnityEngine;

public class Boss3Spinner : BossBehavior
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
        // Lotus Flower Spirals (WIDENED)
        float time = 0f;
        int petals = 4; // Reduced from 5
        float rotationSpeed = 30f; // Slower rotation
        
        while (time < attackDuration)
        {
            float baseAngle = time * rotationSpeed;
            
            for (int i = 0; i < petals; i++)
            {
                float angle = baseAngle + (i * (360f / petals));
                SpawnBullet(angle);
                
                // Reverse spiral
                SpawnBullet(-angle); 
            }
            
            yield return new WaitForSeconds(0.2f); // Slower firing
            time += 0.2f;
        }
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PatternB()
    {
        // The Pinwheel
        float time = 0f;
        int arms = 3;
        float rotationSpeed = 80f; // Fast rotation

        while (time < attackDuration)
        {
            float baseAngle = time * rotationSpeed;

            for (int i = 0; i < arms; i++)
            {
                SpawnBullet(baseAngle + (i * (360f / arms)));
            }

            yield return new WaitForSeconds(0.1f); // Fast stream, creating a solid curved arm
            time += 0.1f;
        }
        yield return new WaitForSeconds(1f);
    }
}
