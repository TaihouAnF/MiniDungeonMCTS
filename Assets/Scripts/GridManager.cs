using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width;
    public int height;
    public TextAsset levelText;
    public GameObject wallPrefab, floorPrefab, playerPrefab, enemyPrefab, potionPrefab, chestPrefab, exitPrefab;
    public Transform gridRoot;
    
    // Start is called before the first frame update
    void Start()
    {
        LoadLevel(levelText.text);
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadLevel(string level)
    {
        string[] lines = level.Split('\n');
        height = lines.Length;
        width = lines[0].Length;

        for (int y = 0; y < height; ++y) {
            string line = lines[lines.Length - 1 - y];  // Inverted Y so top row == top line
            for (int x = 0; x < line.Length; ++x) {
                
                char c = line[x];
                Vector3 localPos = new(x, y, 0);
                Transform parent = gridRoot;
                Instantiate(floorPrefab, localPos, Quaternion.identity, parent);

                switch (c)
                {
                    case '#': Instantiate(wallPrefab, localPos, Quaternion.identity, parent); break;
                    case '@': Instantiate(playerPrefab, localPos, Quaternion.identity, parent); break;
                    case 'M': Instantiate(enemyPrefab, localPos, Quaternion.identity, parent); break;
                    case 'P': Instantiate(potionPrefab, localPos, Quaternion.identity, parent); break;
                    case 'E': Instantiate(exitPrefab, localPos, Quaternion.identity, parent); break;
                }
            }
        }
        // center the grid
        CenterDungeon(width, height);
    }

    private void CenterDungeon(int width, int height)
    {
        gridRoot.position = new Vector3(-width / 2f + 0.5f, -height / 2f + 0.5f);
    }
}
