using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{
    Vector3 mousePos;

    public float movespeed = 5f;

    protected void FixedUpdate()
    {

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector3 lookDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;

        if (angle <= 0 && angle >= -180)
            transform.localScale = new Vector3(1, 1, 1);
        else if (angle < -180 || angle > 0)
            transform.localScale = new Vector3(-1, 1, 1);

        UpdateMotor(new Vector3(x, y, 0));
    }
}
