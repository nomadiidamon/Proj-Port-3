using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorShower : MonoBehaviour
{
    [SerializeField] GameObject[] meteors;
    [SerializeField] int maxMeteors;
    [Range(5, 60)][SerializeField] int destroyTimer;
    [SerializeField] int minSpeed = 1;
    [SerializeField] int maxSpeed = 10;
    int speed;
    public int GetMeteorSpeed()
    {
        return speed; 
    }
    public int currentNumberOfMeteors = 0;
   
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
            Rigidbody rb = meteor.GetComponent<Rigidbody>();
            speed = Random.Range(minSpeed, maxSpeed);
            meteor.transform.rotation = Random.rotation;
            int meteorSize = Random.Range(1, 5);
            meteor.transform.localScale = new Vector3(meteorSize,meteorSize,meteorSize);
            // rb.velocity = -transform.up * speed;
            StartCoroutine(DestroyMeteor(meteor));

            currentNumberOfMeteors++;
        }
    }

    Vector3 GetRandomPoint()
    {
        float width = meteorShowerArea.size.x * transform.localScale.x;
        float length = meteorShowerArea.size.z * transform.localScale.z;

        float x = Random.Range(-width/2, width/2);
        float z = Random.Range(-length/2, length/2);

        return new Vector3(x, -5, z);
    }

    IEnumerator DestroyMeteor(GameObject meteorToDestroy)
    {
        yield return new WaitForSeconds(destroyTimer);
        if (meteorToDestroy != null)
        {
            Destroy(meteorToDestroy);
            currentNumberOfMeteors--;
        }
    }
}
