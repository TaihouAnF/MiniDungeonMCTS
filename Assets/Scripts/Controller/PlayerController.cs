using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerController : Actor
{
    public int Score;

    // Update is called once per frame
    void Update()
    {
        if (DungeonManager.Instance.curTurn != gameTurn.playerTurn) return;

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
    protected override bool TryMove(Vector2Int pos) {
        var dm = DungeonManager.Instance;
        // Extra guarding, forgive my illness
        if (dm.curTurn != gameTurn.playerTurn) return false;

        if (!dm.IsWalkable(pos) || !dm.IsInBound(pos)) return false;

        TileType nxtType = dm.GetTile(pos.x, pos.y);
        Debug.Log("Player Moving");
        if (nxtType == TileType.Enemy) 
        {
            // Do Attack
            return true;
        }
        else if (nxtType == TileType.Floor || nxtType == TileType.Potion || nxtType == TileType.Chest) 
        {
            CurPos = pos;
            EventManager.TriggerLevelUpdate(this);
            PrevPos = CurPos;
            prevTileType = nxtType;
            return true;
        }
        return false;
    }
}
