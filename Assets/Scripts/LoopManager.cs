using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> loops;
    public static event Action<List<GameObject>> onLoopUpdated;
    private int currentIndex = 0;
    void Awake()
    {

        foreach (Transform child in transform)
        {
            loops.Add(child.gameObject);
        }
        onLoopUpdated?.Invoke(loops);
    }

    public void EnableObject()
    {
        loops[currentIndex].gameObject.SetActive(true);
        int i = currentIndex + 1;
        currentIndex = Mathf.Clamp(i, 0, loops.Count - 1);
    }
}
