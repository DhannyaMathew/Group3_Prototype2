using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public float range = 3f;
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(Random.Range(-range,range), Random.Range(-range,range), Random.Range(-range,range));   
    }
}
