using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Retreat : Actions {

    [Header("Condition Scores")]
    public int IsLowOnHP = 7;
    public int HaveMoreUnits = -3;
    public int LowHpThreshold;

    public override void DoAction()
    {
        print("NOT YET!!!!!!!!!!!!");
    }

    public override int GetScore() {
        if (Properties.thisUnit.CurrentHP < LowHpThreshold)
            PScore += IsLowOnHP;

        List<Unit> enemiesNearby = Properties.UnitsInProximity.FindAll(e => e.Owner != Properties.thisUnit.Owner);
        List<Unit> alliesNearby = Properties.UnitsInProximity.FindAll(e => e.Owner == Properties.thisUnit.Owner);
        if (alliesNearby.Count - enemiesNearby.Count >= 0)
            PScore += HaveMoreUnits;

        //Debug.Log(
        //    "Numbers Advantage: " + (alliesNearby.Count - enemiesNearby.Count) +
        //    "   Low HP: " + (Properties.thisUnit.CurrentHP < LowHpThreshold)
        //    );

        return PScore;
    }


    void Start() {
        StartCoroutine(Init());
    }

    IEnumerator Init() {
        Properties = transform.parent.parent.GetComponentInChildren<AIProperties>();
        LowHpThreshold = Properties.thisUnit.MaxHP * (15 / 100);
        yield return new WaitUntil(() => Properties.Initialized);
    }
}
