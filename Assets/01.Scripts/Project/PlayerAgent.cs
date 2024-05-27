using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using System;

public enum TeamColor
{
    Red,
    Blue,
    Green,
    Yellow
}

public class PlayerAgent : Agent
{
    public TeamColor TeamColor;

    public Transform ball;

    private BehaviorParameters _behaviorParameters;
    private Rigidbody _rigidbody;

    public override void Initialize()
    {
        _behaviorParameters = GetComponent<BehaviorParameters>();
        _rigidbody = GetComponent<Rigidbody>(); 

        _behaviorParameters.TeamId = (int)TeamColor;
    }

    public override void OnEpisodeBegin()
    {
        _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;

        //if (TeamColor == TeamColor.Red )
        //{
        //    transform.localPosition = new Vector3(Random.Range(-10, -130), 10, Random.Range(-100, 100));
        //}
        //if (TeamColor == TeamColor.Blue )
        //{
        //    transform.localPosition = new Vector3(Random.Range(10, 130), 10, Random.Range(-100, 100));
        //}

        transform.localPosition = GameManager.Instance.InitPos[(int)TeamColor];
        transform.localRotation = Quaternion.Euler(GameManager.Instance.InitRot[(int)TeamColor]);
        prevDist = float.MaxValue;
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    float prevDist;
    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;

        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        int forward = action[0];
        int rotate = action[1];

        switch (forward)
        {
            case 0: dir = Vector3.zero; break;
            case 1: dir = transform.forward; break;
            case 2: dir = -transform.forward; break;
        }

        switch (rotate)
        {
            case 0: rot = Vector3.zero; break;
            case 1: rot = -transform.up; break;
            case 2: rot = transform.up; break;
        }

        _rigidbody.AddForce(dir * 10f, ForceMode.VelocityChange);
        transform.Rotate(rot, Time.fixedDeltaTime * 100f);

        if (Vector3.Distance(transform.position, ball.transform.position) < prevDist)
        {
        }
        else
        {
            AddReward(-0.3f);
        }
        prevDist = Vector3.Distance(transform.position, ball.transform.position);

        AddReward(-1 / (float)MaxStep);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;

        action.Clear();

        if (Input.GetKey(KeyCode.W)) action[0] = 1;
        if (Input.GetKey(KeyCode.S)) action[0] = 2;

        if (Input.GetKey(KeyCode.A)) action[1] = 1;
        if (Input.GetKey(KeyCode.D)) action[1] = 2;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BALL"))
        {
            AddReward(3f);

            Vector3 kickDir = collision.GetContact(0).point - transform.position;
            
            GameObject redgoal = GameObject.Find("Red_GOAL");
            GameObject bluegoal = GameObject.Find("Blue_GOAL");

            // °ñ´ë ÁÂ¿ì
            if (TeamColor == TeamColor.Red)
            {
                if (Math.Sign(Mathf.Pow(transform.position.x - bluegoal.transform.position.x, 2)) == Math.Sign(kickDir.x))
                    AddReward(0.3f);
                else
                    AddReward(-0.6f);
                
            }
            if (TeamColor == TeamColor.Blue)
            {
                if (Math.Sign(Mathf.Pow(transform.position.x - redgoal.transform.position.x, 2)) == Math.Sign(kickDir.x))
                    AddReward(0.3f);
                else
                    AddReward(-0.6f);
            }

            // °ñ´ë °¡¿îµ¥
            if (Math.Sign(kickDir.z) == -Math.Sign(ball.transform.position.z))
            {
                AddReward(0.3f);
            }
            else
                AddReward(-0.6f);

            collision.gameObject.GetComponent<Rigidbody>().AddForce(kickDir * 1f);
        }       
    }
}
