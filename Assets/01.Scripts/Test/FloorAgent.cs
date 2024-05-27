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

    // 환경이 처음 시작될 때 한 번만 실행되는 초기화 함수 (Start)
    public override void Initialize()
    {
        ballRigidbody = ballTransform.GetComponent<Rigidbody>();
    }

    // 각 에피소드가 시작될 때 호출되는 함수 => 환경의 상태를 초기화하는 역할
    public override void OnEpisodeBegin()
    {
        // Floor x, Floor z축 기준으로 무작위하게 회전.
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));

        // Ball : velocity와 위치를 초기화.
        ballRigidbody.velocity = Vector3.zero;
        ballTransform.localPosition = new Vector3(Random.Range(-1.5f, 1.5f), 1.5f, Random.Range(-1.5f, 1.5f));
    }

    // Agent의 관측을 설정하는 함수 => vector Observation(벡터 관측)
    public override void CollectObservations(VectorSensor sensor)
    {
        // 학습이 이상하다면 이 부분에서 제대로 된 것을 학습시키는지 확인해야됨
        // Agent의 센서가 관측하는 값의 개수는 8개 => Parameter Space Size 맞춰줘야됨
        sensor.AddObservation(transform.rotation.x);
        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(ballRigidbody.velocity);
        sensor.AddObservation(ballTransform.position - transform.position);
    }

    // Agent의 행동을 설정하는 함수
    public override void OnActionReceived(ActionBuffers actions)
    {
        // parameter Continuous action 개수 맞추기, Discrete Branch 0 만들기
        // continuous Action 2개
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

    // 사람이 에이전트를 수동으로 제어하는 방법을 설정하는 함수
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 방향키를 입력을 통해 행동값을 설정한다.
        var ContinuousActionsOut = actionsOut.ContinuousActions;
        ContinuousActionsOut[0] = -Input.GetAxis("Horizontal");
        ContinuousActionsOut[1] = Input.GetAxis("Vertical");

    }
}
