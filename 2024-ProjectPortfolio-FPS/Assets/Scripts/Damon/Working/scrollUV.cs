using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollUV : MonoBehaviour
{

    [SerializeField] float scrollSpeedX = 0.1f; 

    private Renderer rend;


    void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        float offsetX = Time.time * scrollSpeedX;
        Vector2 offset = new Vector2(offsetX, 0.0f);

        rend.material.mainTextureOffset = offset;
    }
}
