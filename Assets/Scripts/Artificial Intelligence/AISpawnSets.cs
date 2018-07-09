using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AISpawnSets : MonoBehaviour
{
    private GameObject Titan = (GameObject)Resources.Load("Titan");
    private GameObject Tanky = (GameObject)Resources.Load("Tanky");
    private GameObject Tiny = (GameObject)Resources.Load("Tiny");
    private List<GameObject> UnitList = new List<GameObject>();

    public void Generic1()
    {
        for (int i = 5; i < 10; i++)
        {
            if(i == 5 || i == 9) {
                GameObject unit = Instantiate(Tanky, new Vector3(i, 0, 11), Quaternion.identity) as GameObject;
                UnitList.Add(unit);
            }
            else {
                GameObject unit = Instantiate(Titan, new Vector3(i, 0, 11), Quaternion.identity) as GameObject;
                UnitList.Add(unit);
            }
        }

        for (int j = 6; j < 9; j++) {
            GameObject Unit = Instantiate(Tiny, new Vector3(j, 0, 12), Quaternion.identity) as GameObject;
            UnitList.Add(Unit);
        }

        PrepareUnits();
    }

    public void Generic2()
    {
        GameObject Unit1 = Instantiate(Tanky, new Vector3(3, 0, 10), Quaternion.identity) as GameObject;
        GameObject Unit2 = Instantiate(Tanky, new Vector3(11, 0, 10), Quaternion.identity) as GameObject;
        GameObject Unit3 = Instantiate(Tanky, new Vector3(7, 0, 10), Quaternion.identity) as GameObject;
        GameObject Unit4 = Instantiate(Titan, new Vector3(8, 0, 11), Quaternion.identity) as GameObject;
        GameObject Unit5 = Instantiate(Titan, new Vector3(6, 0, 11), Quaternion.identity) as GameObject;
        GameObject Unit6 = Instantiate(Titan, new Vector3(12, 0, 11), Quaternion.identity) as GameObject;
        GameObject Unit7 = Instantiate(Titan, new Vector3(2, 0, 11), Quaternion.identity) as GameObject;
        GameObject Unit8 = Instantiate(Tiny, new Vector3(7, 0, 11), Quaternion.identity) as GameObject;
        
        UnitList.AddMany(Unit1, Unit2, Unit3, Unit4, Unit5, Unit6, Unit7, Unit8);
        PrepareUnits();
    }

    public void TriangleFormation()
    {
        GameObject Unit1 = Instantiate(Tanky, new Vector3(7, 0, 10), Quaternion.identity) as GameObject;
        GameObject Unit2 = Instantiate(Titan, new Vector3(9, 0, 12), Quaternion.identity) as GameObject;
        GameObject Unit3 = Instantiate(Titan, new Vector3(5, 0, 12), Quaternion.identity) as GameObject;
        GameObject Unit4 = Instantiate(Titan, new Vector3(7, 0, 12), Quaternion.identity) as GameObject;
        GameObject Unit5 = Instantiate(Titan, new Vector3(8, 0, 12), Quaternion.identity) as GameObject;
        GameObject Unit6 = Instantiate(Titan, new Vector3(6, 0, 12), Quaternion.identity) as GameObject;
        GameObject Unit7 = Instantiate(Titan, new Vector3(7, 0, 11), Quaternion.identity) as GameObject;
        GameObject Unit8 = Instantiate(Tiny, new Vector3(6, 0, 11), Quaternion.identity) as GameObject;
        GameObject Unit9 = Instantiate(Tiny, new Vector3(8, 0, 11), Quaternion.identity) as GameObject;

        UnitList.AddMany(Unit1, Unit2, Unit3, Unit4, Unit5, Unit6, Unit7, Unit8, Unit9);
        PrepareUnits();
    }

    public void TinyTeam()
    {
        GameObject Unit1 = Instantiate(Titan, new Vector3(4, 0, 10), Quaternion.identity) as GameObject;
        GameObject Unit2 = Instantiate(Titan, new Vector3(10, 0, 10), Quaternion.identity) as GameObject;
        GameObject Unit3 = Instantiate(Tiny, new Vector3(9, 0, 10), Quaternion.identity) as GameObject;
        GameObject Unit4 = Instantiate(Tiny, new Vector3(9, 0, 11), Quaternion.identity) as GameObject;
        GameObject Unit5 = Instantiate(Tiny, new Vector3(10, 0, 11), Quaternion.identity) as GameObject;
        GameObject Unit6 = Instantiate(Tiny, new Vector3(5, 0, 10), Quaternion.identity) as GameObject;
        GameObject Unit7 = Instantiate(Tiny, new Vector3(4, 0, 11), Quaternion.identity) as GameObject;
        GameObject Unit8 = Instantiate(Tiny, new Vector3(5, 0, 11), Quaternion.identity) as GameObject;

        UnitList.AddMany(Unit1, Unit2, Unit3, Unit4, Unit5, Unit6, Unit7, Unit8);
        PrepareUnits();
    }

    public void FourThreeSplit()
    {
        GameObject Unit1 = Instantiate(Tanky, new Vector3(4, 0, 12), Quaternion.identity) as GameObject;
        GameObject Unit2 = Instantiate(Tanky, new Vector3(5, 0, 12), Quaternion.identity) as GameObject;
        GameObject Unit3 = Instantiate(Tanky, new Vector3(9, 0, 12), Quaternion.identity) as GameObject;
        GameObject Unit4 = Instantiate(Tanky, new Vector3(10, 0, 12), Quaternion.identity) as GameObject;
        GameObject Unit5 = Instantiate(Tiny, new Vector3(5, 0, 13), Quaternion.identity) as GameObject;
        GameObject Unit6 = Instantiate(Tiny, new Vector3(6, 0, 12), Quaternion.identity) as GameObject;
        GameObject Unit7 = Instantiate(Tiny, new Vector3(9, 0, 13), Quaternion.identity) as GameObject;

        UnitList.AddMany(Unit1, Unit2, Unit3, Unit4, Unit5, Unit6, Unit7);
        PrepareUnits();
    }

    private void PrepareUnits()
    {
        foreach (GameObject u in UnitList) {
            u.transform.rotation = Quaternion.Euler(30, -40, 0);
            u.transform.position = new Vector3(u.transform.position.x, 0.8f, u.transform.position.z);
            u.GetComponent<SpriteRenderer>().flipX = false;
            Destroy(u.GetComponent<Draggable>());
            u.GetComponent<Unit>().enabled = true;
            u.GetComponent<Unit>().Owner = 1;
            u.GetComponent<AIAgent>().enabled = true;
            u.GetComponentInChildren<AIProperties>().enabled = true;

            u.GetComponent<Unit>().CurrentTile = u.GetComponent<Unit>().GetTileBeneath();
            u.GetComponent<Unit>().GetTileBeneath().IsOccupied = true;
            u.transform.parent = GameManager.instance.UnitManObj.AiUnits.transform;
        }
    }

}
