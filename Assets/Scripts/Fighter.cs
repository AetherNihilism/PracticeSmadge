using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    // public fields
    public int hitpoints = 10;
    public int maxHitpoints = 10;
    public float pushRecoverySpeed = 0.2f;

    // Immunity

    // Push
    protected Vector3 pushDirection;

    // All fighters can take damage and die

    protected virtual void Death()
    {

    }
}
