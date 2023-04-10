using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSeederVoronoi : MonoBehaviour
{

    public Grid WorldGrid;
    public List<TileBase> TileList;
    public List<TileBase> TileCollidables;
    private Tilemap tilemap;
    private Tilemap Collidables;
    public int Heat;
    public int Moisture;
    public int Danger;

    private Vector3Int cellPosition;

    // All this script does is store info and position the seed appropriately on the Grid for TileManager to use later.
    void Start()
    {
        tilemap = GameObject.Find("Land").GetComponent<Tilemap>();
        Collidables = GameObject.Find("Collidables").GetComponent<Tilemap>();

        cellPosition = WorldGrid.WorldToCell(transform.position);
        transform.position = WorldGrid.GetCellCenterWorld(cellPosition);

    }

}
