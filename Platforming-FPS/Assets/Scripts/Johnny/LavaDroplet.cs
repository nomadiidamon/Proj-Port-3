using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LavaDroplet : MonoBehaviour
{
    [SerializeField] GameObject lavaPuddlePrefab;
    [SerializeField] float minFallSpeed = 2f;
    [SerializeField] float maxFallSpeed = 5f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        float fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
        rb.velocity = new Vector3(0, -fallSpeed, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
            {
                Quaternion randomRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
                Instantiate(lavaPuddlePrefab, hit.point, randomRot);
            }

            Destroy(gameObject);
        }
    }
}
