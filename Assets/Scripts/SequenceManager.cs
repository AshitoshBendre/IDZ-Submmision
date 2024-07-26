using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> sequence;
    public static event Action<List<GameObject>> onLoopUpdated;
    private int currentIndex = 0;
}
