using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class AwaitState : BaseState {

    private List<Tile> TilesInRange;

    public AwaitState(GameManager gm) : base(gm)
    {

    }

    public override void OnStateEnter() {
        TilesInRange = new List<Tile>();
        ButtonsContainer.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).OnComplete(() => ButtonsContainer.DOScale(new Vector3(0, 0, 0), 0.08f));

        foreach (Tile t in GManager.Tiles) t.OnDefault();

        foreach (Unit u in GManager.Units)
        {
            if(!u.FinishedAttack)
                u.OnDefault();

            if (u.Owner != 0 && u.RedHighlighted)
                u.GetComponent<SpriteRenderer>().color = Color.white;
        }

        UIMan.MoveIcon.onClick.RemoveAllListeners();
        UIMan.AttackIcon.onClick.RemoveAllListeners();

    }
    public override void OnStateExit()
    {

    }


    private void ShowTilesInSpd(Unit _unit)
    {
        if ( _unit.FinishedAttack )
            return;

        foreach (Tile t in GManager.Tiles)
            t.OnDefault();

        _unit.UnitState = new UnitState_Move(GManager);

        TilesInRange = _unit.GetTilesLandable(GManager.Tiles);
        foreach (Tile t in TilesInRange)
        {
            t.OnShowSpdRange();
        }
    }


    #region Tile Related ________________________
    public override void OnTileSelected(Tile _tile)
    {
        _tile.OnSelected();
    }
    public override void OnTileDehighlighted(Tile _tile)
    {
        _tile.OnDefault();
    }
    public override void OnTileHighlighted(Tile _tile)
    {
        _tile.OnHighlighted();
    }
    public virtual void OnShowRange()
    {
    }
    public virtual void OnShowPath()
    {
    }
    #endregion


    #region Unit Related ________________________
    public override void OnUnitHighlighted(Unit _unit)
    {
        if (_unit.FinishedAttack)
            return;

        ShowTilesInSpd(_unit);
        _unit.OnHighlighted();
    }
    public override void OnUnitDehighlighted(Unit _unit)
    {
        foreach (Tile t in TilesInRange) { t.OnDefault(); }
        _unit.OnDehighlighted();
    }
    #endregion


}
