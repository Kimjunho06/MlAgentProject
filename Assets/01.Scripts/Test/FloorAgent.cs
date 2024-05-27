using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class FloorAgent : Agent
{
    public Transform ballTransform;
    private Rigidbody ballRigidbody;

    // ȯ���� ó�� ���۵� �� �� ���� ����Ǵ� �ʱ�ȭ �Լ� (Start)
    public override void Initialize()
    {
        ballRigidbody = ballTransform.GetComponent<Rigidbody>();
    }

    // �� ���Ǽҵ尡 ���۵� �� ȣ��Ǵ� �Լ� => ȯ���� ���¸� �ʱ�ȭ�ϴ� ����
    public override void OnEpisodeBegin()
    {
        // Floor x, Floor z�� �������� �������ϰ� ȸ��.
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));

        // Ball : velocity�� ��ġ�� �ʱ�ȭ.
        ballRigidbody.velocity = Vector3.zero;
        ballTransform.localPosition = new Vector3(Random.Range(-1.5f, 1.5f), 1.5f, Random.Range(-1.5f, 1.5f));
    }

    // Agent�� ������ �����ϴ� �Լ� => vector Observation(���� ����)
    public override void CollectObservations(VectorSensor sensor)
    {
        // �н��� �̻��ϴٸ� �� �κп��� ����� �� ���� �н���Ű���� Ȯ���ؾߵ�
        // Agent�� ������ �����ϴ� ���� ������ 8�� => Parameter Space Size ������ߵ�
        sensor.AddObservation(transform.rotation.x);
        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(ballRigidbody.velocity);
        sensor.AddObservation(ballTransform.position - transform.position);
    }

    // Agent�� �ൿ�� �����ϴ� �Լ�
    public override void OnActionReceived(ActionBuffers actions)
    {
        // parameter Continuous action ���� ���߱�, Discrete Branch 0 �����
        // continuous Action 2��
        var ContinuousActions = actions.ContinuousActions;
        float z_rotation = Mathf.Clamp(ContinuousActions[0], -1f, 1f);
        float x_rotation = Mathf.Clamp(ContinuousActions[1], -1f, 1f);

        transform.Rotate(new Vector3(0, 0, 1), z_rotation);
        transform.Rotate(new Vector3(1, 0, 0), x_rotation);

        if (ballTransform.position.y - transform.position.y < -2f)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else if (Mathf.Abs(ballTransform.position.x - transform.position.x) > 2.5f)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else if (Mathf.Abs(ballTransform.position.z - transform.position.z) > 2.5f)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            SetReward(0.1f);
        }
    }

    // ����� ������Ʈ�� �������� �����ϴ� ����� �����ϴ� �Լ�
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // ����Ű�� �Է��� ���� �ൿ���� �����Ѵ�.
        var ContinuousActionsOut = actionsOut.ContinuousActions;
        ContinuousActionsOut[0] = -Input.GetAxis("Horizontal");
        ContinuousActionsOut[1] = Input.GetAxis("Vertical");

    }
}
