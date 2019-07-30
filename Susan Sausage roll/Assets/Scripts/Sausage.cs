using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sausage : MonoBehaviour
{
    private Vector2Int b1;
    private Vector2Int b2;
    private bool _flipped = false;
    public void Set(Vector2Int b1, Vector2Int b2)
    {
        this.b1 = b1;
        this.b2 = b2;
        transform.position = new Vector3(
            (b1.x + b2.x) / 2f, 1f, (b1.y + b2.y) / 2f
        );
        transform.rotation = Quaternion.LookRotation(new Vector3(b2.x - b1.x, 0, b2.y - b1.y));
    }
}