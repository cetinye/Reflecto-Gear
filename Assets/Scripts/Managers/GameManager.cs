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

    //mode parameter is for disabling level fail when CheckAtStart call this function
    //default mode is 'a' but when we check at the start we pass 'b' so no level fail
    //at the start
    //This check is necessary for detecting unchangable gears that are already spawned
    //symmetrical to the mirror
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

            //for out of reach gears
            if (listchangableGear[i].GetComponent<Gear>().Y >= LevelManager.instance.level.rowCount - 1)
            {
                listchangableGear[i].GetComponent<Gear>().endgameFlag = true;
            }
        }
    }

    public void CheckLevelComplete()
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

    public void CheckUnreachable(Gear gear)
    {
        if (gear.Y >= LevelManager.instance.level.rowCount - 1)
        {
            //level failed, tapped on unreachable gear
            Debug.LogError("FAIL ! Tapped on unreachable gear");
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
