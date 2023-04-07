using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMakerLite2 : MonoBehaviour
{

    public Tilemap tilemap;

    public TileBase tileType;

    public GameObject Builder;

    private float worldX;
    private float worldY;

    public float EastWest;

    public float NorthSouth;

    public int randomizer;



    void Awake()
    {
        worldX = transform.position.x;
        worldY = transform.position.y;

        transform.position = new Vector3(worldX, worldY, 0);

        Invoke("TileChecker", 5.0f);

    }

    void TileChecker()
    {
        worldX = (worldX / 0.16f) - 0.5f;
        worldY = (worldY / 0.16f) - 0.5f;

        if (tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(worldX), Mathf.RoundToInt(worldY), 0)) == null)
        {
            for (int i = 0; i < randomizer; i++)
            {
                GameObject TileLord1 = Instantiate(Builder, new Vector3(transform.position.x + (i * EastWest), transform.position.y + (i * NorthSouth), 0), Quaternion.identity);
                TileLord1.GetComponent<TileMakerMicro>().tilemap = tilemap;
                TileLord1.GetComponent<TileMakerMicro>().tileType = tileType;

                GameObject TileLord2 = Instantiate(Builder, new Vector3(transform.position.x - (i * EastWest), transform.position.y - (i * NorthSouth), 0), Quaternion.identity);
                TileLord2.GetComponent<TileMakerMicro>().tilemap = tilemap;
                TileLord2.GetComponent<TileMakerMicro>().tileType = tileType;
            }
        }
        Destroy(gameObject);
    }
}