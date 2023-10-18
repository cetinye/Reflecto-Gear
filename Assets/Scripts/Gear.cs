using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour, IGear
{
    public bool changable = true;

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
        if (changable) 
        {
            //tap counter for deciding whether highlight the gear or not
            tapCounter++;

            if (tapCounter % 2 == 0)
                this.GetComponent<SpriteRenderer>().sprite = GameManager.instance.unselected;
            else
                this.GetComponent<SpriteRenderer>().sprite = GameManager.instance.selected;
        }
    }
}
