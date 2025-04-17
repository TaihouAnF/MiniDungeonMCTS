using UnityEngine;

[CreateAssetMenu(fileName = "TileSpriteSet", menuName = "Tilemap/TileSpriteSet")]
public class TileSpriteSet : ScriptableObject
{
    public Sprite floorSprite;
    public Sprite wallSprite;
    public Sprite playerSprite;
    public Sprite enemySprite;
    public Sprite potionSprite;
    public Sprite chestSprite;
    public Sprite exitSprite;
    public Sprite defaultSprite;

    public Sprite GetSprite(TileType type = TileType.Empty) 
    {
        return type switch
        {
            TileType.Floor => floorSprite,
            TileType.Wall => wallSprite,
            TileType.Player => playerSprite,
            TileType.Enemy => enemySprite,
            TileType.Potion => potionSprite,
            TileType.Chest => chestSprite,
            TileType.Exit => exitSprite,
            TileType.Empty => defaultSprite,
            _ => null,
        };
    }
}
