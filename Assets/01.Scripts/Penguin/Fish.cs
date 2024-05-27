using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private float fishSpeed;
    private float nextActionTime = -1f;
    private Vector3 targetPosition;

    private void FixedUpdate()
    {
        Swim();
    }

    private void Swim()
    {
        if (Time.fixedTime >= nextActionTime)
        {
            fishSpeed = Random.Range(0.1f, 0.8f);
            targetPosition = PenguinArea.ChooseRandomPosition(transform.parent.position, 100f, 260f, 2f, 13f);
            transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);

            float timeToTarget = Vector3.Distance(transform.position, targetPosition) / fishSpeed;
            nextActionTime = Time.fixedTime + timeToTarget;

        }
        else
        {
            transform.position += transform.forward * Time.fixedDeltaTime * fishSpeed;
        }
    }
}
