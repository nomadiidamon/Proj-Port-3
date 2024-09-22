using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Rendering;
using UnityEngine;

public class cameraController : MonoBehaviour
{

    public float sens;
    [SerializeField] int lockVertMin, lockVertMax;
    public bool invertY;

    public Camera playerCamera;
    public Transform playerTransform;
    public float minFOV = 30f;
    public float maxFOV = 300f;
    public float cameraDistance = 0f;
    public LayerMask collisionMask;

    float rotX;
    Vector3 cameraOffset;

    // Camera shake
    private float shakeDuration = 0f;
    private float shakeIntensity = 0f;
    private float shakeTimer = 0f;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraOffset = playerCamera.transform.localPosition.normalized * cameraDistance;

        originalCamPos = playerCamera.transform.localPosition;
        originalCamRot = playerCamera.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        // get input
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;

        if (invertY)
            rotX += mouseY;
        else
            rotX -= mouseY;

        // clamp the rotX on the x-axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // rotate camera on the x-axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //rotate the PLAYER on the y-axis
        transform.parent.Rotate(Vector3.up * mouseX);

        if (shakeTimer > 0)
        {
            //Debug.Log("Shaking! timer: " + shakeTimer);
            playerCamera.transform.localPosition = originalCamPos + Random.insideUnitSphere * shakeIntensity;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            playerCamera.transform.localPosition = originalCamPos;
        }

        if (shakeTimer <= 0)
        {
            // camera collison
            HandleCameraCollision();
        }
    
        

    }

    void HandleCameraCollision()
    {
        Vector3 newPos = playerTransform.position - transform.forward * cameraDistance;
        RaycastHit hit;

        // Perform a raycast to check if there is an object between the player and the camera
        if (Physics.Raycast(playerTransform.position, -transform.forward, out hit, cameraDistance, collisionMask))
        {
            // Move the camera closer if there's a collision
            playerCamera.transform.position = hit.point;
        }
        else
        {
            // No collision, set the camera to its desired position
            playerCamera.transform.position = newPos;
        }
    }

    public void setSens(float newSens)
    {
        sens = newSens;
    }

    public void SetInvertY(bool toggle)
    {
        invertY = toggle;
        
    }

    public void SetFOV(float fov)
    {
        playerCamera.fieldOfView = Mathf.Clamp(fov, minFOV, maxFOV);
    }

    public void TriggerCameraShake(float duration, float intensity)
    {
        //Debug.Log("Camera shake triggered with duration: " + duration + " and intensity: " + intensity);
        shakeDuration = duration;
        shakeIntensity = intensity;
        shakeTimer = duration;

        originalCamPos = playerCamera.transform.localPosition;
        originalCamRot = playerCamera.transform.localRotation;
    }
}
