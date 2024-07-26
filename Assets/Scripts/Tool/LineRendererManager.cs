using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class LineRendererManager : MonoBehaviour
{
    public string levelName;
    public List<Vector3> gridPointPositions;
    public List<Vector2> gridWinPosition;
    private void OnEnable()
    {
        GridPoint.onGameObjectPressed += onGridPointReceived;
    }
    private void OnDisable()
    {
        GridPoint.onGameObjectPressed -= onGridPointReceived;
    }

    private void onGridPointReceived(Vector2 vector, String obj)
    {
        string coordinates = obj.Replace("points ", "");
        string[] parts = coordinates.Split(',',' ');

        if (parts.Length >0)
        {
            int x = int.Parse(parts[1]);
            int y = int.Parse(parts[2]);
            gridWinPosition.Add(new Vector2(x, y));
        }
        gridPointPositions.Add(vector);          
    }

    [ContextMenu("Save SOData")]
    public void SaveSOData()
    {
        SOQuestionGrid questiongrid = ScriptableObject.CreateInstance<SOQuestionGrid>();
        questiongrid.LevelName = levelName;
        questiongrid.GridPositions = gridPointPositions;
        questiongrid.WinPosition = gridWinPosition;

        string path = $"Assets/Resources/{levelName}Data.asset";
        path = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(questiongrid, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Saved SOQuestionGrid to {path}");
    }
}
