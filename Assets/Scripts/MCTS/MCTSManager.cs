using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCTSManager : MonoBehaviour
{
    public MCTSConfig Config;
    public float MoveCD;
    private float MoveCounter;
    

    [SerializeField]
    private PlayerController m_PlayerController;
    [SerializeField]
    private DungeonManager m_DungeonManager;
    private MiniDungeonGameState m_GameState;
    private bool m_IsGameStart;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsGameStart)
        {
            if (MoveCounter > 0)
            {
                MoveCounter -= Time.deltaTime;
                return;
            }
            MoveCounter = MoveCD;
            MCTSNode root = new MCTSNode(m_GameState);
            MCTSearch mcts = new MCTSearch(root);
            MCTSNode bestNode = mcts.BestAction(100);

            m_GameState = bestNode.GameState;
            m_PlayerController.TryMove(m_GameState.m_PlayerStatus.m_PlayerCurPos);

            if (m_GameState.IsGameOver())
            {
                m_IsGameStart = false;
            }
        }
    }

    public void OnClickStartMCTS()
    {
        Vector2Int WinPos = new Vector2Int(0,0);
        int row = m_DungeonManager.mp.GetLength(0);
        int col = m_DungeonManager.mp.GetLength(1);
        for (int i = 0; i < row; i++)
        {
            for(int j = 0; j< col; j++) 
            {
                if(m_DungeonManager.mp[i,j] == TileType.Exit)
                {
                    WinPos = new Vector2Int(i, j);
                    break;
                }
            }
        }

        m_PlayerController = GameObject.FindAnyObjectByType<PlayerController>();    
        PlayerStatus playerStatus = new PlayerStatus()
        {
            m_EnemyKilled = m_PlayerController.EnemyKilled,
            m_Health = m_PlayerController.Health,
            m_PlayerCurPos = m_PlayerController.CurPos,
            m_Score = m_PlayerController.Score
        };
        m_GameState = new MiniDungeonGameState(m_DungeonManager.mp, WinPos, playerStatus, m_DungeonManager, Config);
        m_IsGameStart = true;
    }
}
