using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonUnit : EnemyUnitBase
{
    public bool IsMotherStack;

    public override void AttackPlayer(PlayerUnitBase player)
    {
        base.AttackPlayer(player);
        StartCoroutine(Attack(player));
    }
    public void SetMotherStack(bool isMotherStack)
    {
        IsMotherStack = isMotherStack;
    }
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (!IsMotherStack) return;

        int fallenComrades = 50 - CountPerStack;
        Debug.Log($"Fallen Comrades: {fallenComrades}");
        if (fallenComrades > 0)
            SpawnNewStack(fallenComrades);
    }

    private void SpawnNewStack(int fallenComrades)
    {
        ScriptableUnit spawnedSkeleton = Resources.Load<ScriptableUnit>("Units/EnemyUnits/SkeletonUnit");
        UnitManager.Instance.SpawnSkeletonUnit(spawnedSkeleton.UnitPrefab, false, fallenComrades);
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
}
