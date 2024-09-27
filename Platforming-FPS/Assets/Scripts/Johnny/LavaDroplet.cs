using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                Instantiate(lavaPuddlePrefab, hit.point, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
