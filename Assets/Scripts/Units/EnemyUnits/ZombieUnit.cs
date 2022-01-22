using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZombieUnit : EnemyUnitBase
{
    public override void AttackPlayer(PlayerUnitBase player)
    {
        base.AttackPlayer(player);
        StartCoroutine(Attack(player));
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
    }

    public override void DeductTurn()
    {
        base.DeductTurn();
    }

    private IEnumerator Attack(PlayerUnitBase player)
    {
        float startTime = Time.time;
        while (Time.time - startTime <= 1)
        {
            transform.position = Vector2.Lerp(transform.position, player.transform.position, Time.time - startTime);
            //yield return 1;
            startTime += .02f;
            yield return new WaitForSeconds(.04f);
        }

        player.TakeDamage(Random.Range(Damage[0], Damage[1] + 1));
        player.Faction = Faction.EnemyUnit;
        yield return new WaitForSeconds(1.0f);


        startTime = Time.time;
        while (Time.time - startTime <= 1)
        {
            transform.position = Vector2.Lerp(transform.position, OccupiedTile.transform.position, Time.time - startTime);
            //yield return 1;
            startTime += .02f;
            yield return new WaitForSeconds(.04f);
        }

        OccupiedTile.SetUnit(this);
        GameManager.Instance.ChangeGameState(GameState.EnemyTurn);
        DeductTurn();
        ExecutePassive();
    }

    public override void DetermineNextMove()
    {
        base.DetermineNextMove();
    }
}
