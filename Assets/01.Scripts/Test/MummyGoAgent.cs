using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MummyGoAgent : Agent
{
    public MeshRenderer _floor;
    public Transform _targetTrm;
    private Rigidbody _rigidbody;

    // 환경이 처음 시작될 때 한 번만 실행되는 초기화 함수 (Start)
    public override void Initialize()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // 각 에피소드가 시작될 때 호출되는 함수 => 환경의 상태를 초기화하는 역할
    public override void OnEpisodeBegin()
    {
        float TrandX = Random.Range(-4.5f, 4.5f);
        float TrandZ = Random.Range(-4.5f, 4.5f);
        float randX = Random.Range(-4.5f, 4.5f);
        float randZ = Random.Range(-4.5f, 4.5f);

        _targetTrm.localPosition = new Vector3(TrandX, 0.05f, TrandZ);

        transform.localPosition = new Vector3(randX, 0.05f, randZ);
        _rigidbody.velocity = Vector3.zero;

        _floor.material.color = Color.gray;
    }

    // Agent의 관측을 설정하는 함수 => vector Observation(벡터 관측)
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_targetTrm.localPosition);  
        sensor.AddObservation(_rigidbody.velocity.x);  
        sensor.AddObservation(_rigidbody.velocity.z);  
    }

    // Agent의 행동을 설정하는 함수
    public override void OnActionReceived(ActionBuffers actions)
    {
        var ContinuousActions = actions.ContinuousActions;
        float x_move = Mathf.Clamp(ContinuousActions[0], -1f, 1f);
        float z_move = Mathf.Clamp(ContinuousActions[1], -1f, 1f);

        _rigidbody.velocity = new Vector3(x_move, 0, z_move).normalized * 10;

        SetReward(-0.001f);
    }

    // 사람이 에이전트를 수동으로 제어하는 방법을 설정하는 함수
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ContinuousActionsOut = actionsOut.ContinuousActions;
        ContinuousActionsOut[0] = -Input.GetAxis("Horizontal");
        ContinuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TARGET"))
        {
            StartCoroutine(ColorDelay(collision.gameObject.tag));
        }
        if (collision.gameObject.CompareTag("WALL"))
        {
            StartCoroutine(ColorDelay(collision.gameObject.tag));
        }
    }

    private IEnumerator ColorDelay(string state)
    {
        switch (state)
        {
            case "TARGET":
                _floor.material.color = Color.blue;
                yield return new WaitForSeconds(0.2f);
                SetReward(0.1f);
                EndEpisode();
                break;
            case "WALL":
                _floor.material.color = Color.red;
                yield return new WaitForSeconds(0.2f);
                SetReward(-1f);
                EndEpisode();
                break;
        }

    }
}
