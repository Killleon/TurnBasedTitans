using UnityEngine;
using System.Collections;

/// <summary>
/// A simple script to create a grid by using public ints for size and a public gameobject to clone as cells.
/// </summary>
public class CreateGrid : MonoBehaviour {

    public int Xsize;
    public int Zsize;
    public GameObject masterCellPrefab;
    public Material DefaultTile;

    void Start () {
        GameObject board = new GameObject();
        board.name = "MasterGrid";
        board.AddComponent<GameManager>();

        for (int x = 0; x < Xsize; x++)
        {
            for (int z = 0; z < Zsize; z++)
            {
                GameObject cloneCellPrefab = Instantiate(masterCellPrefab, new Vector3(x,0,z), Quaternion.identity) as GameObject;
                cloneCellPrefab.name = "Cell"+x+z;
                cloneCellPrefab.transform.parent = transform;
                cloneCellPrefab.GetComponent<Renderer>().material = DefaultTile;
                cloneCellPrefab.GetComponent<Tile>().GridSpace = new Vector2(x,z);
                Debug.Log("Grid Generation Complete");
            }
        }
    }

}
