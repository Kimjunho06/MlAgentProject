using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Player : Agent
{
    public Sprite[] sprites;
    public float strength = 5f;
    public float gravity = -9.81f;
    public float tilt = 5f;

    private SpriteRenderer spriteRenderer;
    private Vector3 direction;
    private int spriteIndex;

    private Spawner spawner;
    private TextMeshPro scoreText;
    private int score;

    public override void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawner = transform.parent.Find("Spawner").GetComponent<Spawner>();
        scoreText = transform.parent.Find("ScoreText").GetComponent<TextMeshPro>();
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    public override void OnEpisodeBegin()
    {
        GameStart();
        Vector3 position = transform.localPosition;
        position.y = 0f;
        transform.localPosition = position;
        direction = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var discreteActions = actions.DiscreteActions;

        if (discreteActions[0] == 1)
        {
            if (transform.localPosition.y <= 3.5f)
            {
                Jump();
            }
        }
        AddReward(0.01f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.Space))
        {
            if (transform.localPosition.y <= 3.5f)
            {
                discreteActionsOut[0] = 1;
            }
        }
    }

    private void FixedUpdate()
    {
        // 플레이어에 중력 적용
        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;

        // 방향에 따라 스프라이트 기울이기
        Vector3 rotation = transform.eulerAngles;
        rotation.z = direction.y * tilt;
        transform.eulerAngles = rotation;
    }

    private void Jump()
    {
        direction = Vector3.up * strength;
    }

    private void AnimateSprite()
    {
        spriteIndex++;

        if (spriteIndex >= sprites.Length) 
        {
            spriteIndex = 0;
        }

        if (spriteIndex < sprites.Length && spriteIndex >= 0) 
        {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }

    private void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    private void GameStart()
    {
        score = 0;
        scoreText.SetText(score.ToString());

        foreach(GameObject obj in spawner.pipeList)
        {
            Destroy(obj);
        }

        spawner.pipeList = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle")) 
        {
            AddReward(-1f);
            EndEpisode();
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        } 
        else if (other.gameObject.CompareTag("Scoring")) 
        {
            AddReward(1f);
            IncreaseScore();
        }
    }
}
