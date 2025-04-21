using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class BaseGameState
{
    public abstract int GameResult();
    public abstract bool IsGameOver();
    public abstract List<MoveAction> GetLegalActions();

}
