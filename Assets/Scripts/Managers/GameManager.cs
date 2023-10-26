using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameState state = GameState.Idle;
    public int gearCountForCompletion;
    public List<Gear> AnswerList = new List<Gear>();

    private Gear tappedGear;
    

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        UIManager.instance.UpdateLevelNo();
        UIManager.instance.UpdateBottomGearImage();
        gearCountForCompletion = LevelManager.instance.level.unchangeableGearCount;

        state = GameState.Idle;
    }

    public void Check(Gear gearToCheck)
    {
        if (AnswerList.Contains(gearToCheck))
        {
            AnswerList.Remove(gearToCheck);
            gearToCheck.changable = false;
            CheckLevelComplete();
            UIManager.instance.LightGreen();
            UIManager.instance.UpdateProgressBar();
            Debug.LogWarning("CORRECT !");
        }
        else
        {
            Debug.LogError("FAIL !");

            //unselect
            tappedGear = gearToCheck;
            StartCoroutine(UnselectGear());

            UIManager.instance.LightRed();
        }
        
    }

    public void CheckLevelComplete()
    {
        if (AnswerList.Count == 0)
        {
            state = GameState.Success;
            StartCoroutine(LevelManager.instance.AnimateUnloadLevel());
        }
    }

    public void CalculateCorrectGears()
    {
        var gearList = GameObject.FindGameObjectsWithTag("Gear");

        for (int i = 0; i < gearList.Length; i++)
        {
            if (gearList[i].GetComponent<Gear>().changable == false)
            {
                if (LevelManager.instance.mirrorPosX == 0 && LevelManager.instance.mirrorPosY != 0)
                    FindGear(gearList[i].GetComponent<Gear>().X, MirrorOnY(gearList[i].GetComponent<Gear>()));

                else if (LevelManager.instance.mirrorPosX != 0 && LevelManager.instance.mirrorPosY == 0)
                    FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), gearList[i].GetComponent<Gear>().Y);

                else
                {
                    if (gearList[i].GetComponent<Gear>().X < LevelManager.instance.mirrorPosX ||
                        gearList[i].GetComponent<Gear>().Y < LevelManager.instance.mirrorPosY)
                    {
                        FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), gearList[i].GetComponent<Gear>().Y);
                        FindGear(gearList[i].GetComponent<Gear>().X, MirrorOnY(gearList[i].GetComponent<Gear>()));
                        FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), MirrorOnY(gearList[i].GetComponent<Gear>()));

                    }
                }
            }
        }
    }

    public void FindGear(int x, int y)
    {
        var gearsList = GameObject.FindGameObjectsWithTag("Gear");

        for (int i = 0; i < gearsList.Length; i++)
        {
            if (gearsList[i].GetComponent<Gear>().X == x && gearsList[i].GetComponent<Gear>().Y == y
                && gearsList[i].GetComponent<Gear>().highlighted == false)
                if (!AnswerList.Contains(gearsList[i].GetComponent<Gear>()))
                    AnswerList.Add(gearsList[i].GetComponent<Gear>());

            //L-shape out of reach
            if (LevelManager.instance.level.Lshape)
            {
                if (gearsList[i].GetComponent<Gear>().X > LevelManager.instance.mirrorPosX &&
                gearsList[i].GetComponent<Gear>().Y > LevelManager.instance.mirrorPosY)
                {
                    AnswerList.Remove(gearsList[i].GetComponent<Gear>());
                }
            }
        }
    }

    public int MirrorOnX(Gear gear)
    {
        int mirrorX = GetXofMirrorOnY(gear.Y);
        int newX = (mirrorX - gear.X) + mirrorX;
        return newX;
    }

    public int MirrorOnY(Gear gear)
    {
        int mirrorY = GetYofMirrorOnX(gear.X);
        int newY = (mirrorY - gear.Y) + mirrorY;
        return newY;
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

    IEnumerator UnselectGear()
    {
        yield return new WaitForSeconds(UIManager.instance.timeToColor);
        tappedGear.Tapped();
    }

    public enum GameState
    {
        Idle,
        Success,
        Failed,
        Playing
    }
}
