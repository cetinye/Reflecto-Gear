using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalculateSymmetry(Gear gear)
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
            Check(gear.X, newY);
        }
    }

    private void Check(int x, int y)
    {
        var listGear = GameObject.FindGameObjectsWithTag("Gear");

        for (int i = 0; i < listGear.Length; i++)
        {
            if (listGear[i].GetComponent<Gear>().X == x && listGear[i].GetComponent<Gear>().Y == y)
            {
                if (listGear[i].GetComponent<Gear>().highlighted)
                {
                    Debug.LogWarning("CORRECT !");
                }
                else
                {
                    Debug.LogWarning("!!!  FALSE  !!!");
                    //level failed
                }
            }
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
