using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIProperties : MonoBehaviour {

    internal bool Initialized;
    internal int Health;
    internal List<Unit> UnitsInProximity;
    internal List<Tile> TilesInRange;
    internal int AtkPower;
    internal int AtkRange;
    internal bool Attacked;
    internal bool Moved;
    internal Unit CurrentTrgt { get { return thisUnit.CurrentTarget; } }
    internal UnitType CurrentType;
    internal List<Unit> Allies;
    internal List<Unit> Enemies; // This doesnt work. dno why.
    internal Unit thisUnit { get { return transform.parent.GetComponentInChildren<Unit>(); } }

    void OnEnable() {
        StartCoroutine( Init() );
    }

    IEnumerator Init() {
        yield return new WaitUntil( ()=> GameManager.instance.Units != null );
        yield return new WaitUntil(() => thisUnit.CurrentTile != null);
        GameManager gm = GameManager.instance;
        UnitsInProximity = new List<Unit>();
        Allies = gm.Units.FindAll(u => u.Owner == thisUnit.Owner);
        Enemies = gm.Units.FindAll(u => u.Owner != thisUnit.Owner);

        Health = thisUnit.CurrentHP;
        TilesInRange = gm.Tiles.FindAll(t => t.GetTileDistance(thisUnit.CurrentTile) <= thisUnit.CurrentSpd);
        foreach(Unit u in gm.Units) {
            if (TilesInRange.Contains(u.CurrentTile))
                UnitsInProximity.Add(u);
        }
        //UnitsInProximity = thisUnit.CurrentTile.GetUnitsInRange(thisUnit, gm.Units, gm.Tiles);
        AtkPower = thisUnit.Atk;
        AtkRange = thisUnit.Rng;
        Attacked = thisUnit.FinishedAttack;
        Moved = thisUnit.FinishedMoving;
        //CurrentTrgt = thisUnit.CurrentTarget;
        CurrentType = thisUnit.UnitType;
        Initialized = true;
    }
}
