using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMaker : MonoBehaviour
{
    private int WaitASecond = 0;

    public Tilemap tilemap;

    public TileBase tileLand;
    public TileBase tileWater;

    private float worldX;
    private float worldY;


    List<List<int>> gameWorld = new List<List<int>>
    {
        new List<int> { 0, 0, 0, 0, 0},
        new List<int> { 0, 1, 1, 1, 0},
        new List<int> { 0, 1, 1, 1, 0},
        new List<int> { 0, 1, 1, 1, 0},
        new List<int> { 0, 0, 0, 0, 0},
    };

    void Start()
    {   
        worldX = transform.position.x;
        worldY = transform.position.y;

        worldY = (((worldY / 0.16f) * 10000) / 5000);
        worldY = Mathf.Round(worldY);
        worldY = ((worldY * 5000) / 10000);
        worldY = ((Mathf.Round(worldY)) + 0.5f) * 0.16f;

        worldX = (((worldX / 0.16f) * 10000)/5000);
        worldX = Mathf.Round(worldX * 1);
        worldX = ((worldX * 5000)/10000);
        worldX = ((Mathf.Round(worldX)) + 0.5f)*0.16f;

        transform.position = new Vector3(worldX, worldY, 0);

    }

    void Update()
    {
        WaitASecond++;
        if (WaitASecond == 120) {
            worldX = (worldX / 0.16f) - 0.5f;
            worldY = (worldY / 0.16f) - 0.5f;

        Debug.Log(worldX);
        Debug.Log(worldY);
            for (int x = 0; x < gameWorld.Count; x++)
            {
                for (int y = 0; y < gameWorld[x].Count; y++)
                {
                    tilemap.SetTile(new Vector3Int(Mathf.RoundToInt(worldX) + x, Mathf.RoundToInt(worldY) + y, 0), (gameWorld[x][y] == 0 ? tileWater : tileLand));
                }
            }
        }
        if (WaitASecond == 3600)
        {
            Destroy(gameObject);
        }
    }
}
