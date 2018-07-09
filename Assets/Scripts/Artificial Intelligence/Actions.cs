using UnityEngine;
using System.Collections;

public class Actions : MonoBehaviour {

    internal AIProperties Properties;
    protected GameManager gm { get { return GameManager.instance; } }
    internal int PScore;

    public virtual void DoAction()
    {

    }

    public virtual int GetScore()
    {
        return 0;
    }

    public virtual int EvaluateTarget(Unit _unit)
    {
        return 0;
    }
}