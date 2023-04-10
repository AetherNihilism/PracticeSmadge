using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public Transform PlayerLocation;
    public GameObject TileCrafter;

    private float Rotation = 0f;

    void Start()
    {
        //    InvokeRepeating("WorldCreation", 2.0f, 900.0f);
        Invoke("Starter", 5.0f);
        InvokeRepeating("ContiniousCreation", 8.0f, 3.0f);
    }

    void ContiniousCreation()
    {
        TileManager.instance.WorldBuildTest(PlayerLocation);
    }

    void WorldCreation()
    {
        Rotation = Mathf.Atan2(1, 1);
        GameObject brush = Instantiate(TileCrafter, PlayerLocation.position, Quaternion.AngleAxis(Rotation, Vector3.forward));
    }

    void Starter()
    {
        TileManager.instance.WorldStart(TileCrafter, PlayerLocation, 0f);
    }
}

/*    private void FireShot1()
{

    for (int i = -1; i < 2; i++)
    {
        ShotRotation = Mathf.Atan2(playerTransform.position.y - shotLocation.position.y, playerTransform.position.x - shotLocation.position.x) * Mathf.Rad2Deg + i * 40;
        GameObject shot = Instantiate(shotGraphic, shotLocation.position, Quaternion.AngleAxis(ShotRotation, Vector3.forward));
        Rigidbody2D rb = shot.GetComponent<Rigidbody2D>();
        rb.AddRelativeForce(shotLocation.right * shotSpeed, ForceMode2D.Impulse);
    }
}*/