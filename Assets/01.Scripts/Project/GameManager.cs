using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<Vector3> InitPos;
    public List<Vector3> InitRot;

    private void Awake()
    {
        Instance = this;
    }
}
