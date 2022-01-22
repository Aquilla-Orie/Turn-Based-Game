using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherUnit : PlayerUnitBase
{
    private int[] _damageWithBow = { 6, 15 };
    private int[] _damageWithHand = { 1, 6 };
    [SerializeField] public DoubleFlower DoubleFlower;
    public override void AttackEnemy(EnemyUnitBase enemy)
    {
        base.AttackEnemy(enemy);
        CheckEnemyDistance(enemy);

        GameManager.Instance.ChangeGameState(GameState.PlayerTurn);
        DeductTurn();
    }

    private void CheckEnemyDistance(EnemyUnitBase enemy)
    {
        if (Vector2.Distance(transform.position, enemy.transform.position) <= 1f)
        {
            Damage = _damageWithHand;
            StartCoroutine(AttackWithHand(enemy));
            return;
        }
        Damage = _damageWithBow;
        enemy.TakeDamage(Random.Range(Damage[0], Damage[1] + 1));
    }

    private IEnumerator AttackWithHand(EnemyUnitBase enemy)
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
        ExecutePassive();
    }

    public override void ExecutePassive()
    {
        base.ExecutePassive();
    }

    public override void ExecuteAbility(EnemyUnitBase enemy = null)
    {
        base.ExecuteAbility();
        if (enemy == null) return;

        var doubleFlower = Instantiate(DoubleFlower, enemy.transform);
        doubleFlower.DamageValue = (Random.Range(Damage[0], Damage[1] + 1))/2;
        doubleFlower.Init();
        UnitManager.Instance.SelectedPlayerUnit.DeductTurn();
    }

}
