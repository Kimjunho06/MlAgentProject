using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using TMPro;

public class BallControl : MonoBehaviour
{
    public Agent[] players;
    private new Rigidbody rigidbody;

    private int redScore, blueScore;
    public TMP_Text redScoreText, blueScoreText;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.position.y < -10f)
        {
            InitBall();

            players[0].EndEpisode();
            players[1].EndEpisode();
        }
    }

    void InitBall()
    {
        rigidbody.velocity = rigidbody.angularVelocity = Vector3.zero;

        transform.localPosition = new Vector3(0.0f, 15.0f, 0.0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("RED_GOAL"))
        {
            blueScoreText.SetText($"{blueScore++}");

            players[1].AddReward(5);

            InitBall();

            players[0].EndEpisode();
            players[1].EndEpisode();
        }
        if (collision.collider.CompareTag("BLUE_GOAL"))
        {
            redScoreText.SetText($"{redScore++}");

            players[0].AddReward(5);

            InitBall();

            players[0].EndEpisode();
            players[1].EndEpisode();
        }

        if (collision.collider.CompareTag("WALL"))
        {
            players[0].AddReward(-0.5f);
            players[1].AddReward(-0.5f);

            InitBall();

            players[0].EndEpisode();
            players[1].EndEpisode();
        }
        
    }
}
