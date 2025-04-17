using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour // Could make a monosingleton here but for the simplicity we are not going to make it
{
    public static DungeonManager Instance { get; private set;}  
    public TileType[,] mp;

    public int Width { get; private set; }
    public int Height { get; private set;}

    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitMap(int width, int height) 
    {
        Width = width;
        Height = height;
        mp = new TileType[Width, Height];
    }

    public TileType GetTile(int x, int y) 
    {
        return x < 0 || y < 0 || x >= Width || y >= Height ? TileType.Empty : mp[x, y];
    }

    public void SetTile(int x, int y, TileType tile) 
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height) return;
        mp[x, y] = tile;
    }

    public bool isWalkable(Vector2Int pos) {
        return mp[pos.x, pos.y] != TileType.Enemy 
            && mp[pos.x, pos.y] != TileType.Wall 
            && mp[pos.x, pos.y] != TileType.Empty;
    }
}
