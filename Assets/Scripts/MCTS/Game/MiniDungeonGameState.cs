using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct PlayerStatus
{
    public Vector2Int m_PlayerPrevPos;
    public Vector2Int m_PlayerCurPos;
    public int m_Health;
    public int m_Score;
    public int m_EnemyKilled;
}
public class MiniDungeonGameState : BaseGameState
{
    public TileType[][] MapState;
    public Vector2Int WinPos;
    //Player Status
    public PlayerStatus m_PlayerStatus;

    private DungeonManager m_DungeonManager;
    private MCTSConfig m_MCTSConfig;


    
    public MiniDungeonGameState(TileType[,] state, Vector2Int win, PlayerStatus player, DungeonManager dungeonManager, MCTSConfig mCTSConfig)
    {
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
    public override int GameResult()
    {
        if (m_PlayerStatus.m_Health <= 0)
        {
            return m_PlayerStatus.m_Score + m_MCTSConfig.PlayerDieScore;
        }
        if(m_PlayerStatus.m_PlayerCurPos == WinPos)
        {
            return m_PlayerStatus.m_Score + m_MCTSConfig.PlayerEscapeScore;
        }
        return int.MinValue;
    }

    public override List<MoveAction> GetLegalActions()
    {
        List<MoveAction> actions = new List<MoveAction>();

        MoveAction left = new MoveAction(m_PlayerStatus.m_PlayerCurPos + Vector2Int.left);
        if (IsMoveLegel(left))
            actions.Add(left);

        MoveAction up = new MoveAction(m_PlayerStatus.m_PlayerCurPos + Vector2Int.up);
        if (IsMoveLegel(up))
            actions.Add(up);

        MoveAction right = new MoveAction(m_PlayerStatus.m_PlayerCurPos + Vector2Int.right);
        if (IsMoveLegel(right))
            actions.Add(right);

        MoveAction down = new MoveAction(m_PlayerStatus.m_PlayerCurPos + Vector2Int.down);
        if (IsMoveLegel(down))
            actions.Add(down);

        return actions;
    }

    public override bool IsGameOver()
    {
        return GameResult() == int.MinValue? false: true;
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
                break;
            case TileType.Potion:
                result.m_PlayerStatus.m_Health += m_MCTSConfig.PotionHeal;
                result.m_PlayerStatus.m_Score += m_MCTSConfig.PotionScore;
                break;
            case TileType.Chest:
                result.m_PlayerStatus.m_Score += m_MCTSConfig.OpenChestScore;
                break;
            case TileType.Exit:
                result.m_PlayerStatus.m_Score += m_MCTSConfig.PlayerEscapeScore;
                break;
        }

        
        if(newPos == m_PlayerStatus.m_PlayerPrevPos)
        {
            m_PlayerStatus.m_Score += m_MCTSConfig.PlayerGoBackScore;
        }

        result.m_PlayerStatus.m_PlayerPrevPos = m_PlayerStatus.m_PlayerCurPos;
        result.m_PlayerStatus.m_PlayerCurPos = newPos;
        result.MapState[newPos.x][newPos.y] = TileType.Player;

        return result;
    }
}
