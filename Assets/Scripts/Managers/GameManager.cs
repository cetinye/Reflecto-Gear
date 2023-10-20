using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalculateSymmetry(Gear gear, char mode = 'a')
    {
        //mirror is vertical
        if (LevelManager.instance.mirrorPosX != 0)
        {

        }

        //mirror is horizontal
        if (LevelManager.instance.mirrorPosY != 0)
        {
            int mirrorY = GetYofMirrorOnX(gear.X);
            int newY = (mirrorY - gear.Y) + mirrorY;
            Check(gear.X, newY, mode);
        }
    }

    private void Check(int x, int y, char mode)
    {
        var listGear = GameObject.FindGameObjectsWithTag("Gear");

        for (int i = 0; i < listGear.Length; i++)
        {
            if (listGear[i].GetComponent<Gear>().X == x && listGear[i].GetComponent<Gear>().Y == y)
            {
                if (listGear[i].GetComponent<Gear>().highlighted)
                {
                    listGear[i].GetComponent<Gear>().endgameFlag = true;
                    Debug.LogWarning("CORRECT !");
                }
                else
                {
                    if (listGear[i].GetComponent<Gear>().changable == true && mode == 'a')
                        Debug.LogError("!!!  FALSE  !!!");
                    //level failed
                }
            }
        }
    }

    public void CheckAtStart()
    {
        var listchangableGear = GameObject.FindGameObjectsWithTag("Gear");
        for (int i = 0; i < listchangableGear.Length; i++)
        {
            if (listchangableGear[i].GetComponent<Gear>().changable == false)
                CalculateSymmetry(listchangableGear[i].GetComponent<Gear>(), 'b');
        }
    }

    public void EndLevel()
    {
        //int counter = 0;
        var listchangableGear = GameObject.FindGameObjectsWithTag("Gear");

        for (int i = 0; i < listchangableGear.Length; i++)
        {
            if (listchangableGear[i].GetComponent<Gear>().changable == false && 
                listchangableGear[i].GetComponent<Gear>().endgameFlag == true &&
                listchangableGear[i].GetComponent<Gear>().isCalculated == false)
            {
                listchangableGear[i].GetComponent<Gear>().isCalculated = true;
                counter++;
            }
        }

        if (counter == LevelManager.instance.level.unchangeableGearCount)
        {
            Debug.LogWarning("LEVEL COMPLETED");
        }
    }

    private int GetYofMirrorOnX(int x)
    {
        var listMirror = GameObject.FindGameObjectsWithTag("Mirror");

        for (int i = 0; i < listMirror.Length; i++)
        {
            if (listMirror[i].GetComponent<Mirror>().X == x)
                return listMirror[i].GetComponent<Mirror>().Y;
        }

        return -1;
    }
}
