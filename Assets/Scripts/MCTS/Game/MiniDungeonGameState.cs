using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct PlayerStatus
{
    public Vector2Int m_PlayerPrevPos;
    public List<Vector2Int> m_PlayerPrevPoses;
    public Vector2Int m_PlayerCurPos;
    public int m_Health;
    public float m_Score;
    public int m_EnemyKilled;
}
public class MiniDungeonGameState : BaseGameState
{
    public TileType[][] MapState;
    public Vector2Int WinPos;
    //Player Status
    public PlayerStatus m_PlayerStatus;

    private DungeonManager m_DungeonManager;
    public MCTSConfig m_MCTSConfig;


    
    public MiniDungeonGameState(TileType[,] state, Vector2Int win, PlayerStatus player, DungeonManager dungeonManager, MCTSConfig mCTSConfig)
    {
        m_PlayerStatus.m_PlayerPrevPoses = new List<Vector2Int>();

        MapState = new TileType[state.GetLength(0)][];
        int row = state.GetLength(0);
        int col = state.GetLength(1);
        for(int i = 0; i < row; i++)
        {
            MapState[i] = new TileType[col];
            for(int j = 0;j< col; j++)
            {
                MapState[i][j] = state[i, j];
            }
        }
        WinPos = win;

        m_PlayerStatus.m_PlayerCurPos = player.m_PlayerCurPos;
        m_PlayerStatus.m_Health = player.m_Health;
        m_PlayerStatus.m_Score = player.m_Score;
        m_PlayerStatus.m_EnemyKilled = player.m_EnemyKilled;

        m_DungeonManager = dungeonManager;
        m_MCTSConfig = mCTSConfig;
    }

    public MiniDungeonGameState(TileType[][] state, Vector2Int win, PlayerStatus player, DungeonManager dungeonManager, MCTSConfig mCTSConfig)
    {
        m_PlayerStatus.m_PlayerPrevPoses = new List<Vector2Int>();

        MapState = new TileType[state.Length][];
        for (int i = 0; i < state.Length; i++)
        {
            MapState[i] = new TileType[state[i].Length];
            for(int j = 0;j<state[i].Length; j++)
            {
                MapState[i][j] = state[i][j];
            }
        }
        WinPos = win;

        m_PlayerStatus.m_PlayerCurPos = player.m_PlayerCurPos;
        m_PlayerStatus.m_Health = player.m_Health;
        m_PlayerStatus.m_Score = player.m_Score;
        m_PlayerStatus.m_EnemyKilled = player.m_EnemyKilled;

        m_DungeonManager = dungeonManager;
        m_MCTSConfig = mCTSConfig;
    }

    public MiniDungeonGameState(MiniDungeonGameState newGameState)
    {
        m_PlayerStatus.m_PlayerPrevPoses = new List<Vector2Int>();
        m_PlayerStatus.m_PlayerPrevPoses.AddRange(newGameState.m_PlayerStatus.m_PlayerPrevPoses);

        MapState = new TileType[newGameState.MapState.Length][];
        for(int i = 0;i< newGameState.MapState.Length; i++)
        {
            MapState[i] = new TileType[newGameState.MapState[i].Length];
            for(int j = 0; j < newGameState.MapState[i].Length; j++)
            {
                MapState[i][j] = newGameState.MapState[i][j];
            }
        }
        WinPos = newGameState.WinPos;

        m_PlayerStatus.m_PlayerCurPos = newGameState.m_PlayerStatus.m_PlayerCurPos;
        m_PlayerStatus.m_Health = newGameState.m_PlayerStatus.m_Health;
        m_PlayerStatus.m_Score = newGameState.m_PlayerStatus.m_Score;
        m_PlayerStatus.m_EnemyKilled = newGameState.m_PlayerStatus.m_EnemyKilled;

        m_DungeonManager = newGameState.m_DungeonManager;
        m_MCTSConfig = newGameState.m_MCTSConfig;
    }
    public override float GameResult()
    {
        if (m_PlayerStatus.m_Health <= 0)
        {
            return m_PlayerStatus.m_Score + m_MCTSConfig.PlayerDieScore;
        }
        if(m_PlayerStatus.m_PlayerCurPos == WinPos)
        {
            return m_PlayerStatus.m_Score;
        }
        switch (MapState[m_PlayerStatus.m_PlayerCurPos.x][m_PlayerStatus.m_PlayerCurPos.y])
        {
            case TileType.Enemy:
                return m_PlayerStatus.m_Score + m_MCTSConfig.KillEnemyScore;
            case TileType.Potion:
                return m_PlayerStatus.m_Score + m_MCTSConfig.PotionScore;
            case TileType.Chest:
                return m_PlayerStatus.m_Score + m_MCTSConfig.OpenChestScore;
            case TileType.Exit:
                return m_PlayerStatus.m_Score + m_MCTSConfig.PlayerEscapeScore;
        }
        return m_PlayerStatus.m_Score;
    }

    public override List<MoveAction> GetLegalActions()
    {
        List<MoveAction> actions = new List<MoveAction>();

        MoveAction left = new MoveAction(m_PlayerStatus.m_PlayerCurPos + Vector2Int.left);
        if (IsMoveLegel(left) && !m_PlayerStatus.m_PlayerPrevPoses.Contains(left.MovePos))
            actions.Add(left);

        MoveAction up = new MoveAction(m_PlayerStatus.m_PlayerCurPos + Vector2Int.up);
        if (IsMoveLegel(up) && !m_PlayerStatus.m_PlayerPrevPoses.Contains(up.MovePos))
            actions.Add(up);

        MoveAction right = new MoveAction(m_PlayerStatus.m_PlayerCurPos + Vector2Int.right);
        if (IsMoveLegel(right) && !m_PlayerStatus.m_PlayerPrevPoses.Contains(right.MovePos))
            actions.Add(right);

        MoveAction down = new MoveAction(m_PlayerStatus.m_PlayerCurPos + Vector2Int.down);
        if (IsMoveLegel(down) && !m_PlayerStatus.m_PlayerPrevPoses.Contains(down.MovePos))
            actions.Add(down);

        return actions;
    }

    public override bool IsGameOver()
    {
        return m_PlayerStatus.m_PlayerCurPos == WinPos;
    }
    public bool IsMoveLegel(MoveAction action)
    {
        return m_DungeonManager.IsWalkable(action.MovePos);
    }
    public MiniDungeonGameState Move(MoveAction action)
    {
        if (!IsMoveLegel(action))
        {
            Debug.LogFormat("Current Move is ilegal: ({0}, {1})", action.MovePos.x, action.MovePos.y);
            return null;
        }
        MiniDungeonGameState result = new MiniDungeonGameState(MapState, WinPos, m_PlayerStatus, m_DungeonManager, m_MCTSConfig);

        result.MapState[m_PlayerStatus.m_PlayerCurPos.x][m_PlayerStatus.m_PlayerCurPos.y] = TileType.Floor;

        Vector2Int newPos = action.MovePos;
        switch (MapState[newPos.x][newPos.y])
        {
            case TileType.Empty:
                break;
            case TileType.Floor:
                break;
            case TileType.Wall:
                break;
            case TileType.Player:
                break;
            case TileType.Enemy:
                result.m_PlayerStatus.m_Health -= m_MCTSConfig.EnemyDamage;
                result.m_PlayerStatus.m_Score += m_MCTSConfig.KillEnemyScore;
                result.m_PlayerStatus.m_EnemyKilled++;
                result.m_PlayerStatus.m_PlayerPrevPoses.Clear();
                break;
            case TileType.Potion:
                result.m_PlayerStatus.m_Health += m_MCTSConfig.PotionHeal;
                result.m_PlayerStatus.m_Score += m_MCTSConfig.PotionScore;
                result.m_PlayerStatus.m_PlayerPrevPoses.Clear();
                break;
            case TileType.Chest:
                result.m_PlayerStatus.m_Score += m_MCTSConfig.OpenChestScore;
                result.m_PlayerStatus.m_PlayerPrevPoses.Clear();
                break;
            case TileType.Exit:
                result.m_PlayerStatus.m_Score += m_MCTSConfig.PlayerEscapeScore;
                result.m_PlayerStatus.m_PlayerPrevPoses.Clear();
                break;
        }

        if (newPos == m_PlayerStatus.m_PlayerPrevPos)
        {
            result.m_PlayerStatus.m_Score += m_MCTSConfig.PlayerGoBackScore;
        }
        result.m_PlayerStatus.m_Score += m_MCTSConfig.PlayerMoveScore;

        result.m_PlayerStatus.m_PlayerPrevPos = m_PlayerStatus.m_PlayerCurPos;
        result.m_PlayerStatus.m_PlayerCurPos = newPos;
        result.MapState[newPos.x][newPos.y] = TileType.Player;



        int count = 0;
        if (m_DungeonManager.IsWalkable(newPos + Vector2Int.left))
        {
            count++;
        }
        if (m_DungeonManager.IsWalkable(newPos + Vector2Int.up))
        {
            count++;
        }
        if (m_DungeonManager.IsWalkable(newPos + Vector2Int.right))
        {
            count++;
        }
        if (m_DungeonManager.IsWalkable(newPos + Vector2Int.down))
        {
            count++;
        }

        if (count > 1)
        {
            result.m_PlayerStatus.m_PlayerPrevPoses.Add(result.m_PlayerStatus.m_PlayerPrevPos);
        }

        /*        int row = MapState.Length;
                int col = MapState[0].Length;
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        switch (MapState[i][j])
                        {
                            case TileType.Empty:
                                break;
                            case TileType.Floor:
                                break;
                            case TileType.Wall:
                                break;
                            case TileType.Player:
                                break;
                            case TileType.Enemy:
                                result.m_PlayerStatus.m_Score += m_MCTSConfig.KillEnemyScore * m_MCTSConfig.BaseCloseToTarget * (Vector2.Distance(new Vector2(i, j), result.m_PlayerStatus.m_PlayerCurPos));
                                break;
                            case TileType.Potion:
                                result.m_PlayerStatus.m_Score += m_MCTSConfig.PotionScore * m_MCTSConfig.BaseCloseToTarget * (Vector2.Distance(new Vector2(i, j), result.m_PlayerStatus.m_PlayerCurPos));
                                break;
                            case TileType.Chest:
                                result.m_PlayerStatus.m_Score += m_MCTSConfig.OpenChestScore * m_MCTSConfig.BaseCloseToTarget * (Vector2.Distance(new Vector2(i, j), result.m_PlayerStatus.m_PlayerCurPos));
                                break;
                            case TileType.Exit:
                                result.m_PlayerStatus.m_Score += m_MCTSConfig.PlayerEscapeScore * m_MCTSConfig.BaseCloseToTarget * (Vector2.Distance(new Vector2(i, j), result.m_PlayerStatus.m_PlayerCurPos));

                                break;
                            default:
                                break;
                        }
                    }
                }*/

        return result;
    }
}
