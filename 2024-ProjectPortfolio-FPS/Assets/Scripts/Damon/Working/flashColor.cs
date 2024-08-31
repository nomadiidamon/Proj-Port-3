using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flashColor : MonoBehaviour
{
    [SerializeField] float flashSpeed;
    [SerializeField] Light myLight;

    private Renderer rend;
    private Color color1 = Color.magenta;
    private Color color2 = Color.yellow;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        time = 0.0f;
        //color1 = rend.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * flashSpeed;
        float lerpFactor = Mathf.PingPong(time, 3f);

        Color modelColor = Color.Lerp(color1, color2, lerpFactor);
        rend.material.color = modelColor;
        Color lightColor = Color.Lerp(color1, color2, lerpFactor);
        myLight.color = lightColor;
    }
}
