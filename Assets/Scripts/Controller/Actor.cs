using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public Vector2Int PrevPos;  // previous location
    public Vector2Int CurPos;   // current location
    public TileType ActorType;  // The actor type, could be player/enemy
    public TileType PrevTileType;   // Stores the type of tile previously was
    public float Health;        // Each actor has health
    public void InitializeActor(Vector2Int pos, TileType aType, float startHealth) 
    {
        PrevPos = pos;
        CurPos = pos;
        ActorType = aType;
        PrevTileType = TileType.Floor;
        Health = startHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual bool TryMove(Vector2Int pos) 
    {
        // should implement their own
        return false;
    }
}
