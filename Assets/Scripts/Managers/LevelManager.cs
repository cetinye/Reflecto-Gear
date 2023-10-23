using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Tracing;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.CoreUtils;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public int levelId;
    public LevelSO level;
    public List<LevelSO> levelList;

    public int mirrorPosY;
    public int mirrorPosX;

    [SerializeField] private int numOfUnchangeable;
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
    private GameObject objToCheck;
    

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        //on default start level index 0
        //levelId = PlayerPrefs.GetInt("level", 0);

        ReadLevelData();
        ChangeGearSprite();
        GenerateLevel();
    }

    private void ReadLevelData()
    {
        level = levelList[levelId];

        mirrorPosY = level.mirrorYPos;
        mirrorPosX = level.mirrorXPos;
    }

    private void ChangeGearSprite()
    {
        //change sprite regarding levels choice
        changeableGear.GetComponent<Image>().sprite = level.unselected;
    }

    private void GenerateLevel()
    {

        //make grid arrangements
        if (level.autoFill)
            ArrangeGrid();

        else
            ManualArrangeGrid();

        //scale gears colliders before spawning
        ScaleGearCollider();

        if (level.randomizeMirrorOnX)
            mirrorPosX = Random.Range(1, level.columnCount);

        if (level.randomizeMirrorOnY)
            mirrorPosY = Random.Range(1, level.rowCount - 1);

        //row count
        for (int y = 0; y < level.rowCount; y++)
        {
            //column count
            for (int x = 0; x < level.columnCount; x++)
            {
                if (x != 0 && x == mirrorPosX)
                {
                    GameObject mirrorObj = Instantiate(mirror, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                    mirrorObj.GetComponent<Mirror>().X = x;
                    mirrorObj.GetComponent<Mirror>().Y = y;
                    mirrorObj.transform.eulerAngles = new Vector3 (mirrorObj.transform.rotation.x, mirrorObj.transform.rotation.y, 90.0f);
                }

                else if (y != 0 && y == mirrorPosY)
                {
                    GameObject mirrorObj = Instantiate(mirror, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                    mirrorObj.GetComponent<Mirror>().X = x;
                    mirrorObj.GetComponent<Mirror>().Y = y;
                }
                
                else
                {
                    GameObject tempGear = Instantiate(changeableGear, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                    tempGear.GetComponent<Gear>().X = x;
                    tempGear.GetComponent<Gear>().Y = y;
                }
            }
        }

        RandomizeGears();
    }

    private void RandomizeGears()
    {
        int counter = level.unchangeableGearCount;
        for (int i = 0; i < counter; i++)
        {
            int num = Random.Range(0, levelParent.transform.childCount);

            if (levelParent.transform.GetChild(num) != null)
            {
                objToCheck = levelParent.transform.GetChild(num).gameObject;

                if (objToCheck.TryGetComponent<IGear>(out IGear iGear) &&
                    objToCheck.GetComponent<Gear>().changable == true)
                {
                    objToCheck.GetComponent<Gear>().changable = false;
                    objToCheck.GetComponent<Image>().sprite = level.selected;
                    objToCheck.GetComponent<Gear>().highlighted = true;
                }
                else
                {
                    counter++;
                }
            }
        }
    }

    private void ScaleGearCollider()
    {
        changeableGear.GetComponent<CircleCollider2D>().radius = cellSize / 2f;
        unchangeableGear.GetComponent<CircleCollider2D>().radius = cellSize / 2f;
    }

    private void ArrangeGrid()
    {
        //12 is a constant i decided for the alignment
        cellSize = (10f - level.rowCount) / 10f;

        //constraint for not going above 10 row size
        if (cellSize < 0.28f)
            cellSize = 0.28f;

        levelParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize, cellSize);
        levelParent.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedRowCount;
        levelParent.GetComponent<GridLayoutGroup>().constraintCount = level.rowCount;
    }

    private void ManualArrangeGrid()
    {
        cellSize = level.cellsize;

        levelParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize, cellSize);
        levelParent.GetComponent<GridLayoutGroup>().constraint = level.constraint;
        levelParent.GetComponent<GridLayoutGroup>().constraintCount = level.constraintCount;
    }

    public void LoadNextLevel()
    {
        levelId++;
        if (levelId >= levelList.Count)
        {
            levelId = 0;
        }
        PlayerPrefs.SetInt("level", levelId);
    }
}
