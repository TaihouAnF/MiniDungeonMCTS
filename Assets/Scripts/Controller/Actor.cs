using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public Vector2Int PrevPos;  // previous location, act as curr location
    public Vector2Int CurPos;   // current location after move, act as a target
    public TileType actorType;  // The actor type, could be player/enemy
    public TileType prevTileType;   // Stores the type of tile previously was

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void TryMove(Vector2Int pos) 
    {
        // should implement their own
    }
}
