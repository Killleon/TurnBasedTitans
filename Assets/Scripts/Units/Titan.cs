using UnityEngine;
using System.Collections;

public class Titan : Unit {

    public void Reset() {
        UnitType = UnitType.Titan;
        MaxHP = 100;
        MaxMP = 50;
        Atk = 32; //totalDmg = Atk - Def;
        Hit = 92; //if (random(0, 100) < Hit - Eva)
        Rng = 1;
        Def = 8;
        Eva = 9;
        MaxSpd = 4;
    }

    protected override void Init() {
        CurrentHP = MaxHP;
        CurrentMP = MaxMP;
        CurrentSpd = MaxSpd;
    }

}
