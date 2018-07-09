using UnityEngine;
using System.Collections;

public class UnitState {

    protected GameManager GManager;

    protected UnitState(GameManager _gm) {
        GManager = _gm;
    }

    public virtual void OnStateEnter() {
    }

    public virtual void OnStateExit() {
    }

}
