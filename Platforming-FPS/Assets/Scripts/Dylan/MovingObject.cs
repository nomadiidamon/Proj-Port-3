using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Moving : MonoBehaviour
{
    [SerializeField] float horizontalMove;
    [SerializeField] float verticalMove;
    [SerializeField] float forwardMove;
    [Range(0.1f, 3f)][SerializeField] float moveSpeed;

    Vector3 startingPosition;

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        SphereCollider contactArea = this.AddComponent<SphereCollider>();
        contactArea.isTrigger = true;
    }
    private void FixedUpdate()
    {
        timer += Time.deltaTime * moveSpeed;
        float newX = Mathf.Cos(timer) * horizontalMove;
        float newY = Mathf.Sin(timer) * verticalMove;
        float newZ = Mathf.Cos(timer) * forwardMove;

        transform.position = startingPosition + new Vector3(newX, newY, newZ);
    }
    private void OnTriggerEnter(Collider other)
    {
        other.transform.parent = this.transform;
    }
    private void OnTriggerExit(Collider other)
    {
        other.transform.parent = null;
    }
}