using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UnitSelectedState : BaseState{

    private Unit SelectedUnit;
    private List<Tile> TilesInRange;
    private List<Tile> TilesInAllRange {
        get {
            if (SelectedUnit != null)
                return GManager.Tiles.FindAll(t => t.GetTileDistance(SelectedUnit.CurrentTile) <= SelectedUnit.Rng + SelectedUnit.MaxSpd);
            else
                return null;
        }
    }
    private List<Unit> EnemiesInRange {
        get
        {
            if (SelectedUnit != null)
                return SelectedUnit.CurrentTile.GetUnitsInAtkRange(SelectedUnit, GManager.Units, GManager.Tiles);
            else
                return null;
        }
    }

    public UnitSelectedState(GameManager _gm, Unit _unit) : base(_gm) {

        if (_unit.Owner == 0)
            SelectedUnit = _unit;
        else if(_unit.Owner == 1)
            Debug.Log("NOT MY GUY");
            
        UIMan.MoveIcon.onClick.RemoveAllListeners();
        UIMan.AttackIcon.onClick.RemoveAllListeners();
        UIMan.MoveIcon.onClick.AddListener(ShowTilesInSpdSelectedUnit);
        UIMan.AttackIcon.onClick.AddListener(ShowTilesInRng);
    }

    public override void OnStateEnter() {

        //TilesInRange = new List<Tile>();
        //if (!SelectedUnit.FinishedAttack)
        //    ButtonsContainer.DOScale( new Vector3(1.1f, 1.1f, 1.1f), 0.1f ).OnComplete(() => ButtonsContainer.DOScale(new Vector3(1, 1, 1), 0.05f));
    }
    public override void OnStateExit() {

    }

    private void ShowTilesInSpdSelectedUnit() {
        if (SelectedUnit == null || SelectedUnit.Owner != 0 || SelectedUnit.FinishedAttack)
            return;

        foreach (Tile t in GManager.Tiles)
            t.OnDefault();

        SelectedUnit.UnitState = new UnitState_Move(GManager);

        TilesInRange = SelectedUnit.GetTilesLandable(GManager.Tiles);
        foreach (Tile t in TilesInRange) {
            t.OnShowSpdRange();
        }
    }

    private void ShowTilesInSpd(Unit _unit)
    {
        if (_unit.Owner != 0 || _unit.FinishedAttack)
            return;

        foreach (Tile t in GManager.Tiles)
            t.OnDefault();

        _unit.UnitState = new UnitState_Move(GManager);

        TilesInRange = _unit.GetTilesLandable(GManager.Tiles);
        foreach (Tile t in TilesInRange)
            t.OnShowSpdRange();

        foreach (Tile t in SelectedUnit.GetAtkBorder(GManager.Tiles))
            t.OnShowAtkRange();
    }

    public override void ShowTilesInRng() {
        if (SelectedUnit == null || SelectedUnit.Owner != 0 || SelectedUnit.FinishedAttack)
            return;

        foreach (Tile t in GManager.Tiles)
            t.OnDefault();

        SelectedUnit.UnitState = new UnitState_Attack(GManager);

        TilesInRange = SelectedUnit.GetTilesInAtk(GManager.Tiles);
        foreach (Tile t in TilesInRange) {
            t.OnShowAtkRange();
        }
    }

    #region Tile Related ________________________
    public override void OnTileSelected(Tile _tile) {
        if (IsAnyoneMoving()) { return; }

        if ( TilesInRange.Contains(_tile) && SelectedUnit.UnitState.GetType().FullName == "UnitState_Move" ) {
            if (_tile.IsOccupied) { Debug.Log("Tile Occupied"); return; }

            List<Tile> MovementPath = SelectedUnit.GetPath(GManager.Tiles, _tile);
            SelectedUnit.Move(_tile, MovementPath);

            GManager.BaseState = new UnitSelectedState(GManager, SelectedUnit);
        }

        if (TilesInRange.Contains(_tile) && _tile.OccupyingUnit != null & SelectedUnit.UnitState.GetType().FullName == "UnitState_Attack") {
            Unit atkTarget = _tile.OccupyingUnit;
            SelectedUnit.Attack(SelectedUnit, atkTarget);
            GManager.BaseState = new UnitSelectedState(GManager, SelectedUnit);
        }

        GManager.DeselectCurrentUnitRef();
        GManager.BaseState = new AwaitState(GManager);
    }
    public override void OnTileHighlighted(Tile _tile) {
        if (IsAnyoneMoving()) { return; }
        _tile.OnHighlighted();
    }
    public override void OnTileDehighlighted(Tile _tile) {
        if (IsAnyoneMoving()) { return; }

        if (SelectedUnit != null)
            _tile.OnDefault();

        if (SelectedUnit.GetTilesLandable(GManager.Tiles).Contains(_tile) && SelectedUnit.UnitState.GetType().FullName == "UnitState_Move")
            _tile.OnShowSpdRange();
        if (SelectedUnit.GetAtkBorder(GManager.Tiles).Contains(_tile) && SelectedUnit.UnitState.GetType().FullName == "UnitState_Move")
            _tile.OnShowAtkRange();

        // This is for Special Attack mode in the future...
        //if (TilesInRange.Contains(_tile) && SelectedUnit.UnitState.GetType().FullName == "UnitState_Attack") {
        //    _tile.OnShowAtkRange();
        //}
    }
    #endregion

    
    #region Unit Related ________________________

    public override void OnUnitSelected(Unit _unit) {

        if (_unit.FinishedAttack || IsAnyoneMoving() || GManager.CurrentPlayer != 0)
            return;

        foreach (Unit u in GManager.Units)
        {
            if(!u.FinishedAttack)
            u.OnDefault();
        }
            
        foreach (Unit u in EnemiesInRange)
            u.OnAttackable();

        SelectionManager.instance.SelectObject(_unit);

        if (GManager.BaseState is UnitSelectedState) {
            foreach (Tile t in GManager.Tiles)
                t.OnDefault();
        }

        ShowTilesInSpd(_unit);
    }
    public override void OnUnitDeselected(Unit _unit) {
        foreach (Tile t in TilesInRange) t.OnDefault();
        foreach (Unit u in EnemiesInRange) u.OnDefault();

        _unit.OnDefault();
    }
    public override void OnUnitHighlighted(Unit _unit) {
        _unit.OnHighlighted();
    }
    public override void OnUnitDehighlighted(Unit _unit) {
        _unit.OnDehighlighted();
    }
    #endregion
}
