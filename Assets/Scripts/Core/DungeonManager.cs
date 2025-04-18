using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum gameTurn 
{
    playerTurn,
    enemyTurn,
}
/// <summary>
/// Act as a game manager
/// </summary>
public class DungeonManager : MonoBehaviour // Could make a monosingleton here but for the simplicity we are not going to make it
{
    public static DungeonManager Instance { get; private set;}  
    public TileType[,] mp;

    public gameTurn curTurn;

    public int Width { get; private set; }
    public int Height { get; private set;}

    void Awake()
    {
        Instance = this;
        EventManager.OnPlayerMoved += ChangeTile;
        EventManager.OnLevelRestart += RestartTheLevel;
    }

    void OnDestroy() 
    {
        EventManager.OnPlayerMoved -= ChangeTile;
        EventManager.OnLevelRestart -= RestartTheLevel;
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
        curTurn = gameTurn.playerTurn;
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

    public bool IsWalkable(Vector2Int pos) {
        return mp[pos.x, pos.y] != TileType.Wall 
            && mp[pos.x, pos.y] != TileType.Empty;
    }

    public bool IsInBound(Vector2Int pos) {
        return pos.x >= 0 && pos.y >= 0 
            && pos.x < Width && pos.y < Height;
    }

    public void EndPlayerTurn() 
    {
        if (curTurn != gameTurn.playerTurn) return;
        curTurn = gameTurn.enemyTurn;
        StartCoroutine(RunEnemyTurn());
    }

    IEnumerator RunEnemyTurn() {
        // TODO Enemy behavior
        yield return new WaitForSeconds(0.5f);
        curTurn = gameTurn.playerTurn;
    }

    private void ChangeTile(Actor actor) 
    {
        Vector2Int prev = actor.PrevPos;
        Vector2Int cur = actor.CurPos;
        TileType preType = actor.PrevTileType;

        mp[cur.x, cur.y] = actor.ActorType;
        mp[prev.x, prev.y] = preType;   // player might consume item
    }

    private void RestartTheLevel() 
    {
        
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
