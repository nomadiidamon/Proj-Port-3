using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollUV : MonoBehaviour
{

    [SerializeField] public float scrollSpeedX = 0.1f;
    [SerializeField] public float scrollSpeedY = 0.1f;

    [SerializeField] public bool horizontal;
    [SerializeField] public bool vertical;
    private Renderer rend;



    void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    void Update()
    {

        float offsetX = 0f;
        float offsetY = 0f;

        if (horizontal)
        {
            offsetX = Time.time * scrollSpeedX;
        }

        if (vertical)
        {
            offsetY = Time.time * scrollSpeedY;
        }

        Vector2 offset = new Vector2(offsetX, offsetY);
        rend.material.mainTextureOffset = offset;


    }
}
