using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public Vector2Int MovePos;
    public MoveAction(Vector2Int movePos)
    {
        MovePos = movePos;
    }
    public override string ToString()
    {
        return string.Format("({0}, {1})",MovePos.x,MovePos.y);
    }
}
