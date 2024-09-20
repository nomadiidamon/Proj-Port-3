using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollUV : MonoBehaviour
{

    [SerializeField] public float scrollSpeedX = 0.1f;
    [SerializeField] public float MinScrollSpeedX = 0.1f;
    [SerializeField] public float MaxScrollSpeedX = 2f;

    [SerializeField] public float scrollSpeedY = 0.1f;
    [SerializeField] public float MinScrollSpeedY = 0.1f;
    [SerializeField] public float MaxScrollSpeedY = 2f;

    [SerializeField] public bool horizontal;
    [SerializeField] public bool vertical;
    [SerializeField] public bool randomizeX;
    [SerializeField] public bool randomizeY;

    private Renderer rend;



    void Start()
    {
        rend = GetComponent<MeshRenderer>();

        if (randomizeX)
        {
            scrollSpeedX = Random.Range(MinScrollSpeedX, MaxScrollSpeedX);
        }

        if (randomizeY)
        {
            scrollSpeedY = Random.Range(MinScrollSpeedY, MaxScrollSpeedY);
        }
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

        rend.material.mainTextureOffset = new Vector2(offsetX, offsetY);


    }
}
