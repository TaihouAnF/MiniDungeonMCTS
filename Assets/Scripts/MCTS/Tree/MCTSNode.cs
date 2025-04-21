using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class MCTSNode
{
    public MCTSNode Parent;
    public MiniDungeonGameState GameState;
    public List<MCTSNode> Children;

    private float m_Result;
    private int m_NumberOfVisits;
    private Queue<MoveAction> m_UntriedActions;

    public MCTSNode(MiniDungeonGameState gameState, MCTSNode parent = null)
    {
        GameState = gameState;
        Parent = parent;
        Children = new List<MCTSNode>();
    }

    public Queue<MoveAction> UntriedActions()
    {
        if(m_UntriedActions == null)
        {
            m_UntriedActions = new Queue<MoveAction>();
            foreach(var action in GameState.GetLegalActions())
            {
                m_UntriedActions.Enqueue(action);
            }
            
        }
        return m_UntriedActions;
    }

    public float GetQ()
    {
        return m_Result;
    }

    public int GetN()
    {
        return m_NumberOfVisits;
    }

    public MCTSNode Expand()
    {
        MoveAction action = UntriedActions().Dequeue();
        MiniDungeonGameState nextState = GameState.Move(action);
        MCTSNode childnode = new MCTSNode(nextState, this);
        Children.Add(childnode);
        return childnode;
    }
    
    public bool IsTerminalNode()
    {
        return GameState.IsGameOver();
    }

    public int RollOut()
    {
        MiniDungeonGameState currentRolloutState = new MiniDungeonGameState(GameState);
        while(!currentRolloutState.IsGameOver())
        {
            List<MoveAction> possibleMoves = currentRolloutState.GetLegalActions();
            MoveAction action = RollOutPolicy(possibleMoves);
            currentRolloutState = currentRolloutState.Move(action);
        }
        return currentRolloutState.GameResult();
    }

    public void BackPropagate(float result)
    {
        m_NumberOfVisits += 1;
        m_Result += result;
        if(Parent != null)
        {
            Parent.BackPropagate(result);
        }
    }

    public bool IsFullyExpand()
    {
        return UntriedActions().Count == 0;
    }

    public MCTSNode BestChild(float cParam=1.2f)
    {
        int bestIndex = -1;
        float bestWeight = float.MinValue;
        int count = Children.Count;
        for (int i = 0;i < count; i++) 
        {
            float q = (float)Children[i].GetQ();
            float n = (float)Children[i].GetN();
            float nodeWeight = q / n + cParam * Mathf.Sqrt((2 * Mathf.Log((float)GetN()) / n));
            if (nodeWeight > bestWeight)
            {
                bestIndex = i;
                bestWeight = nodeWeight;
            }
        }
        return Children[bestIndex];
    }

    public MoveAction RollOutPolicy(List<MoveAction> possibleMoves)
    {
        if(possibleMoves.Count == 0)
        {
            Debug.Log("Current Node doesn't have PossibleMoves!");
            return null;
        }
        return possibleMoves[Random.Range(0, possibleMoves.Count)];
    }
}
