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

    // Event Manager starts us off with WorldStart after 5 seconds.
    public void WorldStart(GameObject SeedObject, Transform PlayerLocation, float Rotation) // SeedObject is just the prefab, the other two are self explanatory. Rotation is depreciated, too lazy to strip it out atm
    {
        // To kick things off, we generate our three dictionaries for our three tilesets. All variances exist only in the Moisture integer right now, for simplicity.
        GenerateDictionary(GrassTileList, GrassTileCollidables, 3, 3, 0);
        GenerateDictionary(SnowTileList, SnowTileCollidables, 2, 3, 0);
        GenerateDictionary(DirtTileList, DirtTileCollidables, 4, 3, 0);

        // Define our seed object that we are gonna toss around. This first one is defined manually in all ways.
        GameObject seed = Instantiate(SeedObject, PlayerLocation.position, Quaternion.AngleAxis(Rotation, Vector3.forward));
        seed.GetComponent<TileSeederVoronoi>().TileList = WorldGenLookup["330"]["GroundTiles"];
        seed.GetComponent<TileSeederVoronoi>().TileCollidables = WorldGenLookup["330"]["CollidableTiles"];
        seed.GetComponent<TileSeederVoronoi>().Moisture = 3;
        seed.GetComponent<TileSeederVoronoi>().Heat = 3;
        seed.GetComponent<TileSeederVoronoi>().Danger = 0;

        // We Manually add this seed to our Active Seeds list. Active seeds are seeds that in theory can be called upon to generate more surrounding seeds used in worldgen.

        ActiveSeeds.Add(seed);
        Invoke("SowSeedsTest", 2.0f); // Special edition of sowseeds used for initial worldgen
    }

    public void WorldBuildTest(Transform PlayerLocation)
    {
        for (int a = 0; a < 2; a++)
        {
            Vector3 PlayerLoc = PlayerLocation.position;
            GameObject TargetSeed = null;
            TargetSeed = FindNearestActiveSeed(ActiveSeeds, PlayerLoc);

            // Generate three seeds...
            for (int i = 0; i < 3; i++)
            {
                int[] QuickList = { 2, 3, 4 };
                int Variance = QuickList[Random.Range(0, QuickList.Length)];
                var shrinky = ActiveSeeds[0].GetComponent<TileSeederVoronoi>();

                SowSeeds(TargetSeed, 2f, Variance, 3, 0);
            }
            WorldConstructor(TargetSeed.transform.position, TargetSeed);
        }

    }

    void SowSeedsTest()
    {
        for (int i = 0; i < 6; i++)
        {
            int[] QuickList = { 2, 3, 4 }; // Using a simple array to randomly assign a Moisture value to any generated seeds.
            int Variance = QuickList[Random.Range(0, QuickList.Length)];
            var shrinky = ActiveSeeds[0].GetComponent<TileSeederVoronoi>(); // just used to shorten this long block of text, for sowseedstest we manually tell it that it is the first seed in the active seeds list. the only one in the list.

            SowSeeds(ActiveSeeds[0], 1.5f, Variance, shrinky.Heat, shrinky.Danger); //Function that creates more seeds using the target seed. in this case it is manually defined as our only seed in the world
        }

        WorldConstructor(ActiveSeeds[0].transform.position, ActiveSeeds[0]); //Function that paints a block of tiles around the target seed.
    }

    public void SowSeeds(GameObject TargetSeed, float seedrange, int Heat, int Moisture, int Danger)
    {
        float distance = 0f;
        GameObject Comparitor = null;
        Vector3 SeedLocation = new Vector3(0, 0, 0);
        int retries = 0;

        // Makes sure it is at least 1.5f units away from the nearest seed. Ensures each seed gets sufficient space to breathe. If it takes more than 50 tries to do this, it ditches the entire thing and returns. The assumption is the seed is in a crowded area already.
            while (distance < 1.5f)
            {
                float angle = Random.Range(0f, 360f);
                float x = TargetSeed.transform.position.x + (seedrange * Mathf.Cos(angle / (180f / Mathf.PI)));
                float y = TargetSeed.transform.position.y + (seedrange * Mathf.Sin(angle / (180f / Mathf.PI)));
                SeedLocation = new Vector3(x, y, 0);
                Comparitor = FindNearestSeed(ActiveSeeds, DormantSeeds, SeedLocation);
                distance = DistanceBetween(Comparitor.transform.position, SeedLocation); // Returns the distance between two objects.
                retries++;
                if (retries > 50)
                return;
            }

        // Create a new seed at the decided location. Define all the components used in the script, the tilelist and tilecollidables are pulled from the dictionaries by combining their heat moisture and danger into a string.
        GameObject seed = Instantiate(Seedoid, SeedLocation, Quaternion.AngleAxis(0, Vector3.forward));
        seed.GetComponent<TileSeederVoronoi>().TileList = WorldGenLookup[Heat.ToString() + Moisture.ToString() + Danger.ToString()]["GroundTiles"];
        seed.GetComponent<TileSeederVoronoi>().TileCollidables = WorldGenLookup[Heat.ToString() + Moisture.ToString() + Danger.ToString()]["CollidableTiles"];
        seed.GetComponent<TileSeederVoronoi>().Heat = Heat;
        seed.GetComponent<TileSeederVoronoi>().Moisture = Moisture;
        seed.GetComponent<TileSeederVoronoi>().Danger = Danger;
        ActiveSeeds.Add(seed); // Adds the new seed to active seeds.
    }

    void WorldConstructor(Vector3 OriginPoint, GameObject ActiveSeed)
    {
        Vector3 SubOrigin = new Vector3(OriginPoint.x, OriginPoint.y);
        GameObject DataGetter;
        
        for (int i = -8; i < 9; i++)
        {
            for (int j = -8; j < 9; j++)
            {
                SubOrigin = OriginPoint;
                SubOrigin.x += i * 0.16f;
                SubOrigin.y += j * 0.16f;
                if (LandTileset.GetTile(WorldGrid.WorldToCell(SubOrigin)) == null)
                {
                    DataGetter = FindNearestSeed(ActiveSeeds, DormantSeeds, SubOrigin); // DataGetter becomes the nearest seed, this seed dictates what tileset we can use
                    LandTileset.SetTile((WorldGrid.WorldToCell(SubOrigin)), DataGetter.GetComponent<TileSeederVoronoi>().TileList[Random.Range(0, DataGetter.GetComponent<TileSeederVoronoi>().TileList.Count)]); // Use the component attached to the object on the list which contains tilelist, then randomly grab a tile from the list
                }
            }
        }
        // Remove the targetted seed from ActiveSeeds and onto DormantSeeds list. It will no longer be called on to WorldGen or Sow Seeds, but it can still be checked to pass data onto tiles from dormant seed list
        ActiveSeeds.Remove(ActiveSeed);
        DormantSeeds.Add(ActiveSeed);

    }

    GameObject FindNearestActiveSeed(List<GameObject> FirstList, Vector3 PositionChecker)
    {
        //   List<GameObject> SeedList = new List<GameObject>(); Not in use yet... need to find list of say... five nearest seeds with below
        float SeedRange = Mathf.Infinity;
        GameObject closest = null;
        foreach (GameObject seed in FirstList)
        {
            Vector3 diff = seed.transform.position - PositionChecker;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < SeedRange)
            {
                closest = seed;
                SeedRange = curDistance;
            }
        }
        return closest;
    }

    float DistanceBetween(Vector3 Obj1, Vector3 Obj2)
    {
        Vector3 diff = Obj1 - Obj2;
        float Distance = diff.sqrMagnitude;
        return Distance;
    }

    GameObject FindNearestSeed(List<GameObject> FirstList, List<GameObject> SecondList, Vector3 PositionChecker)
    {
     //   List<GameObject> SeedList = new List<GameObject>(); Not in use yet... need to find list of say... five nearest seeds with below
        float SeedRange = Mathf.Infinity;
        GameObject closest = null;
        foreach(GameObject seed in FirstList)
        {
            Vector3 diff = seed.transform.position - PositionChecker;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < SeedRange)
            {
                closest = seed;
                SeedRange = curDistance;
            }
        }
        foreach (GameObject seed in SecondList)
        {
            Vector3 diff = seed.transform.position - PositionChecker;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < SeedRange)
            {
                closest = seed;
                SeedRange = curDistance;
            }
        }
        return closest;
    }

    void GenerateDictionary(List<TileBase> BaseTiles, List<TileBase> Collidables, int Heat, int Moisture, int Danger)
    {
        Dictionary<string, List<TileBase>> TileDictionary = new Dictionary<string, List<TileBase>>();
        TileDictionary.Add("GroundTiles", BaseTiles);
        TileDictionary.Add("CollidableTiles", Collidables);
        WorldGenLookup.Add(Heat.ToString()+Moisture.ToString()+Danger.ToString(), TileDictionary);
    }

    // old shit can be ignored. will probably get axed later.
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
