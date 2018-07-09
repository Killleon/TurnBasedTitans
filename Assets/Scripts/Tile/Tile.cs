using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, ISelectable {

    public bool IsOccupied;
    public Unit OccupyingUnit;
    public int CostSpd;
    internal Vector2 GridSpace;
    internal Color DefaultColor;

    public event EventHandler EventTileSelected;
    public event EventHandler EventTileHighlighted;
    public event EventHandler EventTileDehighlighted;
    List<Unit> enemyUnitsInRange;

    void Awake() {
        DefaultColor = GetComponent<Renderer>().material.color;
    }

    void Update()
    {
        //if (IsOccupied && !GameManager.instance.BattleBegan)
        //    GetComponent<Renderer>().material.color = new Color(0.522f, 0.929f, 0.125f, 0.4f);
        //else if (!IsOccupied && !GameManager.instance.BattleBegan)
        //    GetComponent<Renderer>().material.color = DefaultColor;

        if (IsOccupied && OccupyingUnit.Owner == 0)
        {
            GetComponent<MeshRenderer>().materials[1].mainTexture = GameManager.instance.AllyTex;
            GetComponent<MeshRenderer>().materials[1].color = new Color(0.1f, 0.1f, 1f, 0.7f);
        }
        else if (IsOccupied && OccupyingUnit.Owner == 1)
        {
            GetComponent<MeshRenderer>().materials[1].mainTexture = GameManager.instance.EnemyTex;
            GetComponent<MeshRenderer>().materials[1].color = new Color(1, 0.1f, 0.1f, 0.6f);
        }
    }

    public void DisplayPlacable()
    {
        if (!GameManager.instance.BattleBegan && gameObject.layer == 10)
        {
            GetComponent<MeshRenderer>().materials[1].mainTexture = GameManager.instance.DefaultTex;
            GetComponent<MeshRenderer>().materials[0].color = new Color(0.2f, 0.7f, 0.2f, 0.3f);
        }
    }

    public void SelectedTile()
    {
        if (GetComponent<MeshRenderer>().materials[1] == null)
            return;

        GetComponent<MeshRenderer>().materials[1].mainTexture = GameManager.instance.SeletectedTex;
        GetComponent<MeshRenderer>().materials[1].color = new Color(1, 1, 1, 1);
    }

    public void DeselectedTile()
    {
        if (GetComponent<MeshRenderer>().materials[1] == null)
            return;

        GetComponent<MeshRenderer>().materials[1].mainTexture = GameManager.instance.DefaultTex;
        GetComponent<MeshRenderer>().materials[1].color = new Color(0.1f, 0.1f, 0.1f, 1);
    }

    public List<Vector2> GetDirectionRange(int _range) {
        List<Vector2> testX = new List<Vector2>();
        for (int i = 1; i < _range+1; i++) {
            testX.Add(new Vector2(i, 0));
            testX.Add(new Vector2(-i, 0));
            testX.Add(new Vector2(0, i));
            testX.Add(new Vector2(0, -i));
        }

        return testX;
    }

    /// <summary>
    /// Each tile has four neighbors, which positions on grid are stored in _directions.
    /// </summary>
    /// <param name="cells"></param>
    /// <returns></returns>
    public List<Tile> GetNeighbours(List<Tile> _tiles, int _atkRng) {
        List<Vector2> _directions = GetDirectionRange(_atkRng);
        List<Tile> ret = new List<Tile>();
        foreach (Vector2 direction in _directions) {
            Tile neighbour = _tiles.Find( t => t.GridSpace == GridSpace + direction );
            if (neighbour == null) continue;

            ret.Add(neighbour);
        }

        return ret;
    }

    /// <summary>
    /// Returns a list of units in SPD range.
    /// </summary>
    public List<Unit> GetUnitsInAtkRange(Unit _myUnit, List<Unit> _units, List<Tile> _gmTiles) {
        List<Unit> unitsInRange = new List<Unit>();
        List<Tile> tilesInUnitRange = _myUnit.GetTilesInAtk(_gmTiles);

        foreach (Unit u in _units) {
            if (u.Owner != _myUnit.Owner && tilesInUnitRange.Contains(u.CurrentTile))
                unitsInRange.Add(u);
        }

        return unitsInRange;
    }

    /// <summary>
    /// Returns a list of enemy units within SPD range.
    /// </summary>
    public List<Unit> GetEnemiesInRange(Unit _myUnit, List<Unit> _enemyUnits, List<Tile> _gmTiles) {
        List<Unit> enemyUnitsInRange = new List<Unit>();
        List<Tile> myUnitRange = _myUnit.GetTilesInSpdRange(_gmTiles);

        foreach (Unit eu in _enemyUnits) {
            if ( myUnitRange.Contains(eu.CurrentTile) )
                enemyUnitsInRange.Add(eu);
        }

        return enemyUnitsInRange;
    }

    /// <summary>
    /// Returns the lowest HP amount of the nearest enemy unit.
    /// </summary>
    public int GetLowestHPEnemyInRange(Unit _myUnit, List<Unit> _enemyUnits, List<Tile> _gmTiles) {
        if ( GetEnemiesInRange(_myUnit, _enemyUnits, _gmTiles).Count <= 0 ) {
            //print("No enemy in range");
            List<Unit> enemyHPList = _enemyUnits.OrderBy(u => u.CurrentHP).ToList();
            return enemyHPList[enemyHPList.Count-1].MaxHP;
        }
        List<Unit> lowestHPEnemy = GetEnemiesInRange(_myUnit, _enemyUnits, _gmTiles).OrderByDescending(u => u.CurrentHP).ToList();
        Unit lowestHP = lowestHPEnemy[lowestHPEnemy.Count-1];
        return lowestHP.CurrentHP;
    }

    /// <summary>
    /// Returns the number of SPD cost from tile.
    /// </summary>
    public int GetTileDistance(Tile other) {
        return (int)Mathf.Abs(GridSpace.x - other.GridSpace.x) + (int)Mathf.Abs(GridSpace.y - other.GridSpace.y);
    }

    protected virtual void OnMouseEnter() {
        if (EventTileHighlighted != null)
            EventTileHighlighted.Invoke( this, new EventArgs() );
    }
    protected virtual void OnMouseExit(){
        if (EventTileDehighlighted != null)
            EventTileDehighlighted.Invoke( this, new EventArgs() );
    }
    protected virtual void OnMouseDown() {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (EventTileSelected != null)
            EventTileSelected.Invoke( this, new EventArgs() );
    }

    public virtual void OnDefault()
    {
        DeselectedTile();
    }

    public virtual void OnSelected() {
        // MAYBE: Insert logic for unique tile types.
    }

    public virtual void OnHighlighted()
    {
        SelectedTile();
    }

    public virtual void OnDehighlighted()
    {
    }

    public virtual void OnShowSpdRange()
    {
    }

    public virtual void OnShowAtkRange()
    {
    }

    public virtual void OnShowPath()
    {
    }
}