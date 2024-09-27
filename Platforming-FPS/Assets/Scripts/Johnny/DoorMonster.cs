using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorMonster : MonoBehaviour
{
    [SerializeField] enemyManager eManager;
    [SerializeField] Transform pivotPoint;
    private Vector3 initialRot;
    private Vector3 targetRotation;
    public float RotAmt;
    public float doorOpenSpeed = 0.5f;
    private bool isOpening = false;
    public bool isLocked;

    // Start is called before the first frame update
    void Start()
    {
        initialRot = pivotPoint.eulerAngles;
        targetRotation = new Vector3(initialRot.x, initialRot.y - RotAmt, initialRot.z);
        isLocked = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (eManager.enemiesRemaining > 1)
        {
            isLocked = true;
            isOpening = false;
        }
        else if (!isOpening && isLocked)
        {
            isLocked = false;
            isOpening = true;
            StartCoroutine(OpenDoor());
        }
    }

    IEnumerator OpenDoor()
    {
        float elapsedTime = 0f;
        Vector3 currentRotation = pivotPoint.eulerAngles; 

        while (elapsedTime < doorOpenSpeed)
        {
            pivotPoint.eulerAngles = Vector3.Lerp(currentRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        pivotPoint.eulerAngles = targetRotation;
    }
}
