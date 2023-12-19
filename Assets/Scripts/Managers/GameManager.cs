using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameState state = GameState.Idle;
    public List<Gear> AnswerList = new List<Gear>();
    public LevelDifficultyState levelDifState = LevelDifficultyState.Harder;
    public bool sameLevelFlag = false;
    public bool levelFailedBefore = false;
    public int errorCounter = 0;
    public int counterForHarderLvl = 0;

    private Gear tappedGear;
    

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        state = GameState.Intro;
    }

    public void Check(Gear gearToCheck)
    {
        //if tapped gear exists in answerlist then correct move
        if (AnswerList.Contains(gearToCheck))
        {
            UIManager.instance.UpdateProgressBar();
            gearToCheck.changable = false;
            gearToCheck.TurnGreen();
            AudioManager.instance.PlayOneShot("Correct");
            AnswerList.Remove(gearToCheck);
            CheckLevelComplete();
            UIManager.instance.LightGreen();
            Debug.LogWarning("CORRECT !");
        }
        //if not wrong move
        else
        {
            Debug.LogWarning("WRONG !");
            errorCounter++;

            //unselect gear
            tappedGear = gearToCheck;
            AudioManager.instance.PlayOneShot("Wrong");
            StartCoroutine(UnselectGear());

            UIManager.instance.LightRed();
        }
        levelDifState = LevelDifficultyState.Same;
    }

    public void CheckLevelComplete()
    {
        //if answerlist is empty then all moves have played 
        if (AnswerList.Count == 0)
        {
            state = GameState.Success;
            StartCoroutine(LevelManager.instance.AnimateUnloadLevel());
        }
    }

    public void CalculateCorrectGears()
    {
        List<GameObject> gearList = LevelManager.instance.gears;

        for (int i = 0; i < gearList.Count; i++)
        {
            //calculate only the spawned unchangable gears
            if (gearList[i].GetComponent<Gear>().changable == false)
            {
                //if mirror is only horizontal then find gears mirroring their Y value
                if (LevelManager.instance.mirrorPosX == 0 && LevelManager.instance.mirrorPosY != 0)
                    FindGear(gearList[i].GetComponent<Gear>().X, MirrorOnY(gearList[i].GetComponent<Gear>()));

                //if mirror is only vertical then find gears mirroring their X value
                else if (LevelManager.instance.mirrorPosX != 0 && LevelManager.instance.mirrorPosY == 0)
                    FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), gearList[i].GetComponent<Gear>().Y);

                //if mirror is L shaped then find gears mirroring both positions but exclude unreachable gears
                else
                {
                    switch (LevelManager.instance.level.LshapePosition)
                    {
                        case LshapePosition.TopRight:
                            if (gearList[i].GetComponent<Gear>().X > LevelManager.instance.mirrorPosX ||
                                gearList[i].GetComponent<Gear>().Y < LevelManager.instance.mirrorPosY)
                            {
                                FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), gearList[i].GetComponent<Gear>().Y);
                                FindGear(gearList[i].GetComponent<Gear>().X, MirrorOnY(gearList[i].GetComponent<Gear>()));
                                FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), MirrorOnY(gearList[i].GetComponent<Gear>()));

                            }
                            break;
                        case LshapePosition.TopLeft:
                            if (gearList[i].GetComponent<Gear>().X < LevelManager.instance.mirrorPosX ||
                                gearList[i].GetComponent<Gear>().Y < LevelManager.instance.mirrorPosY)
                            {
                                FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), gearList[i].GetComponent<Gear>().Y);
                                FindGear(gearList[i].GetComponent<Gear>().X, MirrorOnY(gearList[i].GetComponent<Gear>()));
                                FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), MirrorOnY(gearList[i].GetComponent<Gear>()));

                            }
                            break;
                        case LshapePosition.BottomRight:
                            if (gearList[i].GetComponent<Gear>().X > LevelManager.instance.mirrorPosX ||
                                gearList[i].GetComponent<Gear>().Y > LevelManager.instance.mirrorPosY)
                            {
                                FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), gearList[i].GetComponent<Gear>().Y);
                                FindGear(gearList[i].GetComponent<Gear>().X, MirrorOnY(gearList[i].GetComponent<Gear>()));
                                FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), MirrorOnY(gearList[i].GetComponent<Gear>()));

                            }
                            break;
                        case LshapePosition.BottomLeft:
                            if (gearList[i].GetComponent<Gear>().X < LevelManager.instance.mirrorPosX ||
                                gearList[i].GetComponent<Gear>().Y > LevelManager.instance.mirrorPosY)
                            {
                                FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), gearList[i].GetComponent<Gear>().Y);
                                FindGear(gearList[i].GetComponent<Gear>().X, MirrorOnY(gearList[i].GetComponent<Gear>()));
                                FindGear(MirrorOnX(gearList[i].GetComponent<Gear>()), MirrorOnY(gearList[i].GetComponent<Gear>()));

                            }
                            break;
                    }
                }
            }
        }
    }

    public void FindGear(int x, int y)
    {
        List<GameObject> gearsList = LevelManager.instance.gears;

        for (int i = 0; i < gearsList.Count; i++)
        {
            //find the gear that is not highlighted at the start and add if it doesnt exist in the answer key
            if (gearsList[i].GetComponent<Gear>().X == x && gearsList[i].GetComponent<Gear>().Y == y
                && gearsList[i].GetComponent<Gear>().highlighted == false)
                if (!AnswerList.Contains(gearsList[i].GetComponent<Gear>()))
                    AnswerList.Add(gearsList[i].GetComponent<Gear>());

            //L-shape remove out of reach gears from answer key
            if (LevelManager.instance.level.Lshape)
            {
                switch (LevelManager.instance.level.LshapePosition)
                {
                    case LshapePosition.TopRight:
                        if (gearsList[i].GetComponent<Gear>().X < LevelManager.instance.mirrorPosX &&
                            gearsList[i].GetComponent<Gear>().Y > LevelManager.instance.mirrorPosY)
                        {
                            AnswerList.Remove(gearsList[i].GetComponent<Gear>());
                        }
                        break;

                    case LshapePosition.TopLeft:
                        if (gearsList[i].GetComponent<Gear>().X > LevelManager.instance.mirrorPosX &&
                            gearsList[i].GetComponent<Gear>().Y > LevelManager.instance.mirrorPosY)
                        {
                            AnswerList.Remove(gearsList[i].GetComponent<Gear>());
                        }
                        break;

                    case LshapePosition.BottomRight:
                        if (gearsList[i].GetComponent<Gear>().X < LevelManager.instance.mirrorPosX &&
                            gearsList[i].GetComponent<Gear>().Y < LevelManager.instance.mirrorPosY)
                        {
                            AnswerList.Remove(gearsList[i].GetComponent<Gear>());
                        }
                        break;

                    case LshapePosition.BottomLeft:
                        if (gearsList[i].GetComponent<Gear>().X > LevelManager.instance.mirrorPosX &&
                            gearsList[i].GetComponent<Gear>().Y < LevelManager.instance.mirrorPosY)
                        {
                            AnswerList.Remove(gearsList[i].GetComponent<Gear>());
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

    //take the symmetry of the given gear | vertical mirror | x value changes
    public int MirrorOnX(Gear gear)
    {
        int mirrorX = GetXofMirrorOnY(gear.Y);
        int newX = (mirrorX - gear.X) + mirrorX;
        return newX;
    }

    //take the symmetry of the given gear | horizontal mirror | y value changes
    public int MirrorOnY(Gear gear)
    {
        int mirrorY = GetYofMirrorOnX(gear.X);
        int newY = (mirrorY - gear.Y) + mirrorY;
        return newY;
    }

    //returns the Y position of the mirror located in given X pos
    private int GetYofMirrorOnX(int x)
    {
        List<GameObject> listMirror = LevelManager.instance.mirrors;

        for (int i = 0; i < listMirror.Count; i++)
        {
            if (listMirror[i].GetComponent<Mirror>().X == x)
                return listMirror[i].GetComponent<Mirror>().Y;
        }

        return -1;
    }

    //returns the X position of the mirror located in given Y pos
    private int GetXofMirrorOnY(int y)
    {
        List<GameObject> listMirror = LevelManager.instance.mirrors;

        for (int i = 0; i < listMirror.Count; i++)
        {
            if (listMirror[i].GetComponent<Mirror>().Y == y)
                return listMirror[i].GetComponent<Mirror>().X;
        }

        return -1;
    }

    IEnumerator UnselectGear()
    {
        //unselect only happens if player made error so turn sprite red and white
        tappedGear.GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(UIManager.instance.timeToColor);
        tappedGear.GetComponent<Image>().color = Color.white;
        tappedGear.Tapped();
    }

    public enum GameState
    {
        Intro,
        Idle,
        Success,
        Failed,
        Playing
    }

    public enum LshapePosition
    {
        TopRight,
        TopLeft,
        BottomRight,
        BottomLeft
    }

    public enum LevelDifficultyState
    {
        Easier,
        Same,
        Harder
    }
}
