using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitBase : UnitBase
{
    public virtual void AttackEnemy(EnemyUnitBase enemy)
    {
        GameManager.Instance.ChangeGameState(GameState.ActionOccuring);
    }
    public virtual void TakeDamage(int amount)
    {
        TotalHealth -= amount;
        CountPerStack = (int)((TotalHealth + HealthPerUnit - 1) / HealthPerUnit);

        if (CountPerStack <= 0)
        {
            UnitManager.Instance.ReslovePlayerDeath(this);
            return;
        }
    }
    public virtual void DeductTurn()
    {
        Turns--;

        if (Turns == 0)
        {
            IsTurnPlayed = true;
        }
        GridManager.Instance.TurnAllGridsOff();
        UnitManager.Instance.ResolvePlayerTurn();
    }

    public virtual void ExecuteAbility(EnemyUnitBase enemy = null)
    {

    }

    public virtual void ExecutePassive()
    {

    }
}
