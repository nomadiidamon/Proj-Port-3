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
    public float minFOV = 30f;
    public float maxFOV = 300f;

    float rotX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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


}
