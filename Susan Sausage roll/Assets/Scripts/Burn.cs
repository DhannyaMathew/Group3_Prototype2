using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : MonoBehaviour
{
    public Color t1;
    public Color t2;
    public Color t3;
    public GameObject cookEffect;
    public GameObject burnEffect;
    private int level = 0;

    public void BurnPiece()
    {
        level++;
        level = Mathf.Clamp(level, 0, 2);
        SetLevel();
    }

    public void UndoPiece()
    {
        level--;
        level = Mathf.Clamp(level, 0, 2);
        SetLevel();
    }

    public void SetLevel()
    {
        Renderer rend = GetComponent<Renderer>();
        switch (level)
        {
            case 0:
                //Set the main Color of the Material to green
                rend.material.SetColor("_Color", t1);
                break;
            case 1:
                Instantiate(cookEffect, GetComponentInParent<Sausage>().Position + Vector3.up * 0.5f,
                    Quaternion.identity);
                rend.material.SetColor("_Color", t2);
                break;
            case 2:
                Instantiate(burnEffect, GetComponentInParent<Sausage>().Position + Vector3.up * 0.5f,
                    Quaternion.identity);
                rend.material.SetColor("_Color", t3);
                break;
        }
    }

    public bool Raw()
    {
        return level == 0;
    }

    public bool Cooked()
    {
        return level == 1;
    }

    public bool Burnt()
    {
        return level == 2;
    }
}