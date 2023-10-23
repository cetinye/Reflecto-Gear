using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TapListener : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckTap();
    }

    private void CheckTap()
    {
        //Checking if user is tapping anywhere on the scene
        if (GameManager.instance.state == GameManager.GameState.Playing && Input.GetMouseButtonDown(0))
        {
            //if yes, get the position
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var touchPos = new Vector2(worldPoint.x, worldPoint.y);

            //checking if user tapped on a gear
            if (Physics2D.OverlapPoint(touchPos) != null &&
                Physics2D.OverlapPoint(touchPos).TryGetComponent<IGear>(out IGear iGear))
            {
                Debug.Log("Tapped on a gear");
                iGear.Tapped();
            }
        }
    }
}
