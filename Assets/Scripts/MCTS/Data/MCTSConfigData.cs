using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMCTSConfigData", menuName = "Data/NewMCTSConfigData")]
public class MCTSConfig : ScriptableObject
{
    public float PlayerDieScore;
    public float PlayerEscapeScore;

    public float PlayerMoveScore;
    public float PlayerGoBackScore;
    public float KillEnemyScore;
    public float PotionScore;
    public float OpenChestScore;
    public int EnemyDamage;
    public int PotionHeal;

    [Header("Multiplier Config")]
    public float BaseCloseToTarget;
}
