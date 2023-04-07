using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSeeder : MonoBehaviour
{

    public Tilemap tilemap;
    public Tilemap Collidables;

    public TileBase tileType;
    public TileBase lastTile;

    private int randomizer;

    public GameObject Builder;

    public TileBase CrapTile;

    private Vector3Int cellPosition;

    public Grid WorldGrid;

    private List<TileBase> TileChooser = new List<TileBase>();
    private List<TileBase> CollidTile = new List<TileBase>();

    private string IgnoreTile;
    private Vector3 CellPositionBreaker;


    void Start()
    {
        tilemap = GameObject.Find("Land").GetComponent<Tilemap>();
        Collidables = GameObject.Find("Collidables").GetComponent<Tilemap>();

        cellPosition = WorldGrid.WorldToCell(transform.position);
        transform.position = WorldGrid.GetCellCenterWorld(cellPosition);

        InvokeRepeating("Moveoid", 3.0f, 0.5f);

    }

    void Moveoid()
    {
        transform.position = new Vector3(transform.position.x + 0.16f, transform.position.y, 0);

        cellPosition = WorldGrid.WorldToCell(transform.position);
        transform.position = WorldGrid.GetCellCenterWorld(cellPosition);

        TileChecker();
    }

    void TileChecker()
    {

            randomizer = Random.Range(100,200);

        if (tilemap.GetTile(cellPosition) == null)
        {
            TileLogic();
            for (int i = 0; i < randomizer; i++)
            {
                CellPositionBreaker = transform.position;
                CellPositionBreaker.x = CellPositionBreaker.x + (0.16f*i);

                if (tilemap.GetTile(WorldGrid.WorldToCell(CellPositionBreaker)) == null || (tilemap.GetTile(WorldGrid.WorldToCell(CellPositionBreaker))).name.StartsWith(IgnoreTile))
                {
                    CrossHatcher(transform.position, (i * 0.16f), 0, 0f, 0.16f, randomizer - i);
                }
                else break;
            }
            for (int i = 0; i < randomizer; i++)
            {
                CellPositionBreaker = transform.position;
                CellPositionBreaker.x = CellPositionBreaker.x - (0.16f * i);
                if (tilemap.GetTile(WorldGrid.WorldToCell(CellPositionBreaker)) == null || (tilemap.GetTile(WorldGrid.WorldToCell(CellPositionBreaker))).name.StartsWith(IgnoreTile))
                {
                    CrossHatcher(transform.position, -(i * 0.16f), 0, 0f, 0.16f, randomizer - i);
                }
                else break;
            }
            for (int i = 0; i < randomizer; i++)
            {
                CellPositionBreaker = transform.position;
                CellPositionBreaker.y = CellPositionBreaker.y + (0.16f * i);

                if (tilemap.GetTile(WorldGrid.WorldToCell(CellPositionBreaker)) == null || (tilemap.GetTile(WorldGrid.WorldToCell(CellPositionBreaker))).name.StartsWith(IgnoreTile))
                {
                    CrossHatcher(transform.position, 0, (i * 0.16f), 0.16f, 0f, randomizer - i);
                }
                else break;
            }
            for (int i = 0; i < randomizer; i++)
            {
                CellPositionBreaker = transform.position;
                CellPositionBreaker.y = CellPositionBreaker.y - (0.16f * i);
                if (tilemap.GetTile(WorldGrid.WorldToCell(CellPositionBreaker)) == null || (tilemap.GetTile(WorldGrid.WorldToCell(CellPositionBreaker))).name.StartsWith(IgnoreTile))
                {
                    CrossHatcher(transform.position, 0, -(i * 0.16f), 0.16f, 0f, randomizer - i);
                }
                else break;
            }
            Destroy(gameObject);
        }
        else
        {
            lastTile = tilemap.GetTile(cellPosition);
        }
    }

    void CrossHatcher(Vector3 Location, float DropperX, float DropperY, float EastWest, float NorthSouth, int Amount)
    {
        Vector3Int IntLocation;
        Location = new Vector3(Location.x + DropperX, Location.y + DropperY, 0);
        Vector3 OriginLocation = Location;

        IntLocation = WorldGrid.WorldToCell(Location);

        if (tilemap.GetTile(IntLocation) == null || tilemap.GetTile(IntLocation).name.StartsWith(IgnoreTile))
        {
            tilemap.SetTile(IntLocation, TileChooser[Random.Range(0, TileChooser.Count)]);
            for (int o = 0; o < Amount; o++)
            {
                Location = OriginLocation;
                Location.x = Location.x + (EastWest*o);
                Location.y = Location.y + (NorthSouth*o);
                if (tilemap.GetTile(WorldGrid.WorldToCell(Location)) == null || tilemap.GetTile(WorldGrid.WorldToCell(Location)).name.StartsWith(IgnoreTile))
                {
                    Blobber(Location.x, Location.y);
                }
                else break;
            }
            for (int o = 0; o < Amount; o++)
            {
                Location = OriginLocation;
                Location.x = Location.x - (EastWest * o);
                Location.y = Location.y - (NorthSouth * o);
                if (tilemap.GetTile(WorldGrid.WorldToCell(Location)) == null || tilemap.GetTile(WorldGrid.WorldToCell(Location)).name.StartsWith(IgnoreTile))
                {
                    Blobber(Location.x, Location.y);
                }
                else break;
            }

        }
    }

    void Blobber(float BlobPointX, float BlobPointY)
    {
        Vector3 BlobLocation = new Vector3(BlobPointX, BlobPointY, 0);
        Vector3Int BlobLocationInt;
        float Objector;

        BlobLocationInt = WorldGrid.WorldToCell(BlobLocation);
        Objector = GenerateNoise(BlobLocation.x, BlobLocation.y);

        tilemap.SetTile((BlobLocationInt), TileChooser[Random.Range(0, TileChooser.Count)]);

        if (Objector > 0.35 && Objector < 0.355)
        {
            Collidables.SetTile((BlobLocationInt), CollidTile[Random.Range(0, CollidTile.Count)]);
        }
    }

    void TileLogic()
    {
        if (lastTile.name.StartsWith("TileGrass"))
        {
            var GetTiles = TileManager.instance.FindGrassNeighbor();
            TileChooser = GetTiles.Item1;
            CollidTile = GetTiles.Item2;
        }
        if (lastTile.name.StartsWith("TileDirt"))
        {
            var GetTiles = TileManager.instance.FindDirtNeighbor();
            TileChooser = GetTiles.Item1;
            CollidTile = GetTiles.Item2;
        }
        if (lastTile.name.StartsWith("TileSnow"))
        {
            var GetTiles = TileManager.instance.FindSnowNeighbor();
            TileChooser = GetTiles.Item1;
            CollidTile = GetTiles.Item2;
        }
        IgnoreTile = TileManager.instance.TileName;
    }
    public float GenerateNoise(float coordX, float coordY)
    {
        float noiseMap = Mathf.PerlinNoise(coordX, coordY);

        return noiseMap;
    }
}