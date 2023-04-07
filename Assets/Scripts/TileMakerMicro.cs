using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMakerMicro : MonoBehaviour
{

    public Tilemap tilemap;

    public TileBase tileType;

    private float worldX;
    private float worldY;



    void Start()
    {

        worldX = transform.position.x;
        worldY = transform.position.y;

        transform.position = new Vector3(worldX, worldY, 0);

        Invoke("TileChecker", 3.0f);

    }

    void TileChecker()
    {
        worldX = (worldX / 0.16f) - 0.5f;
        worldY = (worldY / 0.16f) - 0.5f;

        if (tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(worldX), Mathf.RoundToInt(worldY), 0)) == null)
        {
            tilemap.SetTile(new Vector3Int(Mathf.RoundToInt(worldX), Mathf.RoundToInt(worldY), 0), (tileType));
        }
        Destroy(gameObject);
    }
}