using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStart : MonoBehaviour
{
    public class StartLevel : GameAction
    {
        private uint code;

        public StartLevel(uint code)
        {
            this.code = code;
        }

        protected override bool CanPerform()
        {
            return true;
        }

        protected override void Perform()
        {
            Level.DropAllExcludingMask(code);
        }

        public override void Inverse()
        {
            Level.RiseAll();
        }
    }
    
    private Vector2Int _b1;
    private Vector2Int _b2;
    private uint _maskCode;

    private Vector3 Position => new Vector3(
        (_b1.x + _b2.x) / 2f, 1.5f, (_b1.y + _b2.y) / 2f);

    public void Set(Vector2Int b1, Vector2Int b2, uint maskCode)
    {
        _b1 = b1;
        _b2 = b2;
        _maskCode = maskCode;
        transform.position = Position;
        transform.rotation = Quaternion.LookRotation(new Vector3(b2.x - b1.x, 0, b2.y - b1.y));
    }
}