using UnityEngine;
using System.Collections;

public class Human : Player
{
    public override void Play(GameManager _gm)
    {
        _gm.BaseState = new AwaitState(_gm);
    }

    public override void ChooseUnits(GameManager _gm)
    {
        _gm.BaseState = new PreGameState(_gm, 0);
    }
}
