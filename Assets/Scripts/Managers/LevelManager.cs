using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Tracing;
using System.Text.RegularExpressions;
using TMPro;
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

    private Vector3 startingPos;
    private float cellSize;
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
        int yMax = Mathf.CeilToInt(level.rowCount / 2);
        int xMax = Mathf.CeilToInt(level.columnCount / 2);

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
                //vertical mirror
                if (x != 0 && x == mirrorPosX)
                {
                    GameObject mirrorObj = Instantiate(mirror, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                    mirrorObj.GetComponent<Mirror>().X = x;
                    mirrorObj.GetComponent<Mirror>().Y = y;
                    mirrorObj.transform.eulerAngles = new Vector3 (mirrorObj.transform.rotation.x, mirrorObj.transform.rotation.y, 90.0f);
                    mirrors.Add(mirrorObj);
                    mirrorObj.gameObject.name = mirrorObj.gameObject.name + " " + x + "," + y;
                    mirrorObj.SetActive(false);

                    if (level.Lshape && mirrorObj.GetComponent<Mirror>().Y >= yMax)
                    {
                        if (level.LshapePosition == GameManager.LshapePosition.TopRight || level.LshapePosition == GameManager.LshapePosition.TopLeft)
                            mirrorObj.GetComponent<Image>().enabled = false;
                    }
                    else if (level.Lshape && mirrorObj.GetComponent<Mirror>().Y <= yMax)
                    {
                        if (level.LshapePosition == GameManager.LshapePosition.BottomRight || level.LshapePosition == GameManager.LshapePosition.BottomLeft)
                            mirrorObj.GetComponent<Image>().enabled = false;
                    }

                    if (level.Lshape && x == mirrorPosX && y == mirrorPosY)
                    {
                        mirrorObj.transform.localScale = new Vector3(0.46f, 0.52f, 1f);
                        mirrorObj.GetComponent<Image>().enabled = true;
                    }
                }
                //horizontal mirror
                else if (y != 0 && y == mirrorPosY)
                {
                    GameObject mirrorObj = Instantiate(mirror, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                    mirrorObj.GetComponent<Mirror>().X = x;
                    mirrorObj.GetComponent<Mirror>().Y = y;
                    mirrors.Add(mirrorObj);
                    mirrorObj.gameObject.name = mirrorObj.gameObject.name + " "+ x + "," + y;
                    mirrorObj.SetActive(false);

                    if (level.Lshape && mirrorObj.GetComponent<Mirror>().X > xMax)
                    {
                        if (level.LshapePosition == GameManager.LshapePosition.TopLeft || level.LshapePosition == GameManager.LshapePosition.BottomLeft)
                            mirrorObj.GetComponent<Image>().enabled = false;
                    }

                    else if (level.Lshape && mirrorObj.GetComponent<Mirror>().X <= xMax)
                    {
                        if (level.LshapePosition == GameManager.LshapePosition.TopRight || level.LshapePosition == GameManager.LshapePosition.BottomRight)
                            mirrorObj.GetComponent<Image>().enabled = false;
                    }
                }
                //gear
                else
                {
                    GameObject tempGear = Instantiate(changeableGear, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                    tempGear.GetComponent<Gear>().X = x;
                    tempGear.GetComponent<Gear>().Y = y;
                    tempGear.gameObject.name = tempGear.gameObject.name + x + " -" + y;
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
        yield return new WaitForEndOfFrame();
        UIManager.instance.counterIndicator = 0;
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
        UIManager.instance.nextText.GetComponent<TextMeshProUGUI>().enabled = false;
        StartCoroutine(AnimateMirrors(true));
        StartCoroutine(UIManager.instance.OpenLid());
        GameManager.instance.CalculateCorrectGears();
        GameManager.instance.state = GameManager.GameState.Playing;
    }

    public IEnumerator AnimateUnloadLevel()
    {
        GameManager.instance.state = GameManager.GameState.Idle;
        UIManager.instance.nextText.GetComponent<TextMeshProUGUI>().enabled = true;

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
