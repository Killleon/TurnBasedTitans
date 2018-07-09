using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    internal Unit MyUnit;
    RectTransform CanvasRect;
    Vector3 UnitPos { get{ return new Vector3(MyUnit.transform.position.x, MyUnit.transform.position.y - 1f, MyUnit.transform.position.z); } }


    void Start () {
        CanvasRect = transform.parent.transform.parent.GetComponent<RectTransform>();
    }

    void Update()
    {
        GetComponent<Slider>().value = Helpers.NormalizedHealthValue(MyUnit.CurrentHP, MyUnit.MaxHP);

        if (MyUnit == null)
        {
            Destroy(gameObject);
        }
    }
	
	void LateUpdate () {
        if (MyUnit != null)
        {
            //From: http://answers.unity3d.com/questions/799616/unity-46-beta-19-how-to-convert-from-world-space-t.html

            Vector3 ViewportPosition = Camera.main.WorldToViewportPoint(UnitPos);

            Vector2 WorldObject_ScreenPosition = new Vector2(
                ( (ViewportPosition.x * CanvasRect.sizeDelta.x)-(CanvasRect.sizeDelta.x * 0.5f) ),
                ( (ViewportPosition.y * CanvasRect.sizeDelta.y)-(CanvasRect.sizeDelta.y * 0.5f) )
                );

            GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
        }
    }

    private float NormalizedHealthValue(float _currentHp, float _maxHP)
    {
        float result = (_currentHp / _maxHP) * 100;
        return result;
    }

}
