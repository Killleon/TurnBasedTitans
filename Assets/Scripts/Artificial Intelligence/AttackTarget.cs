using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackTarget : Actions
{
    [Header("Condition Scores")]
    public int InAtkRange = 6;
    public int MyAdvantage = 3;
    public int EnemyAdvantage = -3;
    private bool IHaveAdvantage;

    public override void DoAction() {
        if( InRangeToAttack() )
            StartCoroutine( Attack() );
        else
            StartCoroutine( transform.parent.GetComponentInChildren<MoveToTarget>().Move() );
    }

    public IEnumerator Attack() {
        yield return new WaitForSeconds(0.15f);
        Unit currentUnit = Properties.thisUnit;
        currentUnit.Attack( currentUnit, currentUnit.CurrentTarget );
        while (currentUnit.isMoving)
            yield return new WaitForSeconds(1.5f);
        yield return new WaitUntil( ()=> currentUnit.FinishedAttack );
        yield break;
    }

    public override int GetScore() {
        if (InRangeToAttack())
            PScore += InAtkRange;

        EnemyHasAdvantage();
        if (IHaveAdvantage)
            PScore += MyAdvantage;
        else
            PScore += EnemyAdvantage;

        //Debug.Log(
        //    "InAtkRange: " + InRangeToAttack() +
        //    "   EnemyAdvantage: " + IHaveAdvantage
        //    );
        return PScore;
    }

    private bool InRangeToAttack() {
        List<Tile> neighboors = Properties.thisUnit.CurrentTile.GetNeighbours(GameManager.instance.Tiles, Properties.thisUnit.Rng);

        if (neighboors.Contains(Properties.CurrentTrgt.CurrentTile))
            return true;
        else
            return false;
    }

    private void EnemyHasAdvantage() {
        IHaveAdvantage = false;
        UnitType myType = Properties.CurrentType;
        UnitType enemyType = Properties.CurrentTrgt.UnitType;

        switch (myType)
        {
            case UnitType.Titan:
                if (enemyType == UnitType.Tiny)
                    IHaveAdvantage = true;
                break;

            case UnitType.Tanky:
                if (enemyType == UnitType.Titan)
                    IHaveAdvantage = true;
                break;

            case UnitType.Tiny:
                if (enemyType == UnitType.Tanky)
                    IHaveAdvantage = true;
                break;
        }
    }

    void Start() {
        StartCoroutine(Init());
    }

    IEnumerator Init() {
        Properties = transform.parent.parent.GetComponentInChildren<AIProperties>();
        yield return new WaitUntil(() => Properties.Initialized);
    }
}
