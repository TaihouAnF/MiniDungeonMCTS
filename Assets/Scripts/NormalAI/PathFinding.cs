using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    

    public static List<Vector2Int> GetPathTo(Vector2Int source, Vector2Int to, Func<Vector2Int, bool> passable)
    {
        TileType[,] tiles = DungeonManager.Instance.mp;
        Vector2Int[,] directionMap = new Vector2Int[tiles.GetLength(0), tiles.GetLength(1)];
        int[,] cost = new int[tiles.GetLength(0), tiles.GetLength(1)];

        HashSet<Vector2Int> openList = new();
        HashSet<Vector2Int> waitList = new();

        waitList.Add(source);

        Vector2Int[] next = new Vector2Int[4];

        while(waitList.Count > 0)
        {
            Vector2Int pos = Vector2Int.zero;
            int minCost = 999999;

            foreach(Vector2Int currentPos in waitList)
            {
                int currentCost = cost[currentPos.x, currentPos.y];
                if(currentCost <= minCost)
                {
                    pos = currentPos;
                    minCost = currentCost;
                }
            }

            next[0] = pos + Vector2Int.up;
            next[1] = pos + Vector2Int.down;
            next[2] = pos + Vector2Int.left;
            next[3] = pos + Vector2Int.right;

            openList.Add(pos);
            waitList.Remove(pos);

            if(pos == to)
            {
                List<Vector2Int> result = new List<Vector2Int>();
                Vector2Int prevPos = pos;
                while(prevPos != source)
                {
                    result.Add(prevPos);
                    prevPos += directionMap[prevPos.x, prevPos.y];
                }
                result.Reverse();
                return result;
            }

            foreach(Vector2Int nextPos in next)
            {
                if(openList.Contains(nextPos) || waitList.Contains(nextPos))
                {
                    continue;
                }

                if (!passable(nextPos))
                {
                    continue;
                }

                cost[nextPos.x, nextPos.y] = minCost + 1;
                directionMap[nextPos.x, nextPos.y] = pos - nextPos;

                waitList.Add(nextPos);
            }
        }

        return null;
    }
}
