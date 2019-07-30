using System;
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
            return true;
        }

        protected override void Perform()
        {
            _player._direction += _change;
        }

        public override void Inverse()
        {
            _player._direction -= _change;
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
            var val = Level.IsWalkable(_player._position + _player._direction * (_forward ? 1 : -1));
            return val;
        }

        protected override void Perform()
        {
            if (_forward)
            {
                var sausage = Level.CheckForSausage(_player._position + _player._direction * 2);
                if (sausage != null)
                {
                    subActions.Add(new Sausage.SausageMoveAction(sausage, _player._direction));
                }
            }
            else
            {
                var sausage = Level.CheckForSausage(_player._position - _player._direction);
                if (sausage != null)
                {
                    subActions.Add(new Sausage.SausageMoveAction(sausage, _player._direction * -1));
                }
            }

            _player.Move(_forward);
        }

        public override void Inverse()
        {
            _player.Move(!_forward, true);
        }
    }

    private void GetBurnt(bool inverse, bool isUndo)
    {
        _onGrill = true;
        _inverse = inverse;
        _isUndo = isUndo;
    }

    public float moveDelay = 0.25f;
    public float lerpSpeed;

    private Vector2Int _position;
    private Vector2Int _direction;
    private float _timer;
    private bool _onGrill = false;
    private bool _inverse = false;
    private bool _isUndo = false;

    private void Start()
    {
        _direction = Vector2Int.up;
        Camera.main.gameObject.GetComponent<FollowPlayer>().player = transform;
        _position = Level.GetPlayerSpawn();
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
        var diff = input - _direction;
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
        _position += _direction * (forward ? 1 : -1);
        if (Level.IsGrill(_position))
        {
            GetBurnt(!forward, isUndo);
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var pos = new Vector3(_position.x, transform.position.y, _position.y);
        if (!transform.position.Equals(pos))
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

        var rot = Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.y));
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, lerpSpeed);
    }
}