using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Draggable : MonoBehaviour {
    private Color SelectedColor = new Color(0.35f, 0.52f, 1f);
    private Color OriginalColor = Color.white;
    private Color RefuseColor = new Color(0.82f, 0.25f, 0.40f);
    private bool Dragging = false;
    private float TheDistance;
    private Vector3 OriginalPos;
    private Vector3 OriginalScale;
    private Tile CurrentTile;
    private Tile PreviousTile;
    private GameObject CloneObj;
    public LayerMask LayMasks;
    private List<Tile> Tiles { get { return GameManager.instance.Tiles; } }

    void OnMouseEnter() {

        if (!Dragging) {
            OriginalScale = gameObject.transform.localScale;
            gameObject.transform.localScale *= 1.05f;
        }

        if(GetComponent<Renderer>() != null && gameObject.layer != 9)
            GetComponent<Renderer>().material.color = SelectedColor;

    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RemoveUnit();
        }
    }

    void OnMouseExit() {
        if ( transform.parent == UnitManager.instance.gameObject.transform )
        {
            gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }
        else
            gameObject.transform.localScale = OriginalScale;
        if (GetComponent<Renderer>() != null && gameObject.layer != 9)
            GetComponent<Renderer>().material.color = OriginalColor;
    }

    void OnMouseDown() {
        if(CurrentTile != null) {
            CurrentTile.IsOccupied = false;
        }
            
        TheDistance = Vector3.Distance(transform.position, new Vector3(Camera.main.transform.position.x, 0.8f, Camera.main.transform.position.z));
        GetComponent<SpriteRenderer>().sortingOrder = 2;
        gameObject.layer = 8;

        if (CurrentTile == null) {
            CloneObj = Instantiate(Resources.Load(gameObject.name), transform.position, transform.rotation) as GameObject;
            CloneObj.transform.parent = transform.parent.transform;
            CloneObj.transform.localScale = new Vector3(36f, 36f, 36f);
            CloneObj.layer = 5;
            CloneObj.name = gameObject.name;
        }

        Dragging = true;

        
        foreach (Tile t in Tiles)
        {
            if (!GameManager.instance.BattleBegan && t.gameObject.layer == 10)
            {
                t.DisplayPlacable();
            }
        }
    }

    void OnMouseUp()
    {
        Dragging = false;

        if (CurrentTile == null || transform.position == new Vector3(0, 0, 0))
            RemoveUnit();

        if (GetComponent<Renderer>() != null)
            GetComponent<Renderer>().material.color = OriginalColor;

        if (CurrentTile != null && !CurrentTile.IsOccupied)
        {


            OriginalPos = transform.position;
            CurrentTile.IsOccupied = true;
            PreviousTile = CurrentTile;
            GetComponent<Unit>().CurrentTile = CurrentTile;

            if (GetComponent<Unit>().IsPlaced == false) {
                GameManager.instance.UnitScore += gameObject.GetComponent<Unit>().UnitScore;
                GetComponent<Unit>().IsPlaced = true;
            }
        }
        else if (CurrentTile != null && CurrentTile.IsOccupied)
        {
            CheckForRemoval();
            CurrentTile = PreviousTile;
            if(PreviousTile != null)
                PreviousTile.IsOccupied = true;
        }
        else
            CheckForRemoval();
            
        if (GameManager.instance.UnitScore > 25)
            RemoveUnit();

        GetComponent<SpriteRenderer>().sortingOrder = 1;
        gameObject.layer = 9;

        foreach (Tile t in Tiles)
        {
            if (!GameManager.instance.BattleBegan && t.gameObject.layer == 10)
                t.OnDefault();
        }
    }

    void RemoveUnit() {
        if( OriginalPos != new Vector3(0, 0, 0) )
            GameManager.instance.UnitScore -= gameObject.GetComponent<Unit>().UnitScore;
        if(CurrentTile != null)
            CurrentTile.IsOccupied = false;
        Destroy(gameObject);
    }

    void CheckForRemoval() {
        if (OriginalPos == new Vector3(0, 0, 0))
            Destroy(gameObject);
        else
            transform.position = OriginalPos;
    }

    void Update() {
        if (!Dragging)
            return;

        if (GetComponent<Renderer>() != null)
            GetComponent<Renderer>().material.color = SelectedColor;

        if (CurrentTile != null && CurrentTile.IsOccupied && gameObject.layer != 9 || CurrentTile == null)
            GetComponent<Renderer>().material.color = RefuseColor;

        if (transform.parent != UnitManager.instance.gameObject.transform)
            transform.parent = UnitManager.instance.gameObject.transform;

        transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;
        hits = Physics.RaycastAll( ray, 100.0F, LayMasks);

        if (hits.Where(r => r.collider.GetComponent<Tile>() != null).ToList().Count <= 0 ) {
            Vector3 rayPoint = ray.GetPoint(TheDistance);
            transform.position = rayPoint;
            CurrentTile = null;
        }
        else if (hits.Length > 1 || !(hits.Length == 1 && hits[0].transform == gameObject.transform)) {
            if (hits.Length <= 0)
                return;

            float min = hits[0].distance;
            int minIndex = 0;

            for (int i = 1; i < hits.Length; ++i) {
                if (hits[i].transform != gameObject.transform && hits[i].distance < min) {
                    min = hits[i].distance;
                    minIndex = i;
                }
            }

            if (hits[minIndex].collider.GetComponent<Tile>() != null) {
                transform.position = new Vector3(hits[minIndex].transform.position.x, hits[minIndex].transform.position.y + 0.8f, hits[minIndex].transform.position.z);
                CurrentTile = hits[minIndex].collider.GetComponent<Tile>();
            }
        }
    }
}