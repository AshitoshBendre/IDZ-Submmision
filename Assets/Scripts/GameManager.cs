using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> LoopContainers;
    [SerializeField] private List<GameObject> SequenceContainers;
    public List<SOQuestionGrid> Levels;
    public Transform CodeCenter;

    public Transform Grid;
    public Transform answerGrid;
    public List<GameObject> answerGridPoints;

    public List<DirectionCountPair> loopingInstructions = new List<DirectionCountPair>();
    public List<DirectionCountPair> sequenceInstructions = new List<DirectionCountPair>();
    public List<DirectionCountPair> curerntInstructions = new List<DirectionCountPair>();

    public List<GameObject> path = new List<GameObject>();

    private Dictionary<ArrowDirection, Vector2Int> directionVectors = new Dictionary<ArrowDirection, Vector2Int>
    {
        { ArrowDirection.Up, new Vector2Int(0, 1) },
        { ArrowDirection.Down, new Vector2Int(0, -1) },
        { ArrowDirection.Left, new Vector2Int(-1, 0) },
        { ArrowDirection.Right, new Vector2Int(1, 0) }
    };


    public GameObject winpanel;
    public GameObject losepanel;

    private void Awake()
    {
        LineRenderer lr = Grid.GetComponent<LineRenderer>();
        lr.enabled = true;
        SOQuestionGrid level = Levels[0];
        lr.positionCount = level.GridPositions.Count;
        lr.SetPositions(level.GridPositions.ToArray());

        foreach(Transform child in answerGrid)
        {
            answerGridPoints.Add(child.gameObject);
        }
    }


    public void UpdateDictionary()
    {
        // Clear the list before updating
        loopingInstructions.Clear();

        foreach (GameObject go in LoopContainers)
        {
            var lp = go.GetComponent<LoopContainer>();
            if (lp != null)
            {
                int count = lp.counter;
                ArrowDirection dir = lp.arrowDirection;
                loopingInstructions.Add(new DirectionCountPair { direction = dir, count = count });
            }
            else
            {
                Debug.LogWarning($"GameObject {go.name} does not have a LoopContainer component.");
            }
        }
        DebugInstructions();
        NavigateGrid(0);
    }

    private void DebugInstructions()
    {
        foreach (var instruction in loopingInstructions)
        {
            Debug.Log($"Direction: {instruction.direction}, Count: {instruction.count}");
        }
    }

    public void UpdateSequenceContainers()
    {
        // Clear the list before updating
        // Create a new list to hold instructions for sequence containers if needed
        sequenceInstructions.Clear();

        foreach (GameObject go in SequenceContainers)
        {
            var sp = go.GetComponent<SequenceContainer>(); // Assuming you have a SequenceContainer component
            if (sp != null)
            {
                int count = 1;
                ArrowDirection dir = sp.arrowDirection;
                sequenceInstructions.Add(new DirectionCountPair { direction = dir, count = count });
            }
            else
            {
                Debug.LogWarning($"GameObject {go.name} does not have a SequenceContainer component.");
            }
        }

        DebugSequenceInstructions(sequenceInstructions);
        NavigateGrid(1);
    }


    private void DebugSequenceInstructions(List<DirectionCountPair> sequenceInstructions)
    {
        Debug.LogWarning("Started Sequence Debug");
        foreach (var instruction in sequenceInstructions)
        {
            Debug.Log($"Sequence Direction: {instruction.direction}, Count: {instruction.count}");
        }
        Debug.LogWarning("Ended Sequence Debug");
    }

    public void NavigateGrid(int k)
    {
        Vector2Int currentPosition = new Vector2Int(0, 4); 
        path.Clear();
        foreach (GameObject points in answerGridPoints)
        {
            if (points.name.Equals("point 0,4"))
            {
                path.Add(points);
            }

        }

        if (k == 0)
        {
            curerntInstructions = loopingInstructions;
            Debug.Log("Using Looping Instructions");
        }
        else if (k == 1)
        {
            curerntInstructions = sequenceInstructions;
            Debug.Log("Using Sequence Instructions");
        }
        else
        {
            Debug.LogError("Invalid value for k. Must be 0 or 1.");
            return;
        }

        if (curerntInstructions == null || curerntInstructions.Count == 0)
        {
            Debug.LogWarning("No instructions to process.");
            return;
        }

        foreach (var instruction in curerntInstructions)
        {
            Debug.Log($"Processing Instruction - Direction: {instruction.direction}, Count: {instruction.count}");
            for (int i = 0; i < instruction.count; i++)
            {
                if (!directionVectors.ContainsKey(instruction.direction))
                {
                    Debug.LogError($"Direction {instruction.direction} is not in directionVectors.");
                    continue;
                }

                currentPosition += directionVectors[instruction.direction];
                Debug.Log($"New Position: {currentPosition.x},{currentPosition.y}");

                // Check if the currentPosition is within the bounds of the 5x5 grid
                if (IsWithinBounds(currentPosition))
                {
                    string pointName = $"point {currentPosition.x},{currentPosition.y}";
                    GameObject point = answerGridPoints.FirstOrDefault(p => p.name.Equals(pointName));
                    if (point != null)
                    {
                        path.Add(point);
                    }
                    else
                    {
                        Debug.LogWarning($"Point with name {pointName} not found in answerGridPoints.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Position {currentPosition} is out of bounds.");
                }
            }
        }

        // Debug the path
        foreach (var point in path)
        {
            Debug.Log($"Path Point: {point.name} & {point.transform.position.x},{point.transform.position.y}");
        }

        StartCoroutine(ShowLineRenderer());
    }

    private IEnumerator ShowLineRenderer()
    {
        LineRenderer lp = answerGrid.GetComponent<LineRenderer>();
        lp.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            lp.SetPosition(i, path[i].transform.position);
            yield return new WaitForSeconds(0.3f);
        }

        CheckResult();
    }

    private void CheckResult()
    {
        List<Vector2> gridWinPosition = new List<Vector2>();
        foreach (GameObject i in path)
        {
            string coordinates = i.gameObject.name.Replace("points ", "");
            string[] parts = coordinates.Split(',', ' ');

            if (parts.Length > 0)
            {
                int x = int.Parse(parts[1]);
                int y = int.Parse(parts[2]);
                gridWinPosition.Add(new Vector2(x, y));
            }
        }

        if (AreListsEqual(gridWinPosition, Levels[0].WinPosition))
        {
            Debug.Log("You WOn");
            winpanel.SetActive(true);
        }
        else
        {
            Debug.Log("You Lose");
            losepanel.SetActive(true);
        }
    }
    private bool AreListsEqual(List<Vector2> list1, List<Vector2> list2)
    {
        // Check if the lists have the same count
        if (list1.Count != list2.Count)
            return false;

        // Sort the lists and compare
        List<Vector2> sortedList1 = list1.OrderBy(v => v.x).ThenBy(v => v.y).ToList();
        List<Vector2> sortedList2 = list2.OrderBy(v => v.x).ThenBy(v => v.y).ToList();

        return sortedList1.SequenceEqual(sortedList2);
    }

    private bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < 5 && position.y >= 0 && position.y < 5;
    }
}
