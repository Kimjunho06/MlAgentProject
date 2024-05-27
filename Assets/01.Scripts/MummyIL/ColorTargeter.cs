using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTargeter : MonoBehaviour
{
    public enum TARGET_COLOR
    {
        BLUE,
        RED,
        GREEN,
        PURPLE
    }

    public TARGET_COLOR targetColor = TARGET_COLOR.BLUE;
    public Material[] targetColorMaterial;

    private Renderer hintRenderer;
    private int prevColorIndex = -1;

    private void Start()
    {
        hintRenderer = transform.Find("Hint").GetComponent<Renderer>();   
    }

    public void TargetingColor()
    {
        int currentColorIndex;
        do
        {
            currentColorIndex = Random.Range(0, targetColorMaterial.Length);
        }
        while (currentColorIndex == prevColorIndex);

        prevColorIndex = currentColorIndex;
        targetColor = (TARGET_COLOR)currentColorIndex;
        hintRenderer.material = targetColorMaterial[currentColorIndex];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TargetingColor();
        }
    }


}
