using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private Transform gridTransfrom;
    [SerializeField] private GameObject prefab;
    [SerializeField, Range(0.1f, 10f)] private float cellSize;
    [SerializeField, Range(0f, 100f)] private int xSize =10;
    [SerializeField, Range(0f, 100f)] private int ySize =10;


    [ContextMenu("Generate Grid")]
    void GenerateCubes()
    {
        float offsetX = (xSize-1) * cellSize / 2.0f;
        float offsetY = (ySize-1)*cellSize/2.0f;
        ClearGridPoints(gridTransfrom);
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                var point = Instantiate(prefab,gridTransfrom);
                float x = i * cellSize - offsetX;
                float y = j * cellSize - offsetY;

                point.AddComponent<GridPoint>(); 
                point.transform.position = new Vector3(x, y, 0);
                point.gameObject.name = $"point {i},{j}";
            }
        }
    }

    
    void ClearGridPoints(Transform T)
    {
        for (int i = T.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(T.transform.GetChild(i).gameObject);
        }
    }
}

