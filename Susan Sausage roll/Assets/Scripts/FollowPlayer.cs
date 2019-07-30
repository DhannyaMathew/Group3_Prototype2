using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public float speed;
    public Vector3 offset;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.position - offset, speed);
    }
}