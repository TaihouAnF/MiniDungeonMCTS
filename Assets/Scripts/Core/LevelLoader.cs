using UnityEngine;
public class LevelLoader : MonoBehaviour
{
    public TextAsset levelText;
    public MapTile tilePrefab;
    public Transform gridRoot;
    public MapTile [,] map;
    public TileSpriteSet tileSpriteSet;
    
    // Start is called before the first frame update
    void Start()
    {
        LoadLevel(levelText.text);
    }

    private void LoadLevel(string level)
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
                    case '@': type = TileType.Player; break;
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
        map[x, y].SetSprite(tileSpriteSet.GetSprite(actor.prevTileType));
        int p_x = actor.CurPos.x;
        int p_y = actor.CurPos.y;
        map[p_x, p_y].SetSprite(tileSpriteSet.GetSprite(actor.actorType));
    }
}
