using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static GameManager;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public int levelId;
    public LevelSO level;
    public List<LevelSO> levelList;

    public int mirrorPosY;
    public int mirrorPosX;

    public List<GameObject> gears = new List<GameObject>();
    public List<GameObject> mirrors = new List<GameObject>();

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
        //make grid arrangements if autofill enabled
        if (level.autoFill)
            ArrangeGrid();

        else
            ManualArrangeGrid();

        //scale gears colliders before spawning
        ScaleGearCollider();

        if (level.randomizeMirrorOnX)
            mirrorPosX = Random.Range(level.minRandomMirrorX, level.maxRandomMirrorX);

        if (level.randomizeMirrorOnY)
            mirrorPosY = Random.Range(level.minRandomMirrorY, level.maxRandomMirrorY);

        if (level.randomizeLshapePosition)
            level.LshapePosition = (GameManager.LshapePosition)Random.Range(0, Enum.GetValues(typeof(GameManager.LshapePosition)).Length);

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

                }
                //gear
                else
                {
                    GameObject tempGear = Instantiate(changeableGear, new Vector3(startingPos.x, startingPos.y, startingPos.z), Quaternion.identity, levelParent.transform);
                    tempGear.GetComponent<Gear>().X = x;
                    tempGear.GetComponent<Gear>().Y = y;
                    tempGear.gameObject.name = tempGear.gameObject.name + x + " -" + y;
                    tempGear.GetComponent<Image>().enabled = false;
                    gears.Add(tempGear);
                }
            }
        }

        if (level.Lshape)
            ArrangeMirrors();
        
        RandomizeGears();
    }

    //randomly select unchangeable gears
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

    //close mirrors regarding selected shape
    private void ArrangeMirrors()
    {
        for (int i = 0; i < mirrors.Count; i++)
        {
            if (level.LshapePosition == LshapePosition.TopRight)
                if (mirrors[i].GetComponent<Mirror>().X < mirrorPosX ||
                    mirrors[i].GetComponent<Mirror>().Y > mirrorPosY)
                        mirrors[i].GetComponent<Image>().enabled = false;

            if (level.LshapePosition == LshapePosition.TopLeft)
                if (mirrors[i].GetComponent<Mirror>().X > mirrorPosX ||
                    mirrors[i].GetComponent<Mirror>().Y > mirrorPosY)
                        mirrors[i].GetComponent<Image>().enabled = false;

            if (level.LshapePosition == LshapePosition.BottomRight)
                if (mirrors[i].GetComponent<Mirror>().X < mirrorPosX ||
                    mirrors[i].GetComponent<Mirror>().Y < mirrorPosY)
                        mirrors[i].GetComponent<Image>().enabled = false;

            if (level.LshapePosition == LshapePosition.BottomLeft)
                if (mirrors[i].GetComponent<Mirror>().X > mirrorPosX ||
                    mirrors[i].GetComponent<Mirror>().Y < mirrorPosY)
                        mirrors[i].GetComponent<Image>().enabled = false;
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
        gears.Clear();
        UIManager.instance.updateProgressbarFlag = true;

        //decide on which level to load
        DecideLevelIndex();

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
        GameManager.instance.errorCounter = 0;
        StartCoroutine(AnimateLoadLevel());
    }

    private void DecideLevelIndex()
    {
        if (GameManager.instance.levelDifState == LevelDifficultyState.Same)
        {
            GameManager.instance.sameLevelFlag = true;

            //if completed without error, increment progress counter
            if (GameManager.instance.errorCounter == 0)
                GameManager.instance.counterForHarderLvl++;

            //checking if player played lvlX, X times. If yes load harder level
            if (GameManager.instance.sameLevelFlag && GameManager.instance.counterForHarderLvl == levelId + 1)
                GameManager.instance.levelDifState = LevelDifficultyState.Harder;

            //if made error before and made again this level, load easier
            if (GameManager.instance.levelFailedBefore && GameManager.instance.errorCounter != 0)
                GameManager.instance.levelDifState = LevelDifficultyState.Easier;

            //if made error decrease progress counter
            if (GameManager.instance.errorCounter != 0)
            {
                GameManager.instance.levelFailedBefore = true;
                GameManager.instance.counterForHarderLvl--;
            }
        }

        if (GameManager.instance.levelDifState == LevelDifficultyState.Harder)
        {
            levelId++;
            if (levelId >= levelList.Count)
            {
                levelId = 0;
            }
            GameManager.instance.sameLevelFlag = false;
            GameManager.instance.levelFailedBefore = false;
            GameManager.instance.counterForHarderLvl = 0;
        }
        
        else if (GameManager.instance.levelDifState == LevelDifficultyState.Easier)
        {
            levelId--;
            if (levelId <= 0)
            {
                levelId = 0;
            }
            GameManager.instance.sameLevelFlag = false;
            GameManager.instance.levelFailedBefore = false;
            GameManager.instance.counterForHarderLvl = 0;
        }

        PlayerPrefs.SetInt("level", levelId);
    }

    public IEnumerator AnimateLoadLevel()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < levelParent.transform.childCount; i++)
        {
            if (!levelParent.transform.GetChild(i).TryGetComponent<Mirror>(out Mirror _mirror))
            {
                levelParent.transform.GetChild(i).GetComponent<Image>().enabled = true;
                levelParent.transform.GetChild(i).GetComponent<Gear>().PlaySound();
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
                levelParent.transform.GetChild(i).GetComponent<Gear>().PlaySound();
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
        AudioManager.instance.Play("MirrorSpawn");
        yield return null;
    }
}
