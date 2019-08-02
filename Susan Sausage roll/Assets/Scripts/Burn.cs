using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : MonoBehaviour
{
    public Color t1;
    public Color t2;
    public Color t3;
    private int level = 0;

    public void BurnPiece()
    {
        level++;
        SetLevel();
    }

    public void UndoPiece()
    {
        level--;
        SetLevel();
    }

    public void SetLevel()
    {
        Renderer rend = GetComponent<Renderer>();
        switch (level)
        {
            case 0:
                //Set the main Color of the Material to green
                rend.material.SetColor("_Color",t1);
                break;
            case 1:
                rend.material.SetColor("_Color",t2);
                break;
            case 2:
                rend.material.SetColor("_Color",t3);
                break;
        }
    }
}