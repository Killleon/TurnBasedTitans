using UnityEngine;
using System.Collections;

public class Tiny : Unit {

    public void Reset()
    {
        UnitType = UnitType.Tiny;
        MaxHP = 85;
        MaxMP = 50;
        Atk = 36; //totalDmg = Atk - Def;
        Hit = 90; //if (random(0, 100) < Hit - Eva)
        Rng = 3;
        Def = 4;
        Eva = 9;
        MaxSpd = 3;
    }

    protected override void Init() {
        CurrentHP = MaxHP;
        CurrentMP = MaxMP;
        CurrentSpd = MaxSpd;
    }

}
