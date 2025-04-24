using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StrategySelecter : MonoBehaviour
{
    public enum TileBehavior
    {
        Ignore,
        Reach,
        Avoid,
    }

    private class Solution : IComparable<Solution>
    {
        public int AvoidFailedCount;

        public List<Vector2Int> Path;

        public int CompareTo(Solution other)
        {
            if(AvoidFailedCount != other.AvoidFailedCount)
            {
                return AvoidFailedCount.CompareTo(other.AvoidFailedCount);
            }

            return Path.Count.CompareTo(other.Path.Count);
        }
    }

    [Header("Init Setting")]
    public TileBehavior Enemy;
    public TileBehavior Potion;
    public TileBehavior Chest;

    private void OnGUI()
    {
        DrawChoice(300, ref Enemy, "enemy");
        DrawChoice(400, ref Potion, "potion");
        DrawChoice(500, ref Chest, "chest");

        if (PlayerMover.IsFinished && GUI.Button(new Rect(60, 600, 150, 60), $"begin"))
        {
            StartSearch();
        }
    }

    private void StartSearch()
    {
        TileType[,] map = DungeonManager.Instance.mp;
        HashSet<Vector2Int> avoid = new();
        HashSet<Vector2Int> required = new();
        Vector2Int final = Vector2Int.zero;
        Vector2Int current = PlayerController.Instance.CurPos;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == TileType.Enemy)
                {
                    if(Enemy == TileBehavior.Reach)
                    {
                        required.Add(new Vector2Int(i, j));
                    }
                    else if(Enemy == TileBehavior.Avoid)
                    {
                        avoid.Add(new Vector2Int(i, j));
                    }
                }
                else if (map[i, j] == TileType.Potion)
                {
                    if (Potion == TileBehavior.Reach)
                    {
                        required.Add(new Vector2Int(i, j));
                    }
                    else if (Potion == TileBehavior.Avoid)
                    {
                        avoid.Add(new Vector2Int(i, j));
                    }
                }
                else if (map[i, j] == TileType.Chest)
                {
                    if (Chest == TileBehavior.Reach)
                    {
                        required.Add(new Vector2Int(i, j));
                    }
                    else if (Chest == TileBehavior.Avoid)
                    {
                        avoid.Add(new Vector2Int(i, j));
                    }
                }
                else if (map[i, j] == TileType.Exit)
                {
                    final = new Vector2Int(i, j);
                }
            }
        }

        Solution solution = FindSolution(current, required, avoid, final);
        
        if(solution != null)
        {
            string result = $"Solution({solution.Path.Count}, {solution.AvoidFailedCount}): ";
            foreach (var pos in solution.Path)
            {
                result += $"({pos.x},{pos.y})-";
            }
            Debug.Log(result);

            PlayerMover.SetRoute(solution.Path);
        }
        else
        {
            Debug.Log("No Solution!");
        }
    }

    private Func<Vector2Int, bool> GetFilter(HashSet<Vector2Int> avoid)
    {
        return (Vector2Int pos) =>
        {
            DungeonManager manager = DungeonManager.Instance;
            return manager.IsInBound(pos) && manager.IsWalkable(pos) && !avoid.Contains(pos);
        };
    }

    private Solution FindSolution(Vector2Int current, HashSet<Vector2Int> required, HashSet<Vector2Int> avoid, Vector2Int final)
    {
        var filter = GetFilter(avoid);
        Solution solution = null;

        if (required.Count == 0)
        {
            List<Vector2Int> path = PathFinding.GetPathTo(current, final, filter);
            
            if(path == null)
            {
                if(avoid.Count > 0)
                {
                    List<Solution> solutions = new List<Solution>();
                    Vector2Int[] removed = avoid.ToArray();

                    foreach(Vector2Int pos in removed)
                    {
                        HashSet<Vector2Int> newAvoid = new HashSet<Vector2Int>(avoid);
                        newAvoid.Remove(pos);

                        Solution currentSolution = FindSolution(current, required, newAvoid, final);
                        if(currentSolution != null)
                        {
                            currentSolution.AvoidFailedCount++;
                            solutions.Add(currentSolution);
                        }
                    }

                    if(solutions.Count > 0)
                    {
                        solutions.Sort();
                        return solutions[0];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                solution = new Solution()
                {
                    Path = path,
                };
                return solution;
            }
        }

        List<Solution> allSolutions = new();
        Vector2Int[] requiredPos = required.ToArray();

        foreach(Vector2Int rPos in requiredPos)
        {
            List<Vector2Int> path = PathFinding.GetPathTo(current, rPos, filter);
            Solution rPosSolution = null;

            if(path == null)
            {
                if (avoid.Count > 0)
                {
                    List<Solution> solutions = new List<Solution>();
                    Vector2Int[] removed = avoid.ToArray();

                    foreach (Vector2Int pos in removed)
                    {
                        HashSet<Vector2Int> newAvoid = new HashSet<Vector2Int>(avoid);
                        newAvoid.Remove(pos);

                        Solution currentSolution = FindSolution(current, required, newAvoid, final);
                        if (currentSolution != null)
                        {
                            currentSolution.AvoidFailedCount++;
                            solutions.Add(currentSolution);
                        }
                    }

                    if (solutions.Count > 0)
                    {
                        solutions.Sort();
                        rPosSolution = solutions[0];
                    }
                }
            }
            else
            {
                HashSet<Vector2Int> newRequired = new HashSet<Vector2Int>(required);
                newRequired.Remove(rPos);

                rPosSolution = FindSolution(rPos, newRequired, avoid, final);
                if(rPosSolution == null)
                {
                    return null;
                }

                path.AddRange(rPosSolution.Path);
                rPosSolution.Path = path;
            }
            
            if(rPosSolution != null)
            {
                allSolutions.Add(rPosSolution);
            }
            else
            {
                return null;
            }
        }

        if(allSolutions.Count > 0)
        {
            allSolutions.Sort();
            solution = allSolutions[0];
        }

        return solution;
    }

    private void DrawChoice(float y, ref TileBehavior behavior, string type)
    {
        if(behavior == TileBehavior.Ignore)
        {
            if (GUI.Button(new Rect(60, y, 150, 60), $"ignore {type}")) 
            { 
                behavior = TileBehavior.Reach;
            }
        }
        else if (behavior == TileBehavior.Reach)
        {
            if (GUI.Button(new Rect(60, y, 150, 60), $"reach {type}"))
            {
                behavior = TileBehavior.Avoid;
            }
        }
        else if (behavior == TileBehavior.Avoid)
        {
            if (GUI.Button(new Rect(60, y, 150, 60), $"try to avoid {type}"))
            {
                behavior = TileBehavior.Ignore;
            }
        }
    }
}
