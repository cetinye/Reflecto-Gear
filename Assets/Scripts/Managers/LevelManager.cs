using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.CoreUtils;

public class LevelManager : MonoBehaviour
{
    public int levelId;

    [SerializeField] private float amountToMovePosX;
    [SerializeField] private float amountToMovePosY;
    [SerializeField] private GameObject changeableGear;
    [SerializeField] private GameObject unchangeableGear;
    [SerializeField] private GameObject emptyCell;
    [SerializeField] private GameObject mirror;
    [SerializeField] private GameObject levelParent;

    private string[][] levelBase;
    private Vector3 startingPos;
    private Vector2 lastPos;
    private float cellSize;

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

        //make grid arrangements
        ArrangeGrid();

        //scale gears colliders before spawning
        ScaleGearCollider();

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
                        Instantiate(emptyCell, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                        break;

                    //if 1 then spawn changable gear and move pos 
                    case "1":
                        Instantiate(changeableGear, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                        break;

                    //if 2 then spawn unchangable gear and move pos 
                    case "2":
                        Instantiate(unchangeableGear, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                        break;

                    //if 3 then spawn mirror and move pos 
                    case "3":
                        Instantiate(mirror, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                        break;
                }
            }
        }
        //Debug.Log("rows: " + levelBase.Length.ToString() + " columns: " + levelBase[0].Length.ToString());
    }

    private void ScaleGearCollider()
    {
        changeableGear.GetComponent<CircleCollider2D>().radius = cellSize / 2f;
        unchangeableGear.GetComponent<CircleCollider2D>().radius = cellSize / 2f;
    }

    private void ScaleMirror()
    {
        if (levelBase.Length > 5)
        {
            float mirrorScale = 0.12f - ((levelBase.Length - 5f) / 10f);
            mirror.transform.localScale = new Vector3(mirrorScale / 10f, mirror.transform.localScale.y, mirror.transform.localScale.z);
        }
    }

    private void ArrangeGrid()
    {
        //12 is a constant i decided for the alignment
        cellSize = (12f - levelBase.Length) / 10f;

        //constraint for not going above 10 row size
        if (cellSize < 0.33f)
            cellSize = 0.33f;

        levelParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize, cellSize);
        levelParent.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedRowCount;
        levelParent.GetComponent<GridLayoutGroup>().constraintCount = levelBase.Length;
    }
}
