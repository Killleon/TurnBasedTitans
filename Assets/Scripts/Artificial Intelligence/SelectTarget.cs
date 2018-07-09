using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SelectTarget : Actions {

    [Header("Condition Scores")]
    public int LeastHpEnemy = 2;
    public int FFScore = 2;
    public int TypeAdvantage = 1;
    public int InUnitProximity = 1;
    int TargetScore;
    bool IHaveAdvantage;
    private Unit me;
    private List<Unit> enemies;

    public override int EvaluateTarget(Unit _unit) {

        enemies = new List<Unit>();
        foreach (Unit u in GameManager.instance.Units)
        {
            if (u.Owner != me.Owner ) {
                enemies.Add(u);
            }
        }

        TargetScore = 0;
        if ( LowestHPEnemy(_unit) )
            TargetScore += LeastHpEnemy;
        if ( IsFocusFiring(_unit) )
            TargetScore += FFScore;
        EnemyHasAdvantage(_unit);
        if (IHaveAdvantage)
            TargetScore += TypeAdvantage;
        if ( InRangeToAttack(_unit) )
            TargetScore += InUnitProximity;

        //Debug.Log(
        //    "NAME: " + Properties.thisUnit +
        //    "===============" +
        //    "   SCANNING: " + _unit.name +
        //    "   LowestHP: " + LowestHPEnemy(_unit) +
        //    "   FF: " + IsFocusFiring(_unit) +
        //    "   Type Advantage: " + IHaveAdvantage +
        //    "   In ATK Range: " + InRangeToAttack(_unit) +
        //    "   Final Score: " + TargetScore
        //    );

        return TargetScore;
    }

    private bool InRangeToAttack(Unit _unit) {

        List<Tile> neighboors = me.CurrentTile.GetNeighbours(GameManager.instance.Tiles, me.Rng);

        if (neighboors.Contains(_unit.CurrentTile))
            return true;
        else
            return false;
    }

    private void EnemyHasAdvantage(Unit _unit) {
        IHaveAdvantage = false;
        UnitType myType = Properties.CurrentType;
        UnitType enemyType = _unit.UnitType;

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

    private bool LowestHPEnemy(Unit _unit) {
        List<Unit> enemyList = enemies.OrderBy(eu => eu.CurrentHP).ToList();
        if (enemyList.Count > 1 && enemyList.First().CurrentHP == enemyList[1].CurrentHP ) {
            return false;
        }
        if (_unit == enemyList.First() )
            return true;
        else
            return false;
    }

    private bool IsFocusFiring(Unit _unit) {
        List<Unit> FFList = new List<Unit>();
        for (int i=0;i < Properties.Allies.Count; i ++) {
            if(Properties.Allies[i].CurrentTarget == _unit && Properties.Allies[i] != me) {
                FFList.Add(_unit);
            }
        }

        if (FFList.Contains(_unit))
            return true;
        else
            return false;
    }


    void Start() {
        StartCoroutine(Init());
    }

    IEnumerator Init() {
        Properties = transform.parent.parent.GetComponentInChildren<AIProperties>();
        me = Properties.thisUnit;
        yield return new WaitUntil( () => Properties.Initialized );
    }
}
