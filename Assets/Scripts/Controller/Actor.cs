using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public Vector2Int PrevPos;  // previous location
    public Vector2Int CurPos;   // current location
    public TileType actorType;  // The actor type, could be player/enemy
    public TileType prevTileType;   // Stores the type of tile previously was
    public float health;        // Each actor has health
    public void InitializeActor(Vector2Int pos, TileType aType, float startHealth) 
    {
        PrevPos = pos;
        CurPos = pos;
        actorType = aType;
        prevTileType = TileType.Floor;
        health = startHealth;
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
