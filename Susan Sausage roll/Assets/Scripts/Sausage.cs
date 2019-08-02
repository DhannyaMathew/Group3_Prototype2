﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Sausage : MonoBehaviour
{
    public class SausageBurnAction : GameAction
    {
        private Burn burn;

        public SausageBurnAction(Burn burn)
        {
            this.burn = burn;
        }

        protected override bool CanPerform()
        {
            return true;
        }

        protected override void Perform()
        {
            burn.BurnPiece();
        }

        public override void Inverse()
        {
            burn.UndoPiece();
        }
    }

    public class SausageMoveAction : GameAction
    {
        private Sausage _sausage;
        private Vector2Int _dir;

        public SausageMoveAction(Sausage sausage, Vector2Int dir)
        {
            _dir = dir;
            _sausage = sausage;
        }

        protected override bool CanPerform()
        {
            return LevelStart.aLevelStarted != 0;
        }

        public override string ToString()
        {
            return "Sausage roll " + _sausage;
        }

        protected override void Perform()
        {
            var sausage = Level.CheckForSausage(_sausage.b1 + _dir);
            if (sausage != null && sausage != _sausage && _sausage.Code == sausage.Code)
            {
                subActions.Add(new SausageMoveAction(sausage, _dir));
            }

            var otherSausage = sausage;
            sausage = Level.CheckForSausage(_sausage.b2 + _dir);
            if (sausage != null && sausage != otherSausage && sausage != _sausage && _sausage.Code == sausage.Code)
            {
                subActions.Add(new SausageMoveAction(sausage, _dir));
            }

            var diff = _dir - _sausage.Dir;
            if (diff.x != 0 && diff.y != 0)
            {
                _sausage.Flip(_dir);
            }
            else
            {
                _sausage.Move(_dir);
            }

            if (!Level.IsWalkable(_sausage.b1) && !Level.IsWalkable(_sausage.b2))
            {
                _sausage.Fall();
            }

            if (Level.IsGrill(_sausage.b2))
            {
                subActions.Add(new SausageBurnAction((_sausage._flipped ? _sausage.s3 : _sausage.s1).gameObject
                    .GetComponent<Burn>()));
            }
            if (Level.IsGrill(_sausage.b1))
            {
                subActions.Add(new SausageBurnAction((_sausage._flipped ? _sausage.s4 : _sausage.s2).gameObject
                    .GetComponent<Burn>()));
            }
        }

        public override void Inverse()
        {
            if (!Level.IsWalkable(_sausage.b1) && !Level.IsWalkable(_sausage.b2))
            {
                _sausage.Rise();
            }
            var diff = _dir - _sausage.Dir;
            if (diff.x != 0 && diff.y != 0)
            {
                _sausage.Flip(_dir * -1);
            }
            else
            {
                _sausage.Move(_dir * -1);
            }
        }
    }

    public float lerpSpeed = 1.25f;
    private bool _fall;
    private Vector2Int b1;
    private Vector2Int b2;
    private float currentLerpSpeed;

    private Vector3 Position => new Vector3(
        (b1.x + b2.x) / 2f, _fall ? -0.5f : 1.5f, (b1.y + b2.y) / 2f);

    private float angle = 0;
    private float currentAngle = 0;
    private bool _sinking;
    private bool _flipped;
    public float fallSpeed = 0.05f;
    public GameObject s1;
    public GameObject s2;
    public GameObject s3;
    public GameObject s4;
    public uint Code { get; private set; }
    public Vector2Int Dir => b2 - b1;

    public Vector3 axis
    {
        get
        {
            if (b2.x - b1.x < 0 || b2.y - b1.y < 0)
            {
                return Vector3.back;
            }
            return Vector3.forward;
        }
    }

    private void Start()
    {
        currentLerpSpeed = lerpSpeed;
    }

    private void Move(Vector2Int dir)
    {
        b1 += dir;
        b2 += dir;
    }

    private void Flip(Vector2Int dir)
    {
        _flipped = !_flipped;
        Move(dir);
        if (dir.x > 0 || dir.y < 0)
        {
            angle += 180;
        }
        else if (dir.x < 0 || dir.y > 0)
        {
            angle -= 180;
        }
    }
    
    public void Set(Vector2Int b1, Vector2Int b2, uint code)
    {
        Code = code;
        if (b1.x > b2.x)
        {
            var temp = b1.x;
            b1.x = b2.x;
            b2.x = temp;
        }

        if (b1.y > b2.y)
        {
            var temp = b1.y;
            b1.y = b2.y;
            b2.y = temp;
        }

        this.b1 = b1;
        this.b2 = b2;
        transform.position = Position;
        transform.rotation = Quaternion.LookRotation(new Vector3(b2.x - b1.x, 0, b2.y - b1.y));
    }

    public bool Contains(Vector2Int coord)
    {
        return coord.Equals(b1) || coord.Equals(b2);
    }

    private void FixedUpdate()
    {
        var pos = Position;
        transform.position =
            Vector3.Lerp(
                transform.position,
                pos,
                currentLerpSpeed);
        var temp = currentAngle;
        currentAngle = Mathf.Lerp(currentAngle, angle, currentLerpSpeed);
        if (Mathf.Abs(currentAngle - angle) > 0.1f)
        {
            transform.Rotate(axis, temp - currentAngle);
        }
    }

    public void Fall()
    {
        _fall = true;
        currentLerpSpeed = fallSpeed;
    }

    public void Rise()
    {
        _fall = false;
        currentLerpSpeed = lerpSpeed;
    }
}