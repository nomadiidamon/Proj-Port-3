using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGunBlast : MonoBehaviour
{
    [SerializeField] GameObject[] crystalShards;
    [SerializeField] int maxShards;
    [Range(0, 1)][SerializeField] public float blastAudio;
    [Range(0, 1)][SerializeField] float destroyTimer;
    [SerializeField] int minSpeed = 1;
    [SerializeField] int maxSpeed = 10;
    int speed;
    public int currentNumberOfShards = 0;
    BoxCollider blastArea;

    // Start is called before the first frame update
    void Start()
    {
        blastArea = GetComponent<CapsuleCollider>().GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentNumberOfShards < maxShards)
        {
            GameObject shard = Instantiate(crystalShards[Random.Range(0, crystalShards.Length)], GetRandomPoint() + transform.position, Quaternion.identity);
            Rigidbody rb = shard.GetComponent<Rigidbody>();
            speed = Random.Range(minSpeed, maxSpeed);
            shard.transform.rotation = Random.rotation;
            //int meteorSize = Random.Range(1, 3);
            //shard.transform.localScale = new Vector3(meteorSize, meteorSize, meteorSize);
            StartCoroutine(DestroyShard(shard));

            currentNumberOfShards++;
        }
    }

    Vector3 GetRandomPoint()
    {
        float width = blastArea.size.x * transform.localScale.x;
        float length = blastArea.size.z * transform.localScale.z;

        float x = Random.Range(-width / 2, width / 2);
        float z = Random.Range(-length / 2, length / 2);

        return new Vector3(x, -5, z);
    }

    IEnumerator DestroyShard(GameObject meteorToDestroy)
    {
        yield return new WaitForSeconds(destroyTimer);
        if (meteorToDestroy != null)
        {
            Destroy(meteorToDestroy);
            currentNumberOfShards--;
        }
    }
}
