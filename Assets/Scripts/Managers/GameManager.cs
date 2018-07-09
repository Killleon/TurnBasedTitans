using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameManager : Singleton<GameManager>
{
    public UnitManager UnitManObj;
    public PlayerManager PlayerManObj;
    [Header("Textures")]
    public Texture DefaultTex;
    public Texture SeletectedTex;
    public Texture AllyTex;
    public Texture EnemyTex;

    internal List<Tile> Tiles { get { return transform.GetComponentsInChildren<Tile>().ToList(); } }
    internal List<Unit> Units { get { return UnitManObj.transform.GetComponentsInChildren<Unit>().ToList(); } }
    protected List<Player> Players { get { return PlayerManObj.transform.GetComponentsInChildren<Player>().ToList(); } }
    internal int CurrentPlayer { get; set; }
    protected Unit _currentUnitRef;
    private BaseState _base;
    public BaseState BaseState {
        get { return _base; }
        set {
            if (_base != null)
                _base.OnStateExit();
            _base = value;
            _base.OnStateEnter();
        }
    }
    internal int UnitScore;
    internal bool GameComplete;
    internal bool BattleBegan;
    internal bool TurnSwitched;

    void Awake(){
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 45;
    }

    public void InitAll()
    {
        InitializePlayers();
        InitializeTiles();
        InitializeUnits();
        GameComplete = false;
    }

	void Start () {
        BaseState = new PreGameState(this, UnitScore);
        SoundManager.instance.PlayPreGameBGM();
    }
    
    void Update() {
        if ( BaseState.GetType().FullName == "PreGameState")
        {
            if (Input.GetKeyDown(KeyCode.Space) && Units.FindAll(u => u.Owner == 0).Count >= 1)
            {
                StartCoroutine(BeginBattle());
                SoundManager.instance.PlayBattleBGM();
            }
            return;
        }

        if (SelectionManager.instance.CurrentUnit != null && Input.GetMouseButtonDown(1))
        {
            DeselectCurrentUnitRef();
            BaseState = new AwaitState(this);
        }

        if ( BattleBegan && CurrentPlayer == 0 && !TurnSwitched && Input.GetKeyDown(KeyCode.Space) )
            StartCoroutine( SwitchPlayerTurn() );

        if( !GameComplete )
            CheckUnitsLeft();
    }

    public IEnumerator BeginBattle()
    {
        UIManager.instance.PreGameBar.SetActive(false);
        UIManager.instance.Instructions.SetActive(false);

        foreach (Unit u in Units) {
            u.enabled = true;
            u.DestroyDraggable();
        }

        InitAll();
        UIManager.instance.HealthManager.InitializeHPBars(Units);
        
        BaseState = new AwaitState(this);

        yield return new WaitForSeconds(2.5f);
        BattleBegan = true;
    }

    public void CheckUnitsLeft() {
        // Lose
        if ( Units.FindAll(u => u.Owner == 0).Count <= 0 ) {
            GameComplete = true;
            UIManager.instance.EndGameScreen(1);
        }

        // Win
        if (Units.FindAll(u => u.Owner == 1).Count <= 0) {
            GameComplete = true;
            UIManager.instance.EndGameScreen(0);
            SoundManager.instance.RunFadeOut();
        }
    }

    void InitializeTiles() {
        foreach (Tile t in Tiles) {
            t.EventTileSelected += GM_OnTileSelected;
            t.EventTileHighlighted += GM_OnTileHighlighted;
            t.EventTileDehighlighted += GM_OnTileDehighlighted;
        }

        int _Ziterator = 0;
        int _Xiterator = 0;
        int _TileIterator = 0;
        while ( _Ziterator < Math.Sqrt(Tiles.Count) ) {
            transform.GetChild(_TileIterator).GetComponent<Tile>().GridSpace = new Vector2(_Xiterator, _Ziterator);

            if( (_Xiterator + 1) > Math.Sqrt(Tiles.Count)-1) {
                _Xiterator = 0;
                _Ziterator++;
            }
            else
                _Xiterator++;

            _TileIterator++;
        }
    }

    void InitializeUnits() {
        foreach (Unit u in Units) {
            u.EventUnitSelected += GM_OnUnitSelected;
            u.EventUnitDeselected += GM_OnUnitDeselected;
            u.EventUnitHighlighted += GM_OnUnitHighlighted;
            u.EventUnitDehighlighted += GM_OnUnitDehighlighted;
        }
    }

    void InitializePlayers() {
        CurrentPlayer = Players.Min(p => p.PlayerNumber);
        // MAYBE: Coin Toss to determine the player for first turn.
    }

    public void DeselectCurrentUnitRef() {
        _currentUnitRef = SelectionManager.instance.CurrentUnit;

        if (_currentUnitRef != null)
            SelectionManager.instance.DeselectObject(_currentUnitRef);
    }

    private IEnumerator AIWaitForTransition()
    {
        yield return new WaitForSeconds(2.3f);
        BaseState = new AITurnState(this);
    }

    public IEnumerator SwitchPlayerTurn() {
        TurnSwitched = true;

        foreach (Tile t in Tiles) { t.OnDefault(); }

        if ( Units.FindAll(u => u.Owner == 0).Count <= 0 || Units.FindAll(u => u.Owner == 1).Count <= 0)
            yield break;
            
        CurrentPlayer = (CurrentPlayer + 1) % Players.Count;

        if (CurrentPlayer != 0)
        {
            UIManager.instance.TransitionAnimation(2);
            StartCoroutine(AIWaitForTransition());
        }
        else
        {
            UIManager.instance.TransitionAnimation(1);
            BaseState = new AwaitState(this);
        }
            
        DeselectCurrentUnitRef();
        foreach (Unit u in Units)
        {
            u.OnDefault();

            if (u.Owner == CurrentPlayer)
            {
                u.CurrentSpd = u.MaxSpd;
                u.FinishedAttack = false;
                u.FinishedMoving = false;
            }
        }

        yield return new WaitForSeconds(2.5f);
        TurnSwitched = false;
    }


    #region Tile Related ________________________

    private void GM_OnTileSelected(object sender, EventArgs e) {
        if (SelectionManager.instance.CurrentUnit == null)
            return;

        BaseState.OnTileSelected(sender as Tile);
    }

    private void GM_OnTileHighlighted(object sender, EventArgs e) {
        BaseState.OnTileHighlighted(sender as Tile);
    }

    private void GM_OnTileDehighlighted(object sender, EventArgs e) {
        BaseState.OnTileDehighlighted(sender as Tile);
    }
    #endregion


    #region Unit Related ________________________

    private void GM_OnUnitSelected(object sender, EventArgs e) {

        Unit senderUnit = sender as Unit;
        Unit currentUnit = SelectionManager.instance.CurrentUnit;

        if (BaseState.GetType().FullName == "AITurnState" || sender.Equals(currentUnit) || senderUnit.Owner == 0  && senderUnit.FinishedAttack || Units.FindAll(u => u.isMoving).Count >= 1 )
            return;

        if ( currentUnit != null && currentUnit.CurrentTile.GetUnitsInAtkRange(currentUnit, Units, Tiles).Contains(senderUnit))
        {
            currentUnit.Attack(currentUnit, senderUnit);
            DeselectCurrentUnitRef();
            BaseState = new AwaitState(this);
            return;
        }

        if (senderUnit.Owner != 0)
            return;

        // For AI
        if (currentUnit != null && !currentUnit.FinishedAttack && currentUnit.Owner != senderUnit.Owner && currentUnit.CurrentTile.GetEnemiesInRange(currentUnit, Units, Tiles).Contains(senderUnit) && currentUnit.UnitState != null && currentUnit.UnitState.GetType().FullName == "UnitState_Attack") {
            currentUnit.Attack(currentUnit, sender as Unit);
            DeselectCurrentUnitRef();
            BaseState = new AwaitState(this);

            return;
        }

        BaseState = new UnitSelectedState(this, sender as Unit);
        BaseState.OnUnitSelected(sender as Unit);
    }

    private void GM_OnUnitDeselected(object sender, EventArgs e) {
        BaseState.OnUnitDeselected(sender as Unit);
    }

    private void GM_OnUnitHighlighted(object sender, EventArgs e) {
        BaseState.OnUnitHighlighted(sender as Unit);
    }

    private void GM_OnUnitDehighlighted(object sender, EventArgs e) {
        BaseState.OnUnitDehighlighted(sender as Unit);
    }
    #endregion
}