using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MoveToTarget : Actions
{
    [Header("Condition Scores")]
    public int TargetInProximity = 5;
    public int TargetInAtkRange = 2;
    System.Random rng;

    public override void DoAction() {
        StartCoroutine( Move() );
    }

    public IEnumerator Move() {
        rng = new System.Random();
        Unit unit = Properties.thisUnit;
        Unit target = unit.CurrentTarget;
        GameManager gm = GameManager.instance;

        if (target.CurrentTile.GetNeighbours(gm.Tiles, unit.Rng).Contains(unit.CurrentTile)) {
            transform.parent.GetComponentInChildren<AttackTarget>().DoAction();
            yield break;
        }
            
        List<Tile> enemyNeighboors = target.CurrentTile.GetNeighbours(gm.Tiles, unit.Rng);
        Tile closestTargetNeighboor = GetDistanceToTarget(enemyNeighboors, unit.CurrentTile);
        List<Tile> tilesInRange = unit.GetTilesLandable(gm.Tiles);
        List<Tile> waywardTiles = unit.GetPath( gm.Tiles, enemyNeighboors.FindAll( t => !t.IsOccupied).FirstOrDefault() );
        List<Tile> possibleTiles = new List<Tile>();
        Tile finalDestinationTile;

        for (int i = 0; i < tilesInRange.Count; i ++) {
            if (waywardTiles.Contains(tilesInRange[i]))
                possibleTiles.Add(tilesInRange[i]);
        }

        Tile theMostPossibleTile = GetDistanceToTarget(possibleTiles, unit.CurrentTile);

        if ( tilesInRange.Contains(closestTargetNeighboor) ) {
            finalDestinationTile = closestTargetNeighboor;
        }
        else if (!tilesInRange.Contains(closestTargetNeighboor) && possibleTiles.Count <= 0) {
            finalDestinationTile = tilesInRange[rng.Next(0, tilesInRange.Count)];
        }
        else {
            finalDestinationTile = theMostPossibleTile;
        }

        List<Tile> thePath = unit.GetPath(gm.Tiles, finalDestinationTile);
        unit.Move(finalDestinationTile, thePath);

        yield return new WaitUntil( ()=> unit.CurrentTile == finalDestinationTile);

        if ( target.CurrentTile.GetNeighbours(gm.Tiles, unit.Rng).Contains(unit.CurrentTile) && !unit.FinishedAttack) {
            transform.parent.GetComponentInChildren<AttackTarget>().DoAction();
            yield break;
        }



    }

    public override int GetScore() {
        if (Properties.CurrentTrgt == null)
            return 0;

        if ( IsTargetInSpdDistance() )
            PScore += TargetInProximity;
        if ( CanIReachAttackRng() )
            PScore += TargetInAtkRange;

        //Debug.Log(
        //        "Target in Proximity: " + IsTargetInSpdDistance() +
        //        "   Can I Reach: " + CanIReachAttackRng() +
        //        "   Total Score: " + PScore
        //        );

        return PScore;
    }

    private Tile GetDistanceToTarget(List<Tile> _tiles, Tile _myTile) {
        Dictionary<Tile, int> distance = new Dictionary<Tile, int>();
        foreach (Tile t in _tiles) {
            distance.Add(t, t.GetTileDistance(_myTile));
        }

        Tile target = distance.OrderByDescending(a => a.Value).FirstOrDefault().Key;
        return target;
    }

    private bool IsTargetInSpdDistance() {
        if( Properties.UnitsInProximity.Contains(Properties.CurrentTrgt) )
            return true;
        else
            return false;
    }

    private bool CanIReachAttackRng() {
        Tile targetTile = Properties.CurrentTrgt.CurrentTile;
        List<Tile> potentialTile = targetTile.GetNeighbours(GameManager.instance.Tiles, Properties.thisUnit.Rng);
        if ( Properties.thisUnit.GetTilesLandable(GameManager.instance.Tiles).Any(potentialTile.Contains) )
            return true;
        else
            return false;
    }

    void Start() {
        StartCoroutine(Init());
    }

    IEnumerator Init() {
        Properties = transform.parent.parent.GetComponentInChildren<AIProperties>();
        yield return new WaitUntil(() => Properties.Initialized);
    }
}