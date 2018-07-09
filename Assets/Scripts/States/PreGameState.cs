using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class PreGameState : BaseState
{

    public PreGameState(GameManager gm, int _unitScore) : base(gm)
    {
    }

    public override void OnStateEnter()
    {
        
    }

    public override void OnStateExit()
    {
        SoundManager.instance.RunFadeOut();
        UIManager.instance.TransitionAnimation(0);

        foreach (Unit u in GManager.Units)
        {
            if (u.GetComponent<Draggable>() != null)
                u.DestroyDraggable();
        }
    }

    public override void OnTileHighlighted(Tile _tile)
    {
        _tile.OnHighlighted();
    }
    public override void OnTileDehighlighted(Tile _tile)
    {
        _tile.OnDefault();
    }

}
