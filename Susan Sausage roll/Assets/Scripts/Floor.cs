using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour
{
    public float range = 3f;
    public Vector2Int Pos { get; private set; }
    public float dropSpeed;
    public uint Code { get; private set; }

    public bool drop { get; private set; }

    public void Drop()
    {
        drop = true;
    }

    public void Rise()
    {
        drop = false;
    }

    private void FixedUpdate()
    {
        var temp = transform.position;
        transform.position = Vector3.Lerp(temp, new Vector3(temp.x, drop ? -1f : 0.5f, temp.z), dropSpeed);
    }

    public void Set(Vector2Int pos, uint code)
    {
        Pos = pos;
        Code = code;
        transform.Rotate(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
    }
}