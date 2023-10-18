using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour, IGear
{
    private int tapCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        //apply the unselected sprite
        this.GetComponent<SpriteRenderer>().sprite = GameManager.instance.unselected;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Tapped()
    {
        //tap counter for deciding whether highlight the gear or not
        tapCounter++;

        if (tapCounter % 2 == 0)
            this.GetComponent<SpriteRenderer>().sprite = GameManager.instance.unselected;
        else
            this.GetComponent<SpriteRenderer>().sprite = GameManager.instance.selected;

    }
}
