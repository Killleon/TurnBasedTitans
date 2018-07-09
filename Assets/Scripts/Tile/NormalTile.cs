using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class NormalTile : Tile {

    public override void OnDefault()
    {
        base.OnDefault();
        GetComponent<Renderer>().material.color = DefaultColor;
    }

    public override void OnSelected()
    {
        base.OnSelected();

        if (EventSystem.current.IsPointerOverGameObject()) {
            Debug.Log("BLOCKED");
            return;
        }
        // TODO: Put a cursor pointing at the tile
        // MAYBE: Insert logic for unique tile types.
    }

    public override void OnHighlighted()
    {
        base.OnHighlighted();
        //GetComponent<Renderer>().material.color = new Color(0.75f, 0.75f, 0.75f);
    }

    public override void OnShowSpdRange()
    {
        GetComponent<Renderer>().material.color = new Color(0.1f, 0.1f, 1f, 0.5f);
    }

    public override void OnShowAtkRange()
    {
        GetComponent<Renderer>().material.color = new Color(1, 0.1f, 0.1f, 0.6f);
    }

    public override void OnShowPath()
    {
        GetComponent<Renderer>().material.color = Color.green;
    }
}
