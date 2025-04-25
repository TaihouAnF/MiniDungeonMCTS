using TMPro;
using UnityEngine;

/// <summary>
/// Only Controls generating the map/level, and Update the sprite, 
/// it won't control any other logics
/// </summary>
public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance { get; private set;}
    public TextAsset levelText; // Debug purpose, deprecrated

    public TMP_InputField inputField;
    public MapTile tilePrefab;
    public Transform gridRoot;
    public MapTile [,] map;
    public TileSpriteSet tileSpriteSet;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }

        EventManager.OnPlayerMoved += UpdateLevel;
        EventManager.OnLevelRestart += GameWin;
    }

    void OnDestroy()
    {
        EventManager.OnPlayerMoved -= UpdateLevel;
        EventManager.OnLevelRestart -= GameWin;
    }
    
    public void LoadLevel(string level)
    {
        string[] lines = level.Split('\n');
        int height = lines.Length;
        int width = lines[^1].Length;   // index operator for getting the last

        DungeonManager.Instance.InitMap(height, width);
        map = new MapTile[height, width];

        for (int y = 0; y < height; ++y) {
            string line = lines[lines.Length - 1 - y];  // Inverted Y so top row == top line
            for (int x = 0; x < width; ++x) {
                
                char c = line[x];
                Vector3 localPos = new(x, y, 0);
                Transform parent = gridRoot;
                TileType type = TileType.Empty;
                MapTile tile = Instantiate(tilePrefab, localPos, Quaternion.identity, parent); 

                switch (c)
                {
                    case '#': type = TileType.Wall; break;
                    case '@': 
                        type = TileType.Player; 
                        PlayerController pc = tile.gameObject.AddComponent<PlayerController>();
                        Vector2Int pos = new(x, y);
                        pc.InitializeActor(pos, type, 100);
                        break;
                    case 'M': type = TileType.Enemy; break;
                    case 'P': type = TileType.Potion; break;
                    case 'E': type = TileType.Exit; break;
                    case 'C': type = TileType.Chest; break;
                    case '.': type = TileType.Floor; break;
                }
                tile.SetSprite(tileSpriteSet.GetSprite(type));
                map[x, y] = tile;
                DungeonManager.Instance.SetTile(x, y, type);
            }
        }
        // center the grid
        CenterDungeon(width, height);
    }

    private void CenterDungeon(int width, int height)
    {
        Debug.Log("Called");
        gridRoot.position = new Vector3(-width / 2f + 0.5f, -height / 2f + 0.5f);
    }

    /// <summary>
    /// Update the level by changing sprites only, it is not going to redraw entire grid
    /// This should be called by a event, TODO make it trigger by event
    /// </summary>
    /// <param name="actor">The actor(player/enemy) which stores information about previou tile and curr tile</param>
    public void UpdateLevel(Actor actor)  
    {
        int x = actor.PrevPos.x;
        int y = actor.PrevPos.y;
        map[x, y].SetSprite(tileSpriteSet.GetSprite(actor.PrevTileType));
        int p_x = actor.CurPos.x;
        int p_y = actor.CurPos.y;
        map[p_x, p_y].SetSprite(tileSpriteSet.GetSprite(actor.ActorType));
    }

    public void GameWin() 
    {
        gridRoot.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Loads level file from path "Resources/Levels/<filename>.txt". 
    /// Only need to place the file in the level folder and input its name
    /// WARNING: this loads the level with clearing the old one, please consider using 
    /// LoadNextLevel(string name) if you want complete loading
    /// </summary>
    /// <param name="filename">The name of txt</param>
    public static void LoadAssetfromPath(string filename)
    {
        string name = "Levels/" + filename;
        TextAsset level = Resources.Load<TextAsset>(name);

        if (level == null) 
        {
            Debug.LogError($"Level '{level}' not found in Resources/Levels/");
            return;
        }
        Instance.LoadLevel(level.text);
    }

    public void ClearMap() 
    {
        gridRoot.transform.position = new Vector3(0, 0, 0);
        if (map == null || map.Length == 0) return;
        for (int i = 0; i < map.GetLength(0); ++i) 
        {
            for (int j = 0; j < map.GetLength(1); ++j) 
            {
                GameObject.Destroy(map[i, j].gameObject);
                map[i, j] = null;
            }
        }
    }

    /// <summary>
    /// Wraps the entire loading process and load the level
    /// </summary>
    /// <param name="name">The level name</param>
    public void LoadNextLevel(string name) 
    {
        DungeonManager.Instance.ClearMap();
        ClearMap();
        LoadAssetfromPath(name);
    }

    public void LoadLevelFromInput()
    {
        string name = inputField.text.Trim();
        if (string.IsNullOrEmpty(name)) return;

        LoadNextLevel(name);
    }
}
