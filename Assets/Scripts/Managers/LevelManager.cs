using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static UnityEngine.Rendering.CoreUtils;

public class LevelManager : MonoBehaviour
{
    public int levelId;

    [SerializeField] private float amountToMovePos;
    [SerializeField] private GameObject gear;
    [SerializeField] private GameObject mirror;
    [SerializeField] private GameObject levelParent;

    private string[][] levelBase;
    private Vector3 startingPos;
    private Vector2 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        levelId = PlayerPrefs.GetInt("level");

        //full path of the level texts document
        string text = Resources.Load<TextAsset>("LevelTexts/" + levelId).ToString();
        //split by line breaks and spaces
        string[] lines = Regex.Split(text, "\r\n");
        int rows = lines.Length;
        levelBase = new string[rows][];

        for (int i = 0; i < lines.Length; i++)
        {
            string[] stringsOfLine = Regex.Split(lines[i], " ");
            levelBase[i] = stringsOfLine;
        }

        startingPos = levelParent.transform.position;

        //row count
        for (int y = 0; y < levelBase.Length; y++)
        {
            //column count
            for (int x = 0; x < levelBase[0].Length; x++)
            {
                switch (levelBase[y][x])
                {
                    //if 0 then do nothing and move pos 
                    case "0":
                        startingPos.x += amountToMovePos;
                        break;

                    //if 1 then spawn gear and move pos 
                    case "1":
                        Instantiate(gear, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                        startingPos.x += amountToMovePos;
                        break;

                    //if 2 then spawn mirror and move pos 
                    case "2":
                        Instantiate(mirror, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                        startingPos.x += amountToMovePos;
                        break;
                }
            }

            //keep last x for camera pos
            lastPos.x = startingPos.x;

            //reset position and move below
            startingPos.x = levelParent.transform.position.x;
            startingPos.y -= amountToMovePos;

            //keep last y for camera pos
            lastPos.y = startingPos.y;

        }
    }
}
