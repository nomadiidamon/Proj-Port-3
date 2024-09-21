using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorShower : MonoBehaviour
{
    [SerializeField] GameObject[] meteors;
    [SerializeField] int maxMeteors;
    float speed;
    int currentNumberOfMeteors = 0;
    BoxCollider meteorShowerArea;

    // Start is called before the first frame update
    void Start()
    {
        meteorShowerArea = GetComponent<BoxCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        if (currentNumberOfMeteors < maxMeteors)
        {
            GameObject meteor = Instantiate(meteors[Random.Range(0, meteors.Length)], GetRandomPoint() + transform.position, Quaternion.identity);
            SphereCollider sphereCollider = meteor.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            Rigidbody rb = meteor.AddComponent<Rigidbody>();
            rb.useGravity = false;
            speed = Random.Range(0.5f, 5);
            rb.velocity = -transform.up * speed;

            currentNumberOfMeteors++;
        }
    }

    Vector3 GetRandomPoint()
    {
        float width = meteorShowerArea.size.x * transform.localScale.x;
        float length = meteorShowerArea.size.z * transform.localScale.z;

        float x = Random.Range(-width/2, width/2);
        float z = Random.Range(-length/2, length/2);

        return new Vector3(x, -1, z);
    }
}
