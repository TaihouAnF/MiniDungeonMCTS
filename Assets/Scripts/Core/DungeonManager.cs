using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public bool paused = false;

    public GameObject CoreCanvas;
    public GameObject FinalCanvas;
    public GameObject HealthText;
    public GameObject ScoreText;
    public GameObject EnemyKilledText;

    [Tooltip("Final infor section")]
    public int FinalHealth;
    public int FinalScore;
    public int FinalKilled;

    public int Width { get; private set; }
    public int Height { get; private set;}

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
        EventManager.OnPlayerMoved += ChangeTile;
        EventManager.OnLevelRestart += GameWin;
        EventManager.OnUIChanged += ChangeUI;
    }

    void OnDestroy() 
    {
        EventManager.OnPlayerMoved -= ChangeTile;
        EventManager.OnLevelRestart -= GameWin;
        EventManager.OnUIChanged -= ChangeUI;
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
        FinalCanvas.SetActive(false);
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
        yield return new WaitForSeconds(0.1f);
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

    private void GameWin() 
    {
        CoreCanvas.SetActive(false);
        FinalCanvas.SetActive(true);

        var HealthTxt = FinalCanvas.transform.Find("HealthTxt").GetComponent<TextMeshProUGUI>();
        var ScoreTxt = FinalCanvas.transform.Find("ScoreTxt").GetComponent<TextMeshProUGUI>();
        var EnemyTxt = FinalCanvas.transform.Find("EnemyTxt").GetComponent<TextMeshProUGUI>();
        HealthTxt.text = "Health: " + FinalHealth;
        ScoreTxt.text = "Score: " + FinalScore;
        EnemyTxt.text = "Killed: " + FinalKilled;
        paused = true;
    }

    private void ChangeUI() 
    {
        var player = FindObjectOfType<PlayerController>();
        HealthText.GetComponent<TextMeshProUGUI>().text = "Health: " + player.Health;
        ScoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + player.Score;
        EnemyKilledText.GetComponent<TextMeshProUGUI>().text = "Killed: " + player.EnemyKilled;
    }

    public void ResetLevel() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ClearMap() {
        HealthText.GetComponent<TextMeshProUGUI>().text = "Health: 100";
        ScoreText.GetComponent<TextMeshProUGUI>().text = "Score: 0";
        EnemyKilledText.GetComponent<TextMeshProUGUI>().text = "Killed: 0";
    }

    /// <summary>
    /// Debug purpose
    /// </summary>
    public void PrintMap()
    {
        for (int y = mp.GetLength(1) - 1; y >= 0; y--)
        {
            string row = "";
            for (int x = 0; x < mp.GetLength(0); x++)
            {
                row += mp[x, y].ToString() + " ";
            }
            Debug.Log(row);
        }
    }
}
