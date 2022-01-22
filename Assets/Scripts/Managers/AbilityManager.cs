using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;

    public bool IsAbilitySelected;

    private void Awake()
    {
        Instance = this;
    }

    public void PerformAbility(PlayerUnitBase playerUnit, EnemyUnitBase enemy = null)
    {
        if (!IsAbilitySelected)
        {
            if(enemy != null)playerUnit.AttackEnemy(enemy);
            return;
        }
        playerUnit.ExecuteAbility(enemy);
        IsAbilitySelected = false;
    }

    public void PerformEnemyAbility(EnemyUnitBase enemy, PlayerUnitBase playerUnit = null)
    {
        if (!IsAbilitySelected)
        {
            if(playerUnit != null) enemy.AttackPlayer(playerUnit);
            return;
        }
        enemy.ExecuteAbility(playerUnit);
        IsAbilitySelected = false;
    }


}
