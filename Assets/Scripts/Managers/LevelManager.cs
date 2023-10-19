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

        ReadLevelData();
        ChangeGearSprite();
        GenerateLevel();
    }

    private void ReadLevelData()
    {
        level = levelList[levelId];
    }

    private void ChangeGearSprite()
    {
        //change sprite regarding levels choice
        changeableGear.GetComponent<Image>().sprite = level.unselected;
    }

    private void GenerateLevel()
    {

        //make grid arrangements
        ArrangeGrid();

        //scale gears colliders before spawning
        ScaleGearCollider();

        //row count
        for (int y = 0; y < level.rowCount; y++)
        {
            //column count
            for (int x = 0; x < level.columnCount; x++)
            {
                if (x != 0 && x == level.mirrorXPos)
                {
                    var mirrorObj = Instantiate(mirror, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                    mirrorObj.transform.eulerAngles = new Vector3 (mirrorObj.transform.rotation.x, mirrorObj.transform.rotation.y, 90.0f);
                }

                if (y != 0 && y == level.mirrorYPos)
                    Instantiate(mirror, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                
                else
                    Instantiate(changeableGear, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
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
        cellSize = (12f - level.rowCount) / 10f;

        //constraint for not going above 10 row size
        if (cellSize < 0.33f)
            cellSize = 0.33f;

        levelParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize, cellSize);
        levelParent.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedRowCount;
        levelParent.GetComponent<GridLayoutGroup>().constraintCount = level.rowCount;
    }
}
