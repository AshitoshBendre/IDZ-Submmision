using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPoint : MonoBehaviour
{
    public static event Action<Vector2,String> onGameObjectPressed;
    private void OnMouseDown()
    {
        Vector2 pointPos = new Vector2(this.transform.position.x, this.transform.position.y);
        Debug.Log(this.gameObject.name);
        onGameObjectPressed?.Invoke(pointPos,this.gameObject.name);
    }
}
