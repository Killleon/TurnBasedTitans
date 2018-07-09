using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BaseState {

    protected GameManager GManager;
    protected UIManager UIMan { get { return UIManager.instance; } }
    protected Transform ButtonsContainer { get { return UIMan.ButtonContainer.transform; } }
    protected GameObject Pointer { get { return UIMan.Pointer.transform.gameObject; } }
    protected bool Started = false;

    protected BaseState(GameManager _gm) {
        GManager = _gm;

    }

    /// <summary>
    /// Returns true if any Unit is currently moving.
    /// </summary>
    /// <returns></returns>
    public bool IsAnyoneMoving()
    {
        bool SomeoneIsMoving  = ( GManager.Units.FindAll(u => u.isMoving).Count > 0 );
        return SomeoneIsMoving;
    }

    public virtual void OnStateEnter()
    {

    }
    public virtual void OnStateExit()
    {
    }

    public virtual void ShowTilesInRng()
    {
    }

    #region Tile Related ________________________
    public virtual void OnTileSelected(Tile _tile)
    {
        _tile.OnSelected();
    }
    public virtual void OnTileHighlighted(Tile _tile)
    {
        _tile.OnHighlighted();
    }
    public virtual void OnTileDehighlighted(Tile _tile)
    {
        _tile.OnDefault();
    }
    #endregion


    #region Unit Related ________________________
    public virtual void OnUnitSelected(Unit _unit)
    {
    }
    public virtual void OnUnitDeselected(Unit _unit)
    {
    }
    public virtual void OnUnitHighlighted(Unit _unit)
    {
    }
    public virtual void OnUnitDehighlighted(Unit _unit)
    {
    }
    #endregion
}
