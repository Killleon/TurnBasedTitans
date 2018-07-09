using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIAgent : MonoBehaviour {

    public List<Actions> ActionsList;
    internal Actions CurrentAction;
    internal Actions PreviousAction;
    public Unit ToTest;
    private Dictionary<Unit, int> PotentialTargets;
    private KeyValuePair<Unit, int> ChosenEnemy;
    private KeyValuePair<Actions, int> ChosenAction;

    void OnEnable() {
        StartCoroutine( Init() );
    }

    private IEnumerator Init() {
        yield return new WaitUntil(() => transform.root.GetComponentInChildren<AIProperties>().Initialized);
    }

    public void Evaluate(Unit _currentAIUnit) {
        GameManager gm = GameManager.instance;
        List<Unit> Enemies = gm.Units.FindAll(u => u.Owner != _currentAIUnit.Owner);
        PotentialTargets = new Dictionary<Unit, int>();

        if (_currentAIUnit.CurrentTarget == null && Enemies.Count > 0) {
            foreach (Unit u in Enemies) {
                PotentialTargets.Add( u, ActionsList[4].EvaluateTarget(u) );
            }
            ChosenEnemy = PotentialTargets.Aggregate( (a, b) => a.Value > b.Value ? a : b );
            _currentAIUnit.CurrentTarget = ChosenEnemy.Key;
            GetComponentInChildren<MoveToTarget>().DoAction();
        }
        else {
            Dictionary<Actions, int> evaluatedActions = new Dictionary<Actions, int>();
            for (int i = 0; i < GetComponentsInChildren<Actions>().Count()-1; i++) {
                evaluatedActions.Add(ActionsList[i], ActionsList[i].GetScore());
            }
       
            Actions result = evaluatedActions.OrderByDescending(a => a.Value).ToList().First().Key;
            //Debug.Log("====================== " + _currentAIUnit + " RUNNING: " + result.name + " ======================");
            result.DoAction();
        }

        //ActionsList[0].GetScore();
        //ActionsList[1].GetScore();
        //ActionsList[2].GetScore();
        //ActionsList[3].GetScore();

        //Debug.Log( ActionsList[0].GetScore() );
        //Debug.Log( ActionsList[1].GetScore() );
        //Debug.Log( ActionsList[2].GetScore() );
        //Debug.Log( ActionsList[3].GetScore() );
        //Debug.Log( ActionsList[4].EvaluateTarget(ToTest) );
    }
}