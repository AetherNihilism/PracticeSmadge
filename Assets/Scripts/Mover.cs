using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : Fighter
{
    protected BoxCollider2D boxCollider;
    protected Vector3 moveDelta;
    protected RaycastHit2D hit;
    protected float ySpeed = 1.0f;
    protected float xSpeed = 1.0f;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected virtual void UpdateMotor(Vector3 input)
    {

        // Reset moveDelta
        moveDelta = new Vector3(input.x * xSpeed, input.y * ySpeed, 0);

        // Swap sprite direction

        if (moveDelta.x > 0 && transform.name != "Player")
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveDelta.x < 0 && transform.name != "Player")
            transform.localScale = new Vector3(-1, 1, 1);

        // Add push vector, if any
        moveDelta += pushDirection;

        // Reduce push force every frame, based off recovery speed
            pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverySpeed);

        // Make sure we can move in this direction, by casting a box in there first, if the box returns null, we're free to move
                hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("NPC", "Blocked"));
                if (hit.collider == null)
                {
                    transform.Translate(0, moveDelta.y * Time.deltaTime, 0); //equalizes time across devices
                }

                hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("NPC", "Blocked"));
                if (hit.collider == null)
                {
                    transform.Translate(moveDelta.x * Time.deltaTime, 0, 0); //equalizes time across devices
                }
        // Make this thing move!

    }
}
