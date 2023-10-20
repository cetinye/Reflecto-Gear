using UnityEngine;
using Image = UnityEngine.UI.Image;

public class Gear : MonoBehaviour, IGear
{
    public bool changable = true;
    public int X;
    public int Y;
    public bool highlighted = false;

    private int tapCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Tapped()
    {
        Debug.Log("X: " + X  + ", Y: " + Y);

        if (changable) 
        {
            //tap counter for deciding whether highlight the gear or not
            tapCounter++;

            if (tapCounter % 2 == 0)
                this.GetComponent<Image>().sprite = LevelManager.instance.level.unselected;
            else
                this.GetComponent<Image>().sprite = LevelManager.instance.level.selected;

            GameManager.instance.CalculateSymmetry(this);
        }
    }
}
