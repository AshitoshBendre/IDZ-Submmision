using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceContainer : MonoBehaviour
{
    public ArrowDirection arrowDirection;

   public void onObjectDropped(ArrowDirection dir)
    {
        arrowDirection = dir;
        Debug.Log(arrowDirection.ToString());
    }

}
