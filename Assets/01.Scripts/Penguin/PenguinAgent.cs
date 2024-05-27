using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PenguinAgent : Agent
{
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;
    public GameObject heartPrefab;
    public GameObject regurgitateFishPrefab;

    private PenguinArea penguinArea;
    private new Rigidbody rigidbody;
    private GameObject babyPenguin;
    private bool isFull;


    public override void Initialize()
    {
        penguinArea = transform.parent.Find("PenguinArea").GetComponent<PenguinArea>();
        babyPenguin = penguinArea.penguinBaby;
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        isFull = false;
        penguinArea.ResetArea();
    }

    public override void CollectObservations(VectorSensor sensor)
    {


        sensor.AddObservation(isFull);
        sensor.AddObservation(Vector3.Distance(transform.position, babyPenguin.transform.position));
        sensor.AddObservation((babyPenguin.transform.position - transform.position).normalized);
        sensor.AddObservation(transform.forward);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var DiscreteActions = actions.DiscreteActions;

        int forwardAmount = DiscreteActions[0];

        int turnAmount = 0;
        if (DiscreteActions[1] == 1)
        {
            turnAmount = -1;
        }
        else if (DiscreteActions[1] == 2)
        {
            turnAmount = 1;
        }

        rigidbody.MovePosition(transform.position + transform.forward * forwardAmount * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(Vector3.up * turnAmount * turnSpeed * Time.fixedDeltaTime);

        AddReward(-1f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var DiscreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.W))
        {
            DiscreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A)) 
        {
            DiscreteActionsOut[1] = 1;
        }
        else if (Input.GetKey(KeyCode.D)) 
        {
            DiscreteActionsOut[1] = 2;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("FISH"))
        {
            EatFish(collision.gameObject);
        }
        else if (collision.transform.CompareTag("BABY_PENGUIN"))
        {
            RegurgitateFish();
        }
    }

    private void EatFish(GameObject fishObject)
    {
        if (isFull) return;
        isFull = true;

        penguinArea.RemoveFishInList(fishObject);
        AddReward(1f);
    }

    private void RegurgitateFish()
    {
        if (!isFull) return;
        isFull = false;

        GameObject regurgitatedFish = Instantiate(regurgitateFishPrefab);
        regurgitatedFish.transform.SetParent(transform.parent);
        regurgitatedFish.transform.localPosition = babyPenguin.transform.localPosition + Vector3.up * 0.01f;
        Destroy(regurgitatedFish, 4f);

        GameObject heart = Instantiate(heartPrefab);
        heart.transform.SetParent(transform.parent);
        heart.transform.localPosition = babyPenguin.transform.localPosition + Vector3.up;
        Destroy(heart, 4f);

        AddReward(1f);

        if (penguinArea.remainingFish <= 0)
        {
            EndEpisode();
        }
    }

}
