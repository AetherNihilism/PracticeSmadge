using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;
    private Tilemap LandTileset;
    private Tilemap LandCollidables;

    private void Awake()
    {
        instance = this;
        LandTileset = GameObject.Find("Land").GetComponent<Tilemap>();
        LandCollidables = GameObject.Find("Collidables").GetComponent<Tilemap>();
    }

    public Grid WorldGrid;

    public List<TileBase> GrassTileList = new List<TileBase>();
    public List<TileBase> DirtTileList = new List<TileBase>();
    public List<TileBase> SnowTileList = new List<TileBase>();

    public List<TileBase> GrassTileCollidables = new List<TileBase>();
    public List<TileBase> DirtTileCollidables = new List<TileBase>();
    public List<TileBase> SnowTileCollidables = new List<TileBase>();

    public List<GameObject> ActiveSeeds = new List<GameObject>();
    public List<GameObject> DormantSeeds = new List<GameObject>();

    public TileBase SeedTile;

    public string TileName;

    public GameObject Seedoid;

    public Dictionary<string, Dictionary<string, List<TileBase>>> WorldGenLookup = new Dictionary<string, Dictionary<string, List<TileBase>>>();

    public void WorldStart(GameObject SeedObject, Transform PlayerLocation, float Rotation)
    {
        GenerateDictionary(GrassTileList, GrassTileCollidables, 3, 3, 0);
        GenerateDictionary(SnowTileList, SnowTileCollidables, 2, 3, 0);
        GenerateDictionary(DirtTileList, DirtTileCollidables, 4, 3, 0);
        GameObject seed = Instantiate(SeedObject, PlayerLocation.position, Quaternion.AngleAxis(Rotation, Vector3.forward));
        seed.GetComponent<TileSeederVoronoi>().TileList = WorldGenLookup["330"]["GroundTiles"];
        seed.GetComponent<TileSeederVoronoi>().TileCollidables = WorldGenLookup["330"]["CollidableTiles"];
        seed.GetComponent<TileSeederVoronoi>().Moisture = 3;
        seed.GetComponent<TileSeederVoronoi>().Heat = 3;
        seed.GetComponent<TileSeederVoronoi>().Danger = 0;
        ActiveSeeds.Add(seed);
        Invoke("SowSeedsTest", 2.0f);
    }

    void SowSeedsTest()
    {
        int[] QuickList = { -1, 1 };
        int Variance = QuickList[Random.Range(0, QuickList.Length)];
        var shrinky = ActiveSeeds[0].GetComponent<TileSeederVoronoi>();
        SowSeeds(ActiveSeeds[0], 1.5f, 4, shrinky.Moisture+Variance, shrinky.Heat, shrinky.Danger);
        WorldConstructor(ActiveSeeds[0].transform.position, ActiveSeeds);
     //   DormantSeeds.Add(ActiveSeeds[0]);
    //    ActiveSeeds.Remove(ActiveSeeds[0]);
    }

    public void SowSeeds(GameObject TargetSeed, float seedrange, int seedamount, int Heat, int Moisture, int Danger)
    {
        for (int i = 0; i < seedamount; i++)
        {
            float angle = Random.Range(22.5f, 67.5f) + (i * 90);
            float x = TargetSeed.transform.position.x + (seedrange * Mathf.Cos(angle / (180f / Mathf.PI)));
            float y = TargetSeed.transform.position.y + (seedrange * Mathf.Sin(angle / (180f / Mathf.PI)));
            Vector3 SeedLocation = new Vector3(x, y, 0);
            GameObject seed = Instantiate(Seedoid, SeedLocation, Quaternion.AngleAxis(0, Vector3.forward));
            seed.GetComponent<TileSeederVoronoi>().TileList = WorldGenLookup[Heat.ToString()+Moisture.ToString()+Danger.ToString()]["GroundTiles"];
            seed.GetComponent<TileSeederVoronoi>().TileCollidables = WorldGenLookup[Heat.ToString() + Moisture.ToString() + Danger.ToString()]["CollidableTiles"];
            seed.GetComponent<TileSeederVoronoi>().Heat = Heat;
            seed.GetComponent<TileSeederVoronoi>().Moisture = Moisture;
            seed.GetComponent<TileSeederVoronoi>().Danger = Danger;
            ActiveSeeds.Add(seed);
        }
    }

    void WorldConstructor(Vector3 OriginPoint, List<GameObject> ActiveSeeders)
    {
        Vector3 SubOrigin = new Vector3(OriginPoint.x, OriginPoint.y);
        
        for (int i = -5; i < 6; i++)
        {
            for (int j = -5; j < 6; j++)
            {
                SubOrigin = OriginPoint;
                SubOrigin.x += i * 0.16f;
                SubOrigin.y += j * 0.16f;
                if (LandTileset.GetTile(WorldGrid.WorldToCell(SubOrigin)) == null)
                {
                    LandTileset.SetTile((WorldGrid.WorldToCell(SubOrigin)), ActiveSeeders[0].GetComponent<TileSeederVoronoi>().TileList[Random.Range(0, ActiveSeeders[0].GetComponent<TileSeederVoronoi>().TileList.Count)]); // Use the component attached to the object on the list which contains tilelist, then randomly grab a tile from the list
                    Debug.Log(ActiveSeeders[0].GetComponent<TileSeederVoronoi>().TileList[Random.Range(0, ActiveSeeders[0].GetComponent<TileSeederVoronoi>().TileList.Count)]);
                }
            }
        }
    }

    void GenerateDictionary(List<TileBase> BaseTiles, List<TileBase> Collidables, int Heat, int Moisture, int Danger)
    {
        Dictionary<string, List<TileBase>> TileDictionary = new Dictionary<string, List<TileBase>>();
        TileDictionary.Add("GroundTiles", BaseTiles);
        TileDictionary.Add("CollidableTiles", Collidables);
        WorldGenLookup.Add(Heat.ToString()+Moisture.ToString()+Danger.ToString(), TileDictionary);
    }

    public (List<TileBase>, List<TileBase>) FindGrassNeighbor()
    {
        int CaseChooser = Random.Range(0, 2);
        switch (CaseChooser)
        {
            case 0:
                TileName = "TileDirt";
                return (new List<TileBase>(DirtTileList), new List<TileBase>(DirtTileCollidables));
            case 1:
                TileName = "TileSnow";
                return (new List<TileBase>(SnowTileList), new List<TileBase>(SnowTileCollidables));
            default:
                TileName = "TileDirt";
                return (new List<TileBase>(DirtTileList), new List<TileBase>(DirtTileCollidables));
        }
    }
    public (List<TileBase>, List<TileBase>) FindDirtNeighbor()
    {
        int CaseChooser = Random.Range(0, 2);
        switch (CaseChooser)
        {
            case 0:
                TileName = "TileGrass";
                return (new List<TileBase>(GrassTileList), new List<TileBase>(GrassTileCollidables));
            case 1:
                TileName = "TileSnow";
                return (new List<TileBase>(SnowTileList), new List<TileBase>(SnowTileCollidables));
            default:
                TileName = "TileGrass";
                return (new List<TileBase>(GrassTileList), new List<TileBase>(GrassTileCollidables));
        }
    }
    public (List<TileBase>, List<TileBase>) FindSnowNeighbor()
    {
        int CaseChooser = Random.Range(0, 2);
        Debug.Log(CaseChooser);
        switch (CaseChooser)
        {
            case 0:
                TileName = "TileDirt";
                return (new List<TileBase>(DirtTileList), new List<TileBase>(DirtTileCollidables));
            case 1:
                TileName = "TileGrass";
                return (new List<TileBase>(GrassTileList), new List<TileBase>(GrassTileCollidables));
            default:
                TileName = "TileDirt";
                return (new List<TileBase>(DirtTileList), new List<TileBase>(DirtTileCollidables));
        }
    }

}
