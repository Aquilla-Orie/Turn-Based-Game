using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightUnit : PlayerUnitBase
{
    private int _specialUsed = 0;
    private int _damageReduction = 99;

    public override void AttackEnemy(EnemyUnitBase enemy)
    {
        base.AttackEnemy(enemy);
        StartCoroutine(Attack(enemy));
    }

    public override void ExecuteAbility(EnemyUnitBase enemy = null)
    {
        base.ExecuteAbility();
        if (_specialUsed <= 4)
        {
            _specialUsed++;
        }
    }

    public override void ExecutePassive()
    {
        base.ExecutePassive();
        CalculateDashProbability();
    }

    private void CalculateDashProbability()
    {
        var chance = Random.value * 100;
        if (chance <= 45.0)
        {
            Debug.Log("Dash Added");
            Turns++;
            if(IsTurnPlayed == true) IsTurnPlayed = false;
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        TotalHealth += (amount * _damageReduction) / 100;
        _damageReduction -= 33;
        Mathf.Clamp(_damageReduction, 0, 99);
    }

    private IEnumerator Attack(EnemyUnitBase enemy)
    {
        float startTime = Time.time;
        while (Time.time - startTime <= 1)
        {
            transform.position = Vector2.Lerp(transform.position, enemy.transform.position, Time.time - startTime);
            //yield return 1;
            startTime += .02f;
            yield return new WaitForSeconds(.04f);
        }

        enemy.TakeDamage(Random.Range(Damage[0], Damage[1] + 1));
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
        GameManager.Instance.ChangeGameState(GameState.PlayerTurn);
        ExecutePassive();
        DeductTurn();
    }
}
