using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MummyILAgent : Agent
{
    public float moveSpeed = 10f, turnSpeed = 50f;

    private ColorTargeter colorTargeter;
    private new Rigidbody rigidbody;
    private Vector3 originPosition;

    public Material goodMaterial, badMaterial;
    private Material originMaterial;
    private Renderer floorRenderer;

    public override void Initialize()
    {
        colorTargeter = transform.parent.GetComponent<ColorTargeter>();
        rigidbody = GetComponent<Rigidbody>();
        originPosition = transform.localPosition;
        floorRenderer = transform.parent.Find("Floor").GetComponent<Renderer>();
        originMaterial = floorRenderer.material;
    }

    public override void OnEpisodeBegin()
    {
        colorTargeter.TargetingColor();

        rigidbody.velocity = rigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = originPosition;
        transform.localRotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var DiscreteActions = actions.DiscreteActions;

        Vector3 direction = Vector3.zero;
        Vector3 rotationAxis = Vector3.zero;

        // DiscreteAction[0] : 정지(0), 전진(1), 후진(2)
        switch (DiscreteActions[0])
        {
            case 1: direction = transform.forward; break;
            case 2: direction = -transform.forward; break;
        }
        
        // DiscreteAction[1] : 정지(0), 좌회전(1), 우회전(2)
        switch (DiscreteActions[1])
        {
            case 1: rotationAxis = Vector3.down; break;
            case 2: rotationAxis = -Vector3.down; break;
        }

        rigidbody.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        transform.Rotate(rotationAxis, turnSpeed * Time.fixedDeltaTime);

        AddReward(-0.01f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var DiscreteActionsOut = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W))
        {
            DiscreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            DiscreteActionsOut[0] = 2;
        }
        if (Input.GetKey(KeyCode.A))
        {
            DiscreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            DiscreteActionsOut[1] = 2;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(colorTargeter.targetColor.ToString()))
        {
            StartCoroutine(ChangeFloorColor(goodMaterial));
            AddReward(1f);
            EndEpisode();
        }
        else if (collision.collider.CompareTag("WALL"))
        {
            StartCoroutine(ChangeFloorColor(badMaterial));
            AddReward(-1f);
            EndEpisode();
        }
        else if (collision.collider.CompareTag("HINT"))
        {

        }
    }

    private IEnumerator ChangeFloorColor(Material changeMaterial)
    {
        floorRenderer.material = changeMaterial;
        yield return new WaitForSeconds(0.2f);
        floorRenderer.material = originMaterial;

    }
}
