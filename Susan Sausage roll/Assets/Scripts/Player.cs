﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private class RotateAction : GameAction
    {
        private readonly Player _player;
        private readonly Vector2Int _change;

        public RotateAction(Player player, Vector2Int change)
        {
            _player = player;
            _change = change;
        }

        protected override bool CanPerform()
        {
            if (LevelStart.aLevelStarted == 0)
            {
                var prevDir = _player.Direction;
                _player.Direction += _change;
                var sausage = Level.CheckForSausage(_player.Position + prevDir + _player.Direction);
                if (sausage != null)
                {
                    _player.Direction -= _change;
                    return false;
                }

                var temp = sausage;
                sausage = Level.CheckForSausage(_player.Position + _player.Direction);
                if (sausage != temp && sausage != null)
                {
                    _player.Direction -= _change;
                    return false;
                }

                _player.Direction -= _change;
            }

            return true;
        }

        protected override void Perform()
        {
            var prevDir = _player.Direction;
            _player.Direction += _change;
            _player.PlayWhoosh();
            var sausage = Level.CheckForSausage(_player.Position + prevDir + _player.Direction);
            if (sausage != null && sausage.Code == LevelStart.aLevelStarted)
            {
                subActions.Add(new Sausage.SausageMoveAction(sausage, _player.Direction));
            }

            var temp = sausage;
            sausage = Level.CheckForSausage(_player.Position + _player.Direction);
            if (sausage != temp && sausage != null && sausage.Code == LevelStart.aLevelStarted)
            {
                subActions.Add(new Sausage.SausageMoveAction(sausage, prevDir * -1));
            }

            var level = Level.CheckForLevelStart(_player.Position, _player.Direction);
            if (level != null)
            {
                subActions.Add(new LevelStart.PlayerEnterAction(level));
            }

            Actions++;
        }

        public override void Inverse()
        {
            _player.Direction -= _change;
            Actions--;
        }

        public override string ToString()
        {
            return "Player Rotate";
        }
    }

    protected class MoveAction : GameAction
    {
        private readonly Player _player;
        private readonly bool _forward;

        public MoveAction(Player player, bool forward)
        {
            _forward = forward;
            _player = player;
        }


        protected override bool CanPerform()
        {
            if (LevelStart.aLevelStarted == 0)
            {
                if (_forward)
                {
                    var sausage = Level.CheckForSausage(_player.Position + _player.Direction * 2);
                    if (sausage != null)
                    {
                        return false;
                    }
                }
                else
                {
                    var sausage = Level.CheckForSausage(_player.Position - _player.Direction);
                    if (sausage != null)
                    {
                        return false;
                    }
                }
            }

            var val = Level.IsWalkable(_player.Position + _player.Direction * (_forward ? 1 : -1));
            return val;
        }

        public override string ToString()
        {
            return "Player Move";
        }

        protected override void Perform()
        {
            if (_forward)
            {
                var sausage = Level.CheckForSausage(_player.Position + _player.Direction * 2);
                if (sausage != null && sausage.Code == LevelStart.aLevelStarted)
                {
                    subActions.Add(new Sausage.SausageMoveAction(sausage, _player.Direction));
                }
            }
            else
            {
                var sausage = Level.CheckForSausage(_player.Position - _player.Direction);
                if (sausage != null && sausage.Code == LevelStart.aLevelStarted)
                {
                    subActions.Add(new Sausage.SausageMoveAction(sausage, _player.Direction * -1));
                }
            }

            _player.Move(_forward);
            var level = Level.CheckForLevelStart(_player.Position, _player.Direction);
            if (level != null)
            {
                subActions.Add(new LevelStart.PlayerEnterAction(level));
            }

            Actions++;
        }

        public override void Inverse()
        {
            _player.Move(!_forward, true);
            Actions--;
        }
    }

    private void PlayWhoosh()
    {
        _audioSource.PlayOneShot(whoosh, 0.5f);
    }

    private void GetBurnt(bool inverse, bool isUndo)
    {
        _onGrill = true;
        _inverse = inverse;
        _isUndo = isUndo;
        _audioSource.PlayOneShot(burnt);
    }

    public float moveDelay = 0.25f;
    public float lerpSpeed;
    public AudioClip burnt;
    public AudioClip whoosh;
    public static int Actions;
    private AudioSource _audioSource;

    public Vector2Int Position { get; private set; }
    public Vector2Int Direction { get; private set; }
    private float _timer;
    private bool _onGrill = false;
    private bool _inverse = false;
    private bool _isUndo = false;
    private Animator _animator;
    private static readonly int Walk = Animator.StringToHash("Direction");

    private void Start()
    {
        Direction = Vector2Int.up;
        Camera.main.gameObject.GetComponent<FollowPlayer>().player = transform;
        Position = Level.GetPlayerSpawn();
        _animator = GetComponentInChildren<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_timer <= 0)
        {
            if (Input.GetButton("Right"))
                CalculateMove(Vector2Int.right);
            else if (Input.GetButton("Left"))
                CalculateMove(Vector2Int.left);
            else if (Input.GetButton("Up"))
                CalculateMove(Vector2Int.up);
            else if (Input.GetButton("Down"))
                CalculateMove(Vector2Int.down);
        }
        else
            _timer -= Time.deltaTime;
    }

    private void CalculateMove(Vector2Int input)
    {
        var diff = input - Direction;
        if (diff.Equals(Vector2Int.zero))
            new MoveAction(this, true).Execute();
        else if (diff.x != 0 && diff.y != 0)
            new RotateAction(this, diff).Execute();
        else
            new MoveAction(this, false).Execute();
        _timer = moveDelay;
    }

    public void Move(bool forward, bool isUndo = false)
    {
        Position += Direction * (forward ? 1 : -1);
        Level.GetFloor(Position).PlaySound();
        _animator.SetFloat(Walk, forward ? 1f : -1f);
        if (Level.IsGrill(Position))
        {
            GetBurnt(!forward, isUndo);
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var pos = new Vector3(Position.x, transform.position.y, Position.y);
        if ((transform.position - pos).magnitude > 0.05f)
        {
            transform.position =
                Vector3.Lerp(
                    transform.position,
                    pos,
                    lerpSpeed);
            if (_onGrill)
            {
                if ((transform.position - pos).sqrMagnitude < 0.1f * 0.1f)
                {
                    _onGrill = false;
                    if (_isUndo)
                    {
                        Level.Undo();
                    }
                    else
                    {
                        new MoveAction(this, _inverse).Execute();
                    }
                }
            }
        }
        else
        {
            _animator.SetFloat(Walk, 0f);
        }

        var rot = Quaternion.LookRotation(new Vector3(Direction.x, 0, Direction.y));
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, lerpSpeed);
    }
}