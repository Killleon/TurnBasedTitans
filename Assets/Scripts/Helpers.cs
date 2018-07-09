using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UnitType
{
    Titan,
    Tiny,
    Tanky
}

public static class Helpers {

    public static void AddMany<T>(this List<T> list, params T[] elements)
    {
        list.AddRange(elements);
    }

    public static float NormalizedHealthValue(float _currentHp, float _maxHP)
    {
        float result = (_currentHp / _maxHP) * 100;
        return result;
    }

}
