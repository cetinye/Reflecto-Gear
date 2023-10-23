using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int counter = 0;
    public GameState state = GameState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        UIManager.instance.UpdateLevelNo();

        state = GameState.Idle;
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
            int mirrorX = GetXofMirrorOnY(gear.Y);
            int newX = (mirrorX - gear.X) + mirrorX;
            Check(newX, gear.Y, mode);
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
                    UIManager.instance.LightGreen();
                    Debug.LogWarning("CORRECT !");

                    if (mode == 'a')
                        break;
                }
                else
                {
                    if (listGear[i].GetComponent<Gear>().changable == true && mode == 'a')
                    {
                        //Debug.LogError("X: " + x + ", Y: " + y);
                        Debug.LogError("!!!  FALSE  !!!");
                        UIManager.instance.LightRed();
                    }
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

            //out of reach gears
            if (LevelManager.instance.mirrorPosY != 0 && listchangableGear[i].GetComponent<Gear>().Y >= (LevelManager.instance.mirrorPosY * 2) + 1 ||
                listchangableGear[i].GetComponent<Gear>().Y < LevelManager.instance.mirrorPosY - ((LevelManager.instance.level.rowCount - 1) - LevelManager.instance.mirrorPosY))
            {
                listchangableGear[i].GetComponent<Gear>().endgameFlag = true;
            }

            if (LevelManager.instance.mirrorPosX != 0 && listchangableGear[i].GetComponent<Gear>().X >= (LevelManager.instance.mirrorPosX * 2) + 1 ||
                listchangableGear[i].GetComponent<Gear>().X < LevelManager.instance.mirrorPosX - ((LevelManager.instance.level.columnCount - 1) - LevelManager.instance.mirrorPosX))
            {
                listchangableGear[i].GetComponent<Gear>().endgameFlag = true;
            }
        }
    }

    public void CheckLevelComplete()
    {
        var listchangableGear = GameObject.FindGameObjectsWithTag("Gear");

        for (int i = 0; i < listchangableGear.Length; i++)
        {
            if (listchangableGear[i].GetComponent<Gear>().changable == false && 
                listchangableGear[i].GetComponent<Gear>().endgameFlag == true &&
                listchangableGear[i].GetComponent<Gear>().isCalculated == false)
            {
                listchangableGear[i].GetComponent<Gear>().isCalculated = true;
                counter++;
                UIManager.instance.UpdateProgressBar();
            }
        }

        if (counter == LevelManager.instance.level.unchangeableGearCount)
        {
            //Debug.LogWarning("LEVEL COMPLETED");
            state = GameState.Success;
        }
    }

    public void CheckUnreachable(Gear gear)
    {
        //mirror horizontal
        if (LevelManager.instance.mirrorPosY != 0 && gear.Y >= (LevelManager.instance.mirrorPosY * 2) + 1 ||
            gear.Y < LevelManager.instance.mirrorPosY - ((LevelManager.instance.level.rowCount - 1) - LevelManager.instance.mirrorPosY))
        {
            //level failed, tapped on unreachable gear
            //Debug.LogError("FAIL ! Tapped on unreachable gear");
            UIManager.instance.LightRed();
        }

        //mirror vertical
        if (LevelManager.instance.mirrorPosX != 0 && gear.X >= (LevelManager.instance.mirrorPosX * 2) + 1 || 
            gear.X < LevelManager.instance.mirrorPosX - ((LevelManager.instance.level.columnCount - 1) - LevelManager.instance.mirrorPosX))
        {
            //level failed, tapped on unreachable gear
            //Debug.LogError("FAIL ! Tapped on unreachable gear");
            UIManager.instance.LightRed();
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

    private int GetXofMirrorOnY(int y)
    {
        var listMirror = GameObject.FindGameObjectsWithTag("Mirror");

        for (int i = 0; i < listMirror.Length; i++)
        {
            if (listMirror[i].GetComponent<Mirror>().Y == y)
                return listMirror[i].GetComponent<Mirror>().X;
        }

        return -1;
    }

    public enum GameState
    {
        Idle,
        Success,
        Failed,
        Playing
    }
}
