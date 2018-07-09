using UnityEngine;
using System.Collections;

public abstract class Player : MonoBehaviour
{
    public int PlayerNumber;

    public abstract void Play(GameManager _gm);

    public abstract void ChooseUnits(GameManager _gm);

}
