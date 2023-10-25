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

    [SerializeField] private float gearSpawnTime;
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
    private List<GameObject> mirrors = new List<GameObject>();
    

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        //on default start level index 0
        levelId = PlayerPrefs.GetInt("level", 0);

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
                    mirrors.Add(mirrorObj);
                    mirrorObj.SetActive(false);
                }

                else if (y != 0 && y == mirrorPosY)
                {
                    GameObject mirrorObj = Instantiate(mirror, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                    mirrorObj.GetComponent<Mirror>().X = x;
                    mirrorObj.GetComponent<Mirror>().Y = y;
                    mirrors.Add(mirrorObj);
                    mirrorObj.SetActive(false);
                }
                
                else
                {
                    GameObject tempGear = Instantiate(changeableGear, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                    tempGear.GetComponent<Gear>().X = x;
                    tempGear.GetComponent<Gear>().Y = y;
                    tempGear.GetComponent<Image>().enabled = false;
                }
            }
        }

        RandomizeGears();
    }

    private void RandomizeGears()
    {
        int counter = 0;
        while (counter < level.unchangeableGearCount)
        {
            int num = Random.Range(0, level.rowCount * level.columnCount);

            if (levelParent.transform.GetChild(num).TryGetComponent<IGear>(out IGear iGear) && levelParent.transform.GetChild(num).GetComponent<Gear>().changable == true)
            {
                levelParent.transform.GetChild(num).GetComponent<Gear>().changable = false;
                levelParent.transform.GetChild(num).GetComponent<Image>().sprite = level.selected;
                levelParent.transform.GetChild(num).GetComponent<Gear>().highlighted = true;

                counter++;
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

    public IEnumerator LoadNextLevel()
    {
        //Empty all previous level objects
        for (int i = 0; i < levelParent.transform.childCount; i++)
        {
            Destroy(levelParent.transform.GetChild(i).gameObject);
        }
        mirrors.Clear();
        GameManager.instance.counter = 0;
        UIManager.instance.updateProgressbarFlag = true;

        //increase level index
        levelId++;
        if (levelId >= levelList.Count)
        {
            levelId = 0;
        }
        PlayerPrefs.SetInt("level", levelId);

        //create level
        ReadLevelData();
        yield return new WaitForEndOfFrame();
        ChangeGearSprite();
        yield return new WaitForEndOfFrame();
        GenerateLevel();
        yield return new WaitForEndOfFrame();
        UIManager.instance.UpdateLevelNo();
        yield return new WaitForEndOfFrame();
        UIManager.instance.UpdateBottomGearImage();
        StartCoroutine(AnimateLoadLevel());
    }

    public IEnumerator AnimateLoadLevel()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < levelParent.transform.childCount; i++)
        {
            if (!levelParent.transform.GetChild(i).TryGetComponent<Mirror>(out Mirror _mirror))
            {
                levelParent.transform.GetChild(i).GetComponent<Image>().enabled = true;
                yield return new WaitForSeconds(gearSpawnTime);
            }
        }
        StartCoroutine(AnimateMirrors(true));
        StartCoroutine(UIManager.instance.OpenLid());
        GameManager.instance.CheckAtStart();
        GameManager.instance.state = GameManager.GameState.Playing;
    }

    public IEnumerator AnimateUnloadLevel()
    {
        GameManager.instance.state = GameManager.GameState.Idle;

        //delay mirror despawning
        yield return new WaitForSeconds(1f);

        StartCoroutine(AnimateMirrors(false));

        for (int i = levelParent.transform.childCount - 1; i >= 0; i--)
        {
            if (!levelParent.transform.GetChild(i).TryGetComponent<Mirror>(out Mirror _mirror))
            {
                levelParent.transform.GetChild(i).GetComponent<Image>().enabled = false;
                yield return new WaitForSeconds(gearSpawnTime);
            }
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(LoadNextLevel());
    }

    IEnumerator AnimateMirrors(bool boolean)
    {
        for (int i = 0; i < mirrors.Count; i++)
        {
            mirrors[i].SetActive(boolean);
        }

        yield return null;
    }
}
