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
    public float riseSpeed = 0.4f;
    private AudioSource _audioSource;
    private Grill _grill;
    private float _timer;
    public uint Code { get; private set; }

    public bool drop { get; private set; }


    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _grill = GetComponent<Grill>();
    }

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
        _timer -= Time.fixedDeltaTime;
        if (_timer <= 0 && _audioSource.isPlaying)
        {
            StopSound();
        }

        var temp = transform.position;
        transform.position = Vector3.Lerp(temp, new Vector3(temp.x, drop ? -1f : 0.5f, temp.z),
            drop ? dropSpeed : riseSpeed);
    }

    public void Set(Vector2Int pos, uint code)
    {
        Pos = pos;
        Code = code;
        transform.Rotate(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
    }

    public void PlaySound()
    {
        _timer = _audioSource.clip.length / 3f;
        if (_grill != null)
        {
            _audioSource.clip = _grill.Sound();
            _timer = _audioSource.clip.length / (_grill.IsOn ? 5f : 3f);
        }

        _audioSource.Play();
    }

    public void StopSound()
    {
        _audioSource.Stop();
    }
}