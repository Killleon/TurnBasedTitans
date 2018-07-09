using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour {

    public GameObject MasterObj;
    public Sprite EnemyBar;
    public Text FCTDmg;
    RectTransform CanvasRect;

    void Start () {
        CanvasRect = transform.parent.GetComponent<RectTransform>();
    }

    public void InitializeHPBars(List<Unit> _unitList)
    {
        foreach (Unit u in _unitList)
        {
            GameObject cloneObj = Instantiate(MasterObj, transform) as GameObject;
            cloneObj.GetComponent<HealthBar>().MyUnit = u;
            u.HPBar = cloneObj.GetComponent<HealthBar>();
            cloneObj.GetComponent<Slider>().value = Helpers.NormalizedHealthValue(u.CurrentHP, u.MaxHP);

            if (u.Owner == 1)
                cloneObj.GetComponentInChildren<Slider>().fillRect.GetComponent<Image>().sprite = EnemyBar;
        }
    }

    private float NormalizedHealthValue(float _currentHp, float _maxHP)
    {
        float result = (_currentHp / _maxHP) * 100;
        return result;
    }

    public void DisplayDamage(int _dmg, Unit _victim, bool _hit)
    {
        if (_victim == null)
            return;

        Vector3 UnitPos = new Vector3(_victim.transform.position.x, _victim.transform.position.y, _victim.transform.position.z);
        Vector3 ViewportPosition = Camera.main.WorldToViewportPoint(UnitPos);

        Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f))
            );

        if (_hit)
            FCTDmg.text = _dmg.ToString();
        else
            FCTDmg.text = "MISS!";

        FCTDmg.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
        FCTDmg.gameObject.SetActive(true);
        FCTDmg.transform.DOLocalMoveY(92, 1.5f).SetEase(Ease.OutCubic).OnComplete( ()=>FCTDmg.gameObject.SetActive(false) );
    }

}
