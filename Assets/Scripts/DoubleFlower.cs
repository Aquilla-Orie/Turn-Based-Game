using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleFlower : MonoBehaviour
{
    public int DamageValue;
    public float DamageRaduis;
    public GameObject SecondFlower;
    public void Init()
    {

        var objFirst = Physics2D.OverlapCircleAll(transform.position, DamageRaduis);
        var objSecond = Physics2D.OverlapCircleAll(SecondFlower.transform.position, DamageRaduis);

        foreach (var item in objFirst)
        {
            if(item.TryGetComponent<Tile>(out Tile tile))
            {
                tile.SetHighlight(true);
            }
            if(item.TryGetComponent<EnemyUnitBase>(out EnemyUnitBase enemy))
            {
                enemy.TakeDamage(DamageValue);
            }

        }
        foreach (var item in objSecond)
        {
            if(item.TryGetComponent<Tile>(out Tile tile))
            {
                tile.SetHighlight(true);
            }
            if(item.TryGetComponent<EnemyUnitBase>(out EnemyUnitBase enemy))
            {
                enemy.TakeDamage(DamageValue);
            }

        }
        StartCoroutine(DestroyObject());

    }

    private IEnumerator DestroyObject()
    {
        var objFirst = Physics2D.OverlapCircleAll(transform.position, DamageRaduis);
        var objSecond = Physics2D.OverlapCircleAll(SecondFlower.transform.position, DamageRaduis);

        foreach (var item in objFirst)
        {
            if (item.TryGetComponent<Tile>(out Tile tile))
            {
                tile.SetHighlight(true);
            }
        }
        foreach (var item in objSecond)
        {
            if (item.TryGetComponent<Tile>(out Tile tile))
            {
                tile.SetHighlight(true);
            }
        }

        yield return new WaitForSeconds(.5f);


        GridManager.Instance.TurnAllGridsOff();
        Destroy(gameObject);
    }
}
