using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMakerLite : MonoBehaviour
{

    public Tilemap tilemap;

    public TileBase tileType;

    private float worldX;
    private float worldY;



    void Start()
    {
        tilemap = GameObject.Find("Land").GetComponent<Tilemap>();

        worldX = transform.position.x;
        worldY = transform.position.y;

        worldY = (((worldY / 0.16f) * 10000) / 5000);
        worldY = Mathf.Round(worldY);
        worldY = ((worldY * 5000) / 10000);
        worldY = ((Mathf.Round(worldY)) + 0.5f) * 0.16f;

        worldX = (((worldX / 0.16f) * 10000) / 5000);
        worldX = Mathf.Round(worldX * 1);
        worldX = ((worldX * 5000) / 10000);
        worldX = ((Mathf.Round(worldX)) + 0.5f) * 0.16f;

        transform.position = new Vector3(worldX, worldY, 0);

        InvokeRepeating("Moveoid", 1.0f, 0.5f);

    }

    void Moveoid()
    {
        transform.position = new Vector3(transform.position.x + 0.08f, transform.position.y, 0);

        worldX = transform.position.x;
        worldY = transform.position.y;

        worldY = (((worldY / 0.16f) * 10000) / 5000);
        worldY = Mathf.Round(worldY);
        worldY = ((worldY * 5000) / 10000);
        worldY = ((Mathf.Round(worldY)) + 0.5f) * 0.16f;

        worldX = (((worldX / 0.16f) * 10000) / 5000);
        worldX = Mathf.Round(worldX * 1);
        worldX = ((worldX * 5000) / 10000);
        worldX = ((Mathf.Round(worldX)) + 0.5f) * 0.16f;

        transform.position = new Vector3(worldX, worldY, 0);

        TileChecker();
    }

    void TileChecker()
    {
        worldX = (worldX / 0.16f) - 0.5f;
        worldY = (worldY / 0.16f) - 0.5f;

        if (tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(worldX), Mathf.RoundToInt(worldY), 0)) == null)
        {
            tilemap.SetTile(new Vector3Int(Mathf.RoundToInt(worldX), Mathf.RoundToInt(worldY), 0), (tileType));
            Debug.Log(Mathf.RoundToInt(worldX).ToString() + Mathf.RoundToInt(worldY).ToString());
            Destroy(gameObject);
        }
    }
}