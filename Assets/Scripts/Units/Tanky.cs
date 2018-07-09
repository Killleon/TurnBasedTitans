using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tanky : Unit {

    public void Reset() {
        UnitType = UnitType.Tanky;
        MaxHP = 146;
        MaxMP = 20;
        Atk = 29; //totalDmg = Atk - Def;
        Hit = 90; //if (random(0, 100) < Hit - Eva)
        Rng = 1;
        Def = 12;
        Eva = 8;
        MaxSpd = 2;
    }

    protected override void Init() {
        CurrentHP = MaxHP;
        CurrentMP = MaxMP;
        CurrentSpd = MaxSpd;
    }
}