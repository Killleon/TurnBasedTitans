using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

    private int ScreenWidth { get { return Screen.width; } }
    private int ScreenHeight { get { return Screen.height; } }
    private int Boundary { get { return 20; } }
    private float Speed { get { return 10f; } }
    private Transform CameraTrans;
    RectTransform CanvasRect;

    public Font UnitScoreFont;
    private Unit CurrentUnit {
        get {
            if (SelectionManager.instance.CurrentUnit != null)
                return SelectionManager.instance.CurrentUnit;
            else
                return null;
        }
    }

    [Header("Unit Buttons")]
    public GameObject ButtonContainer;
    public Button AttackIcon;
    public Button MoveIcon;

    [Header("PreGame")]
    public Text UnitScore;
    public GameObject PreGameBar;
    public GameObject Instructions;

    [Header("Transitions")]
    public GameObject TransitionTextObj;
    public GameObject BlackScreen;
    public Sprite BattleBegin;
    public Sprite PlayerTurn;
    public Sprite EnemyTurn;
    public Sprite Victory;
    public Sprite Defeat;
    private Vector2 OriginalX;

    [Header("Others")]
    public GameObject Pointer;

    [Header("UnitFrames")]
    public GameObject UnitFrame;
    public Slider HPSlider;
    public Text HPValue;
    public Sprite Knight;
    public Sprite Tanky;
    public Sprite Lancer;

    [Header("HealthBars")]
    public HealthBarManager HealthManager;

    private Transform FinalTransform;

    void Start()
    {
        OriginalX = TransitionTextObj.transform.localPosition;
        ButtonContainer.transform.localScale = new Vector3(0, 0, 0);
        StartCoroutine( Init() );
        UnitScore.font = UnitScoreFont;
        PointerAnimation();
        CameraTrans = Camera.main.transform;
        CanvasRect = GetComponent<RectTransform>();
    }

    IEnumerator Init()
    {
        yield return new WaitUntil( ()=> SelectionManager.instance.CurrentUnit != null);
        FinalTransform = SelectionManager.instance.CurrentUnit.transform;
    }

    void Update()
    {
        UnitScore.text = GameManager.instance.UnitScore.ToString();

        if (SelectionManager.instance.CurrentUnit != null)
            UnitFrame.transform.parent.GetComponentInChildren<Slider>().value = Helpers.NormalizedHealthValue( CurrentUnit.CurrentHP, CurrentUnit.MaxHP );

        if (SelectionManager.instance.CurrentUnit != null && FinalTransform != null) {
            ButtonContainer.transform.position = new Vector3(FinalTransform.position.x + -1f, FinalTransform.position.y - 0.4f, FinalTransform.position.z);
            FinalTransform = SelectionManager.instance.CurrentUnit.transform;
        }

        if(SelectionManager.instance.CurrentUnit != null)
            HPValue.text = SelectionManager.instance.CurrentUnit.CurrentHP.ToString();

        PanCamera();

    }

    private void PanCamera()
    {
        if (Input.mousePosition.x >= (ScreenWidth - Boundary) && CameraTrans.position.x <= 16)
            CameraTrans.Translate(Vector2.right * Speed * Time.deltaTime);
        if (Input.mousePosition.x <= (0 + Boundary) && CameraTrans.position.x >= 7)
            CameraTrans.Translate(Vector2.left * Speed * Time.deltaTime);
        if (Input.mousePosition.y >= (ScreenHeight - Boundary) && CameraTrans.position.y <= 6)
            CameraTrans.Translate(Vector2.up * Speed * Time.deltaTime);
        if (Input.mousePosition.y <= (0 + Boundary) && CameraTrans.position.y >= 1)
            CameraTrans.Translate(Vector2.down * Speed * Time.deltaTime);
    }

    /// <summary>
    /// 0=BattlbeBegins,
    /// 1=PlayerTurn,
    /// 2=EnemyTurn,
    /// </summary>
    public void TransitionAnimation(int _transitionType)
    {
        if (_transitionType < 0 || _transitionType > 2)
        {
            Debug.LogError("Argument must be an integer between 0 - 2");
            return;
        }

        Image transTxt = TransitionTextObj.GetComponent<Image>();
        switch (_transitionType)
        {
            case 0:
                transTxt.sprite = BattleBegin;
                break;
            case 1:
                transTxt.sprite = PlayerTurn;
                break;
            case 2:
                transTxt.sprite = EnemyTurn;
                break;
        }

        SoundManager.instance.PlayTransiton();

        TransitionTextObj.GetComponent<Image>().SetNativeSize();
        Sequence transitionSequence = DOTween.Sequence();
        Tween In = TransitionTextObj.transform.DOLocalMoveX(4, 0.35f).SetEase(Ease.OutCirc);
        Tween Mid = TransitionTextObj.transform.DOLocalMoveX(-16, 0.8f).SetEase(Ease.OutCirc);
        Tween Out = TransitionTextObj.transform.DOLocalMoveX( -(Screen.width/2 + 200), 0.35f).SetEase(Ease.InOutCirc);
        Tween blackIn = BlackScreen.GetComponent<Image>().DOFade(0.4f, 0.35f);
        Tween blackOut = BlackScreen.GetComponent<Image>().DOFade(0, 0.35f);

        BlackScreen.SetActive(true);
        TransitionTextObj.SetActive(true);

        transitionSequence.Append(blackIn)
            .Append(In)
            .Append(Mid)
            .Append(Out)
            .Append(blackOut)
            .OnComplete( ()=> ResetTransitionElements() );
    }

    /// <summary>
    /// 0=Win!!!,
    /// 1=Lose...
    /// </summary>
    public void EndGameScreen(int _winOrLose)
    {
        if (_winOrLose < 0 || _winOrLose > 1)
        {
            Debug.LogError("Argument must be an integer between 0 - 1");
            return;
        }

        Image transTxt = TransitionTextObj.GetComponent<Image>();
        switch (_winOrLose)
        {
            case 0:
                transTxt.sprite = Victory;
                break;
            case 1:
                transTxt.sprite = Defeat;
                break;
        }

        TransitionTextObj.GetComponent<Image>().SetNativeSize();
        TransitionTextObj.GetComponent<Image>().DOFade(0f, 0f);
        TransitionTextObj.transform.DOLocalMoveX(0, 0f);
        Tween Activate = TransitionTextObj.GetComponent<Image>().DOFade(1f, 6f);
        Tween blackIn = BlackScreen.GetComponent<Image>().DOFade(0.7f, 6f);

        BlackScreen.SetActive(true);
        TransitionTextObj.SetActive(true);
    }

    private void ResetTransitionElements()
    {
        BlackScreen.SetActive(false);
        TransitionTextObj.SetActive(false);
        TransitionTextObj.transform.localPosition = OriginalX;
    }

    private void PointerAnimation()
    {
        TweenParams tParms = new TweenParams().SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InQuad);
        Pointer.transform.DOLocalMoveY(25, 0.5f).SetAs(tParms);
    }

    public void SetPointerPosition(Unit _selectedUnit)
    {
        Vector3 unitPos = new Vector3(_selectedUnit.transform.position.x, _selectedUnit.transform.position.y + 1f, _selectedUnit.transform.position.z);
        if (_selectedUnit != null)
        {
            //From: http://answers.unity3d.com/questions/799616/unity-46-beta-19-how-to-convert-from-world-space-t.html

            Vector3 ViewportPosition = Camera.main.WorldToViewportPoint(unitPos);

            Vector2 WorldObject_ScreenPosition = new Vector2(
                ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
                ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f))
                );

            Pointer.transform.parent.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
        }
    }
}
