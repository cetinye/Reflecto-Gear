using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using UnityEngine;
using static UnityEngine.Rendering.CoreUtils;

public class LevelManager : MonoBehaviour
{
    public int levelId;

    [SerializeField] private float amountToMovePosX;
    [SerializeField] private float amountToMovePosY;
    [SerializeField] private GameObject changeableGear;
    [SerializeField] private GameObject unchangeableGear;
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
        //levelId = PlayerPrefs.GetInt("level");
        
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
                        startingPos.x += amountToMovePosX;
                        break;

                    //if 1 then spawn changable gear and move pos 
                    case "1":
                        Instantiate(changeableGear, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                        startingPos.x += amountToMovePosX;
                        break;

                    //if 2 then spawn unchangable gear and move pos 
                    case "2":
                        Instantiate(unchangeableGear, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                        startingPos.x += amountToMovePosX;
                        break;

                    //if 3 then spawn mirror and move pos 
                    case "3":
                        Instantiate(mirror, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                        startingPos.x += amountToMovePosX;
                        break;
                }
            }

            //keep last x for camera pos
            lastPos.x = startingPos.x;

            //reset position and move below
            startingPos.x = levelParent.transform.position.x;
            startingPos.y -= amountToMovePosY;

            //keep last y for camera pos
            lastPos.y = startingPos.y;

            //Debug.Log("rows: " + levelBase.Length.ToString() + " columns: " + levelBase[0].Length.ToString());
            
        }

        //ChangeScale();
        PositionCamera();
    }

    private void ChangeScale()
    {
        if (levelBase.Length > 5)
        {
            float newSize = (levelBase[0].Length - 3) * 0.1666f;
            levelParent.transform.localScale = new Vector3(1 - newSize, 1 - newSize, 1 - newSize);

            float newX = (levelBase[0].Length - 3) * 0.06f;
            float newY = (levelBase[0].Length - 3) * 0.1f;
            levelParent.transform.position = new Vector3(levelParent.transform.position.x - newX, levelParent.transform.position.y + newY, levelParent.transform.position.z);
        }
    }

    void PositionCamera()
    {
        if (levelBase[0].Length > 3)
        {
            //position the camera
            Camera.main.transform.position = new Vector3(Mathf.CeilToInt((lastPos.x - 3) / 2) + 0.5f, Mathf.CeilToInt(((lastPos.y) / 4)), -1);
            Camera.main.orthographicSize += (levelBase.Length - 3) * 0.1666f;
        }
    }
}
