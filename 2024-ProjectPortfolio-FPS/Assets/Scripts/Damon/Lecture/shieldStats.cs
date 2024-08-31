using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]

public class shieldStats : gunStats
{
    //private BoxCollider collider;

    private void Start()
    {
        isShield = true;
       //collider = this.GetComponentInParent<BoxCollider>();
    }

    //public BoxCollider GetCollider()
    //{
    //    return collider;
    //}

}
