using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MCTSearch
{
    public MCTSNode Root;
    public float EndTime;
    public MCTSearch(MCTSNode node)
    {
        Root = node;
    }

    public MCTSNode BestAction(int simulationNumber = 0, float totalSimulationTime = 0.0f )
    {
        if(simulationNumber == 0)
        {
            if (totalSimulationTime <= 0.0f)
            {
                Debug.LogError("Simulation Number and TotalSimulationTime are all 0");
                return null;
            }
            EndTime = Time.timeSinceLevelLoad + totalSimulationTime;
            while (true)
            {
                MCTSNode v = TreePolicy();
                float reward = v.RollOut();
                v.BackPropagate(reward);
                if(Time.timeSinceLevelLoad > EndTime)
                {
                    break;
                }
            }
        }
        else
        {
            for(int i = 0;i < simulationNumber; i++)
            {
                MCTSNode v = TreePolicy();
                float reward = v.RollOut();
                v.BackPropagate(reward);
            }
        }
        return Root.BestChild(0);
    }

    public MCTSNode TreePolicy()
    {
        MCTSNode currentNode = Root;
        while (!currentNode.IsTerminalNode())
        {
            if (!currentNode.IsFullyExpand())
            {
                return currentNode.Expand();
            }
            else
            {
                currentNode = currentNode.BestChild();
            }
        }
        return currentNode;
    }
}
