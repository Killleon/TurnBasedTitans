using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Unit : MonoBehaviour, ISelectable {

    public int Owner;
    private Tile hitTile;
    public Tile _currentTile;
    public Tile CurrentTile {
        get {
            if (_currentTile != null)
                return _currentTile;
            else
                return null;
        }
        set {
            if(_currentTile != null)
                _currentTile.OccupyingUnit = null;
            _currentTile = value;
            _currentTile.OccupyingUnit = this;
        }
    }
    public UnitType UnitType;
    public List<UnitType> UnitAdvantage;
    public List<UnitType> UnitWeakness;

    public Unit _currentTarget;
    internal Unit CurrentTarget {
        get { return _currentTarget; }
        set {
            _currentTarget = value;
        }
    }

    private int _currentHP;
    public int CurrentHP {
        get { return _currentHP; }
        set {
            _currentHP = value;
            if (_currentHP <= 0) {
                //Destroy(gameObject);
            }
        }
    }

    public int MaxHP;
    internal int CurrentMP;
    public int MaxMP;
    public int Atk;
    public int Hit;
    public int Rng;
    public int Def;
    public int Eva;
    internal int CurrentSpd;
    public int MaxSpd;

    internal Unit SelectedUnit {
        get {
            if (SelectionManager.instance.CurrentUnit != null)
                return SelectionManager.instance.CurrentUnit;
            else
                return null;
            }
    }

    private bool _isMoving;
    public bool isMoving {
        get { return _isMoving; }
        set {
            _isMoving = value;
            SelectionManager.instance.SomeoneIsMoving = value;
        }
    }
    internal bool FinishedMoving;
    internal bool FinishedAttack;
    internal bool BeingTargetted;
    internal bool IsPlaced;
    internal bool RedHighlighted;
    public int UnitScore;
    internal HealthBar HPBar;
    System.Random random;
    private int FinalDamage;

    private PathFinder PathFind = new PathFinder();
    public event EventHandler EventUnitSelected;
    public event EventHandler EventUnitDeselected;
    public event EventHandler EventUnitHighlighted;
    public event EventHandler EventUnitDehighlighted;

    private UnitState _uniteState;
    public UnitState UnitState {
        get { return _uniteState; }
        set {
            if (_uniteState != null)
                _uniteState.OnStateExit();
            _uniteState = value;
            _uniteState.OnStateEnter();
        }
    }

    void Start()
    {
        
    }

    private void Update()
    {
        if (CurrentHP <= 0)
            Destroy(gameObject);
    }

    private GameObject Pointer {
        get { return UIManager.instance.Pointer.transform.parent.gameObject; }
    }

    void OnEnable() {
        Init();
    }

    public Tile GetTileBeneath()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit)) {
            hitTile = hit.collider.gameObject.GetComponent<Tile>();
        }

        return hitTile;
    }

    protected virtual void Init() {
        print(CurrentTile);
    }

    protected virtual void OnMouseEnter() {

        if (EventUnitHighlighted != null)
            EventUnitHighlighted.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseExit() {
        if (SelectionManager.instance.CurrentUnit == this)
            return;

        if (EventUnitDehighlighted != null)
            EventUnitDehighlighted.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseDown() {
        if (SelectionManager.instance.CurrentUnit == this || EventSystem.current.IsPointerOverGameObject())
            return;

        if (EventUnitSelected != null)
            EventUnitSelected.Invoke(this, new EventArgs());
    }

    #region Pathfinding Related ________________________

    ///<summary>
    /// Method indicates if unit is capable of moving to cell given as parameter.
    /// </summary>
    public virtual bool CanMoveTo(Tile _tile) {
        return !_tile.IsOccupied;
    }

    /// <summary>
    /// Method indicates if unit is capable of moving through cell given as parameter.
    /// </summary>
    public virtual bool CanMoveThrough(Tile _tile) {
        return !_tile.IsOccupied;
    }

    /// <summary>
    /// Returns a list of Tiles that are within Spd value AND can be landed on
    /// </summary>
    /// <param name="_tiles"></param>
    /// <returns></returns>
    public List<Tile> GetTilesLandable(List<Tile> _tiles) {
        List<Tile> ret = new List<Tile>();
        List<Tile> TraversableTiles = _tiles.FindAll(t => CanMoveThrough(t) && t.GetTileDistance(CurrentTile) <= CurrentSpd); //filter out cells that are within movement points and can be moving through.
        TraversableTiles.Add(CurrentTile);

        foreach (Tile _tileInRange in GetTilesInSpdRange(_tiles))
        {
            if (_tileInRange.Equals(CurrentTile)) continue;

            List<Tile> path = GetPath(TraversableTiles, _tileInRange);
            int pathCost = path.Sum(c => c.CostSpd);
            if (pathCost > 0 && pathCost <= CurrentSpd)
                ret.AddRange(path);
        }

        return ret.FindAll(CanMoveTo).Distinct().ToList();
    }

    /// <summary>
    /// Returns a list of Tiles that are within MaxSpd value, regardless of whether it can be landed on
    /// </summary>
    /// <param name="_tiles"></param>
    /// <returns></returns>
    public List<Tile> GetTilesInSpdRange(List<Tile> _tiles) {
        List<Tile> TilesInRange = _tiles.FindAll(t => t.GetTileDistance(CurrentTile) <= CurrentSpd);

        return TilesInRange;
    }

    /// <summary>
    /// Returns a list of Tiles that are within Rng value
    /// </summary>
    public List<Tile> GetTilesInAtk(List<Tile> _tiles) {
        List<Tile> TilesInRange = _tiles.FindAll(t => t.GetTileDistance(CurrentTile) <= Rng);

        return TilesInRange;
    }

    public List<Tile> GetAtkBorder(List<Tile> _tiles)
    {
        List<Tile> AtkBorder = new List<Tile>();
        List<Tile> OutsideTiles = _tiles.FindAll(t => t.GetTileDistance(CurrentTile) <= Rng + CurrentSpd);

        foreach (Tile t in OutsideTiles)
        {
            if ( !GetTilesInSpdRange(_tiles).Contains(t))
                AtkBorder.Add(t);
        }
        return AtkBorder;
    }

    /// <summary>
    /// Returns a path based on A* pathfinding with JPS using HeapPriorityQueue
    /// </summary>
    /// <param name="_tiles">_tiles should be the main List of tiles from GM</param>
    /// <param name="_destination"></param>
    /// <returns></returns>
    public List<Tile> GetPath(List<Tile> _tiles, Tile _destination) {
        return PathFind.FindPath(GetGraphEdges(_tiles), CurrentTile, _destination);
    }

    /// <summary>
    /// Method returns graph representation of cell grid for pathfinding.
    /// </summary>
    protected virtual Dictionary<Tile, Dictionary<Tile, int>> GetGraphEdges(List<Tile> _tiles) {
        Dictionary<Tile, Dictionary<Tile, int>> Graph = new Dictionary<Tile, Dictionary<Tile, int>>();
        foreach (Tile t in _tiles) {
            if (CanMoveThrough(t) || t.Equals(CurrentTile)) {
                Graph[t] = new Dictionary<Tile, int>();
                foreach (Tile neighbour in t.GetNeighbours(_tiles, 1).FindAll(CanMoveThrough)) {
                    Graph[t][neighbour] = neighbour.CostSpd;
                }
            }
        }
        return Graph;
    }
    #endregion

    #region Movement Related ________________________

    public virtual void Move(Tile _destination, List<Tile> _path) {
        if (isMoving)
            return;
        int movementCost = _path.Sum(h => h.CostSpd);
        if (CurrentSpd < movementCost)
            return;

        StartCoroutine(MoveAnimation(_path, _destination));
    }


    private void ColorTile(Tile _tile)
    {
        if (Owner == 0)
        {
            _tile.GetComponent<MeshRenderer>().materials[1].mainTexture = GameManager.instance.AllyTex;
            _tile.GetComponent<MeshRenderer>().materials[1].color = new Color(0.1f, 0.1f, 1f, 0.7f);
        }
        else if (Owner == 1)
        {
            _tile.GetComponent<MeshRenderer>().materials[1].mainTexture = GameManager.instance.EnemyTex;
            _tile.GetComponent<MeshRenderer>().materials[1].color = new Color(1, 0.1f, 0.1f, 0.6f);
        }
    }

    public virtual IEnumerator MoveAnimation(List<Tile> _path, Tile _destination) {
        isMoving = true;
        _path.Reverse();

        CurrentTile.IsOccupied = false;
        Tile currentTile;
        Tile previousTile = null;
        foreach (Tile tile in _path)
        {
            currentTile = tile;
            if (previousTile != null)
                previousTile.OnDefault();

            Vector3 tilePos = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.8f, tile.transform.position.z);
            while (new Vector3(transform.position.x, transform.position.y, transform.position.z) != tilePos) {
                transform.position = Vector3.MoveTowards(transform.position, tilePos, Time.deltaTime * 8);
                ColorTile(tile);
                previousTile = tile;
                yield return null;
            }
        }

        isMoving = false;
        yield return new WaitUntil(() => !isMoving);
        
        _destination.IsOccupied = true;
        CurrentTile = _destination;
        CurrentSpd = 0;
        FinishedMoving = true;

        foreach (Tile t in GameManager.instance.Tiles) {
            t.OnDefault();
        }

        List<Unit> test = CurrentTile.GetUnitsInAtkRange(this, GameManager.instance.Units, GameManager.instance.Tiles);
        if (FinishedMoving && test.Count > 0) {
            GameManager.instance.BaseState.ShowTilesInRng();
        }

    }

    #endregion

    public virtual void Attack(Unit _me, Unit _enemy) {
        if (isMoving)
            return;
        StartCoroutine(AttackAnimation(_me, _enemy));

    }

    /// <summary>
    /// 0 = No Change,
    /// 1 = Have Advantage,
    /// 2 = Have Disadvantage
    /// </summary>
    /// <param name="_enemy"></param>
    /// <returns></returns>
    private int HaveTypeAdvantage(Unit _enemy)
    {
        switch (UnitType)
        {
            case UnitType.Titan:
                {
                    if (_enemy.UnitType == UnitType.Tiny)
                        return 1;
                    else if (_enemy.UnitType == UnitType.Tanky)
                        return 2;
                    else
                        return 0;
                }
            case UnitType.Tanky:
                {
                    if (_enemy.UnitType == UnitType.Titan)
                        return 1;
                    else if (_enemy.UnitType == UnitType.Tiny)
                        return 2;
                    else
                        return 0;
                }
            case UnitType.Tiny:
                {
                    if (_enemy.UnitType == UnitType.Tanky)
                        return 1;
                    else if (_enemy.UnitType == UnitType.Titan)
                        return 2;
                    else
                        return 0;
                }
            default:
                return 0;
        }
    }

    public virtual IEnumerator AttackAnimation(Unit _me, Unit _enemy) {
        if (_enemy == null)
            yield break;

        isMoving = true;

        Vector3 originalPos = _me.transform.position;
        Vector3 targetPos = _enemy.transform.position;
        Quaternion originalRot = _me.transform.rotation; // Will use this later
        Quaternion targetRot = _enemy.transform.rotation; // Will use this later

        while (transform.position != targetPos) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 8);
            yield return null;
        }

        random = new System.Random();
        int hitNumber = random.Next(0, 100);
        if (hitNumber <= (_me.Hit - _enemy.Eva))
        {
            SoundManager.instance.PlayAttack();

            if (HaveTypeAdvantage(_enemy) == 0)
            {
                FinalDamage = (_me.Atk - _enemy.Def);
            }
            else if (HaveTypeAdvantage(_enemy) == 1)
            {
                FinalDamage = ((int)(_me.Atk * 1.5f) - _enemy.Def);
            }
            else if (HaveTypeAdvantage(_enemy) == 2)
            {
                FinalDamage = ((int)(_me.Atk / 1.5f) - _enemy.Def);
            }
            else
                print("nope");

            _enemy.CurrentHP -= FinalDamage;
            UIManager.instance.HealthManager.DisplayDamage(FinalDamage, _enemy, true);
        }
        else
        {
            SoundManager.instance.PlayCancel();
            UIManager.instance.HealthManager.DisplayDamage(_me.Atk, _enemy, false);
        }

        while (transform.position != originalPos) {
            transform.position = Vector3.MoveTowards(transform.position, originalPos, Time.deltaTime * 6);
            yield return null;
        }

        isMoving = false;

        while (isMoving)
            yield return new WaitForSeconds(1.5f);

        FinishedAttack = true;
        GetComponent<SpriteRenderer>().color = new Color(0.60f, 0.60f, 0.60f);
        yield return null;
    }

    public virtual void OnDefault() {
        RedHighlighted = false;
        Pointer.SetActive(false);
        HPBar.gameObject.SetActive(false);
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    public virtual void OnUnitSelected() {
        if (FinishedAttack)
            return;

        GetComponent<SpriteRenderer>().color = new Color(0.75f, 0.75f, 0.75f);
    }
    public virtual void OnHighlighted() {
        HPBar.gameObject.SetActive(true);

        if (SelectedUnit == null)
        {
            Pointer.SetActive(true);
            SoundManager.instance.PlayCursor();
            UIManager.instance.SetPointerPosition(this);
            //Pointer.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        }

        if (FinishedAttack)
            return;

        if (!RedHighlighted)
            GetComponent<SpriteRenderer>().color = new Color(0.80f, 0.80f, 0.80f);
    }
    public virtual void OnDehighlighted() {
        HPBar.gameObject.SetActive(false);

        if (GameManager.instance.CurrentPlayer == 0 && !RedHighlighted)
            GetComponent<SpriteRenderer>().color = Color.white;

        if (FinishedAttack)
            return;

        if (SelectionManager.instance.CurrentUnit != null)
            return;

        Pointer.SetActive(false);
    }
    public virtual void OnAttackable() {
        RedHighlighted = true;
        GetComponent<SpriteRenderer>().color = new Color(0.80f, 0.35f, 0.50f);
    }

    public void DestroyDraggable()
    {
        if(GetComponent<Draggable>() != null)
            Destroy( GetComponent<Draggable>() );
    }

    void OnDestroy()
    {
        if(CurrentTile != null) {
            CurrentTile.OccupyingUnit = null;
            CurrentTile.IsOccupied = false;
        }
    }
}
