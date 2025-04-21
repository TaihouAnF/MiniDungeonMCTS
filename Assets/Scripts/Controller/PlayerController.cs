using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : Actor
{
    public int Score;
    public int EnemyKilled;

    // Update is called once per frame
    void Update()
    {
        if (DungeonManager.Instance.curTurn != gameTurn.playerTurn || DungeonManager.Instance.paused) return;

        // Debug.Log("Player Moving");
        Vector2Int dir = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.W)) dir = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.A)) dir = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.D)) dir = Vector2Int.right;

        if (dir == Vector2Int.zero) return;

        bool valid = TryMove(CurPos + dir);
        // Only end turn when it is a valid move/attack
        if (valid) DungeonManager.Instance.EndPlayerTurn(); 
    }

    /// <summary>
    /// Try move/attack to new pos. It handles the logic of the tile
    /// and the player: picking up item on the route, move to new pos,
    /// and attack which won't change the actor pos.
    /// </summary>
    /// <param name="pos">the new position, AKA target pos</param>
    /// <returns>True if the move is done, False otherwise</returns>
    public override bool TryMove(Vector2Int pos) {
        var dm = DungeonManager.Instance;
        // Extra guarding, forgive my illness
        if (dm.curTurn != gameTurn.playerTurn) return false;

        if (!dm.IsWalkable(pos) || !dm.IsInBound(pos)) return false;

        TileType nxtType = dm.GetTile(pos.x, pos.y);
        if (nxtType != TileType.Empty && nxtType != TileType.Exit) 
        {
            CurPos = pos;
            EventManager.TriggerLevelUpdate(this);

            PrevPos = CurPos;
            PrevTileType = (nxtType != TileType.Floor) ? TileType.Floor : nxtType;
            Score += nxtType == TileType.Chest ? 10 : 0;
            Health += nxtType == TileType.Potion ? 10 : 0;
            EnemyKilled += nxtType == TileType.Enemy ? 1 : 0;
            Health -= nxtType == TileType.Enemy ? 10 : 0;
            EventManager.TriggerUIChanged();
            return true;
        }
        else if (nxtType == TileType.Exit) 
        {
            CurPos = pos;
            EventManager.TriggerLevelUpdate(this);
            PrevPos = CurPos;
            PrevTileType = nxtType;
            DungeonManager.Instance.FinalHealth = Health;
            DungeonManager.Instance.FinalScore = Score;
            DungeonManager.Instance.FinalKilled = EnemyKilled;
            EventManager.TriggerLevelRestart(); // Currently restart the level
            return true;
        }
        return false;
    }
}
