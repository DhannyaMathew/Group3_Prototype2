using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStart : MonoBehaviour
{
    public static uint aLevelStarted = 0;

    public class EndLevel : GameAction
    {
        private LevelStart ls;

        public EndLevel(uint code)
        {
            ls = Level.GetLevelStarter(code);
        }

        protected override bool CanPerform()
        {
            return !ls.Active;
        }

        protected override void Perform()
        {
            ls.Activate();
        }

        public override void Inverse()
        {
            ls.Deactivate();
        }
    }

    public class PlayerEnterAction : GameAction
    {
        private LevelStart ls;

        public PlayerEnterAction(LevelStart ls)
        {
            this.ls = ls;
        }

        protected override bool CanPerform()
        {
            return ls.Active;
        }

        protected override void Perform()
        {
            if (aLevelStarted == 0)
            {
                Level.DropAllExcludingMask(ls.Code);
                aLevelStarted = ls.Code;
            }
            else
            {
                Level.CompleteLevel(ls.Code);
                Level.RiseAll();
                ls.Deactivate();
                aLevelStarted = 0;
            }
        }

        public override void Inverse()
        {
            if (aLevelStarted == 0)
            {
                Level.DropAllExcludingMask(ls.Code);
                aLevelStarted = ls.Code;
            }
            else
            {
                Level.RiseAll();
                aLevelStarted = 0;
            }
        }
    }

    private Vector2Int _b1;
    private Vector2Int _b2;
    public uint Code { get; private set; }
    public bool Active { get; private set; }

    public bool Contains(Vector2Int coord, Vector2Int dir)
    {
        return coord.Equals(_b1) && (coord + dir).Equals(_b2);
    }

    private Vector3 Position => new Vector3(
        (_b1.x + _b2.x) / 2f, 1.5f, (_b1.y + _b2.y) / 2f);

    public void Set(Vector2Int b1, Vector2Int b2, uint maskCode)
    {
        Active = true;
        _b1 = b1;
        _b2 = b2;
        Code = maskCode;
        transform.position = Position;
        transform.rotation = Quaternion.LookRotation(new Vector3(b2.x - b1.x, 0, b2.y - b1.y));
    }

    public void Activate()
    {
        if (!Level.LevelCompleted(Code))
        {
            Active = true;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void Deactivate()
    {
        Active = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}