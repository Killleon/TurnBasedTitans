using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI : Player {

    System.Random rng;
    private int AInumber;
    AISpawnSets Setter;
    List<System.Action> Formations = new List<System.Action>();

    void Awake()
    {
        Setter = new AISpawnSets();
        SpawnAIUnits();
    }

    public AI() {
    }

    public void SpawnAIUnits()
    {
        rng = new System.Random();
        Formations.AddMany(  
            ()=> Setter.Generic1(),
            ()=> Setter.Generic2(),
            ()=> Setter.TriangleFormation(),
            ()=> Setter.TinyTeam(),
            ()=> Setter.FourThreeSplit()
        );

        Formations[rng.Next(0,5)]();
    }

    public override void Play(GameManager _gm) {
        StartCoroutine( DefaultAI(_gm) );
        //_gm.BaseState = new AwaitState(_gm);
    }

    private IEnumerator DefaultAI(GameManager _gm) {
        if (PlayerManager.instance.transform.GetComponentInChildren<AI>() != null)
            AInumber = PlayerManager.instance.transform.GetComponentInChildren<AI>().PlayerNumber;

        List<Unit> AIunits = _gm.Units.FindAll(u => u.Owner == AInumber);
        List<Unit> EnemyUnits = _gm.Units.Except(AIunits).ToList();

        foreach ( Unit unit in AIunits.OrderBy(u => u.CurrentTile.GetLowestHPEnemyInRange(u, EnemyUnits, _gm.Tiles)).ToList() ) {
            if (unit == null || unit.CurrentTile.GetNeighbours(_gm.Tiles, 1).FindAll(unit.CanMoveThrough).Count <= 0)
                continue;

            unit.CurrentSpd = unit.MaxSpd;
            unit.GetComponent<AIAgent>().Evaluate(unit);

            while (unit.isMoving)
                yield return new WaitForSeconds(0.8f);
        }
        
        AIunits.Clear();
        EnemyUnits.Clear();

        yield return new WaitForSeconds(1.2f);
        StartCoroutine( _gm.SwitchPlayerTurn() );

        yield return null;
    }

    public override void ChooseUnits(GameManager _gm) {
        _gm.BaseState = new PreGameState(_gm, 0);
    }
}