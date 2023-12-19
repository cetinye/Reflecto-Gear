using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Gear : MonoBehaviour, IGear
{
    public static bool isTappable = true;
    public bool changable = true;
    public int X;
    public int Y;
    public bool highlighted = false;
    public bool endgameFlag = false;
    public bool isCalculated = false;

    private int tapCounter = 0;

    public void Tapped()
    {
        //Debug.Log("X: " + X  + ", Y: " + Y);

        if (changable) 
        {
            //tap counter for deciding whether highlight the gear or not
            tapCounter++;

            if (tapCounter % 2 == 0)
            {
                this.GetComponent<Image>().sprite = LevelManager.instance.level.unselected;
                this.highlighted = false;

                isTappable = true;
            }
            else
            {
                isTappable = false;

                this.GetComponent<Image>().sprite = LevelManager.instance.level.selected;
                this.highlighted = true;

                GameManager.instance.Check(this);
            }
        }
    }

    public void TurnGreen()
    {
        StartCoroutine(TurnGreenRoutine());
    }

    IEnumerator TurnGreenRoutine()
    {
        this.GetComponent<Image>().color = Color.green;
        yield return new WaitForSeconds(UIManager.instance.timeToColor);
        this.GetComponent<Image>().color = Color.white;
    }
}
