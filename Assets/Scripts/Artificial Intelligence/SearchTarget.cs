using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class SearchTarget : Actions {

    private bool HasTarget {
        get { return Properties.CurrentTrgt != null ? true : false ; }
    }
    private bool HasTypeAdv;
    private bool HasTypeSame;
    private List<Unit> enemies;

    [Header("Condition Scores")]
    public int HasTargetScore = 10;
    public int HasWeakerEnemyNearby = 4;
    public int PerThreeAttackFromLife = 2;
    public int HasTypeAdvantage = 4;
    public int HasEqualType = 2;


    public override void DoAction() {
        enemies = new List<Unit>();
        foreach (Unit u in GameManager.instance.Units)
        {
            if (u.Owner != Properties.thisUnit.Owner) {
                enemies.Add(u);
            }
        }

        Dictionary<Unit, int> PotentialTargets = new Dictionary<Unit, int>();
        KeyValuePair<Unit, int> ChosenEnemy = new KeyValuePair<Unit, int>();
        foreach (Unit eu in enemies) {
            PotentialTargets.Add( eu, transform.parent.GetComponentInChildren<SelectTarget>().EvaluateTarget(eu) );
        }

        if (PotentialTargets.Count < 1)
            return;

        ChosenEnemy = PotentialTargets.Aggregate((a, b) => a.Value > b.Value ? a : b);
        Properties.thisUnit.CurrentTarget = ChosenEnemy.Key;
        StartCoroutine( transform.parent.GetComponentInChildren<MoveToTarget>().Move() );
    }

    public override int GetScore() {
        if(!HasTarget) PScore += HasTargetScore;
        if( CheckForWeakerEnemyInProximity() ) PScore += HasWeakerEnemyNearby;
        PScore += AttacksFromDeath();

        TypeAdvantageCheck();
        if (HasTypeAdv)
            PScore += HasTypeAdvantage;
        else if (HasTypeSame)
            PScore += HasEqualType;

        //Debug.Log(
        //    "ME: " + Properties.thisUnit +
        //    "   ===================== Has Target: " + HasTarget +
        //    "   Has Weaker Enemy Nearby: " + CheckForWeakerEnemyInProximity() +
        //    "   attacks Per: " + AttacksFromDeath() +
        //    "   Has Better ADV: " + HasTypeAdv +
        //    "   Has Equal: " + HasTypeSame
        //    );
        //
        //Debug.Log("Search Score: " + PScore);
        return PScore;
    }

    private bool CheckForWeakerEnemyInProximity() {
        if (Properties.CurrentTrgt == null)
            return false;
        List<Unit> weakerEnemies = Properties.UnitsInProximity.FindAll(e => e.Owner != Properties.thisUnit.Owner && e.CurrentHP < Properties.CurrentTrgt.CurrentHP);

        if (weakerEnemies.Count >= 1)
            return true;
        else
            return false;
    }

    private int AttacksFromDeath() {
        // TODO: Refine the math, AND INCLUDE UNIT DEFENSE!!!!
        if (Properties.CurrentTrgt == null)
            return 0;
        List<Unit> enemiesNearby = Properties.UnitsInProximity.FindAll(e => e.Owner != Properties.thisUnit.Owner);
        List<Unit> enemiesTwoHitsFromDeath = enemiesNearby.FindAll(eu => eu.CurrentHP / Properties.AtkPower < 2f );
        List<Unit> enemiesOneHitsFromDeath = enemiesTwoHitsFromDeath.FindAll(wu => wu.CurrentHP / Properties.AtkPower < 1f);

        if (enemiesTwoHitsFromDeath.Count >= 1 && enemiesOneHitsFromDeath.Count <= 0) 
            return PerThreeAttackFromLife;
        else if (enemiesOneHitsFromDeath.Count >= 1)
            return PerThreeAttackFromLife * 2;
        else
            return 0;
    }

    private void TypeAdvantageCheck() {
        HasTypeAdv = false;
        HasTypeSame = false;
        List<Unit> enemiesNearby = Properties.UnitsInProximity.FindAll(e => e.Owner != Properties.thisUnit.Owner);
        List<Unit> TITAN = enemiesNearby.FindAll(titan => titan.UnitType == UnitType.Titan);
        List<Unit> TANKY = enemiesNearby.FindAll(tanks => tanks.UnitType == UnitType.Tanky);
        List<Unit> TINY = enemiesNearby.FindAll(tiny => tiny.UnitType == UnitType.Tiny);

        switch (Properties.CurrentType)
            {
            case UnitType.Titan:
                if (TINY.Count > 0 && Properties.CurrentTrgt.UnitType != UnitType.Tiny)
                    HasTypeAdv = true;
                else if (TITAN.Count > 0 && Properties.CurrentTrgt.UnitType == UnitType.Tanky)
                    HasTypeSame = true;
                break;

            case UnitType.Tanky:
                if (TITAN.Count > 0 && Properties.CurrentTrgt.UnitType != UnitType.Titan)
                    HasTypeAdv = true;
                else if (TANKY.Count > 0 && Properties.CurrentTrgt.UnitType == UnitType.Tiny )
                    HasTypeSame = true;
                break;

            case UnitType.Tiny:
                if (TANKY.Count > 0 && Properties.CurrentTrgt.UnitType != UnitType.Tanky)
                    HasTypeAdv = true;
                    
                else if (TINY.Count > 0 && Properties.CurrentTrgt.UnitType == UnitType.Titan)
                    HasTypeSame = true;
                break;
        }
    }

    void Start() {
        StartCoroutine( Init() );
    }

    IEnumerator Init() {
        Properties = transform.parent.parent.GetComponentInChildren<AIProperties>();
        yield return new WaitUntil(() => Properties.Initialized);
    }

}
