using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    [SerializeField] Vector3 startingPoint;
    private Vector3 currentPosition;
    [SerializeField] Vector3 endingPoint;
    [SerializeField] float speed;
    [SerializeField] int timeUntilReturn;


    // Start is called before the first frame update
    void Start()
    {
        currentPosition = startingPoint;
        //movePlatform();
    }

    // Update is called once per frame
    void Update()
    {
        //movePlatform();
        goUp();
        goDown();
    
    }

    public void goUp()
    {
        currentPosition.y = endingPoint.y;
        //startingPoint *= this.transform.position.y * speed * Time.deltaTime;
    }

    public void goDown()
    {
        if (currentPosition.y != endingPoint.y) { return; }
            currentPosition.y -= startingPoint.y * (speed);

    }

    IEnumerator movePlatform()
    {
        goUp();
        yield return new WaitForSeconds(timeUntilReturn);
        if (this.transform.position == endingPoint)
        {
            goDown();
        }
    }
}
