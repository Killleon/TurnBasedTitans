using UnityEngine;
using System.Collections;
using DG.Tweening;

public class AITurnState : BaseState
{
    GameManager gm;

    public AITurnState(GameManager _gm) : base(_gm)
    {
        gm = _gm;
        PlayerManager.instance.transform.GetComponentInChildren<AI>().Play(_gm);
    }

    public override void OnStateEnter()
    {
        gm.DeselectCurrentUnitRef();
    }
    public override void OnStateExit()
    {
        Pointer.SetActive(true);
        //ButtonsContainer.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).OnComplete(() => ButtonsContainer.DOScale(new Vector3(0, 0, 0), 0.08f));
    }

    public override void OnTileSelected(Tile _tile)
    {
        return;
    }

    public override void OnUnitSelected(Unit _unit)
    {
        return;
    }

    #region Unit Related ________________________
    public override void OnUnitHighlighted(Unit _unit)
    {
        _unit.OnHighlighted();
    }
    public override void OnUnitDehighlighted(Unit _unit)
    {
        _unit._currentTile.OnDefault();
        _unit.OnDehighlighted();
    }
    #endregion
}
