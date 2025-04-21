using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMCTSConfigData", menuName = "Data/NewMCTSConfigData")]
public class MCTSConfig : ScriptableObject
{
    public int PlayerDieScore;
    public int PlayerEscapeScore;
    /// <summary>
    /// Score when player moves one step
    /// </summary>
    public int PlayerMoveScore;
    public int PlayerGoBackScore;
    public int KillEnemyScore;
    public int PotionScore;
    public int OpenChestScore;

    public int EnemyDamage;
    public int PotionHeal;


}
