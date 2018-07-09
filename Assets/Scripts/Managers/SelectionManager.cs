using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class SelectionManager : Singleton<SelectionManager> {

    public Unit CurrentUnit { get; set; }
    internal bool SomeoneIsMoving;
    private GameObject Pointer
    {
        get { return UIManager.instance.Pointer.transform.parent.gameObject; }
    }

    void Update()
    {
        if (CurrentUnit != null)
            UIManager.instance.SetPointerPosition(CurrentUnit);
    }

    public void SelectObject(Unit _selectedObj) {

        if (SomeoneIsMoving || _selectedObj.FinishedAttack)
            return;

        if (CurrentUnit != null)
        {
            CurrentUnit.CurrentTile.DeselectedTile();
            CurrentUnit.OnDefault();
        }

        CurrentUnit = _selectedObj;
        
        if (CurrentUnit != null)
        {
            CurrentUnit.CurrentTile.SelectedTile();
            Pointer.SetActive(true);
            //Pointer.transform.position = new Vector3(CurrentUnit.transform.position.x, CurrentUnit.transform.position.y + 1, CurrentUnit.transform.position.z);
        }

        Image unitFrameImage = UIManager.instance.UnitFrame.GetComponent<Image>();

        if (_selectedObj.UnitType == UnitType.Titan)
            unitFrameImage.sprite = UIManager.instance.Knight;
        else if (_selectedObj.UnitType == UnitType.Tanky)
            unitFrameImage.sprite = UIManager.instance.Tanky;
        else if (_selectedObj.UnitType == UnitType.Tiny)
            unitFrameImage.sprite = UIManager.instance.Lancer;

        unitFrameImage.SetNativeSize();
        UIManager.instance.UnitFrame.transform.parent.gameObject.SetActive(true);
        SoundManager.instance.PlayConfirm();
    }

    public void DeselectObject(Unit _selectedObj)
    {
        _selectedObj.CurrentTile.DeselectedTile();
        _selectedObj.OnDefault();
        CurrentUnit = null;
        UIManager.instance.UnitFrame.transform.parent.gameObject.SetActive(false);
        SoundManager.instance.PlayCancel();
    }

    void ShowToolTip() {

    }

    void ShowUnitInfo() {

    }

}
