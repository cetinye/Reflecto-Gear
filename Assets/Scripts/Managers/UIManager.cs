using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public float moveAmount;
    public GameObject upLid;
    public GameObject downLid;
    public GameObject upLidFinalPos;
    public GameObject downLidFinalPos;

    [SerializeField] private float timeToMove;

    private float distanceUpLid;
    private float distanceDownLid;
    private bool flag = true;
    private float newUpPos;
    private float newDownPos;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        newUpPos = upLid.transform.localPosition.y;
        newDownPos = downLid.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        upLid.transform.localPosition = Vector3.Lerp(upLid.transform.localPosition, new Vector3(upLid.transform.localPosition.x, newUpPos, upLid.transform.localPosition.z), timeToMove);
        downLid.transform.localPosition = Vector3.Lerp(downLid.transform.localPosition, new Vector3(downLid.transform.localPosition.x, newDownPos, downLid.transform.localPosition.z), timeToMove);
    }

    public void UpdateProgressBar()
    {
        Debug.Log("Update");

        if (flag)
        {
            flag = false;
            distanceUpLid = (upLidFinalPos.transform.localPosition.y - upLid.transform.localPosition.y) / LevelManager.instance.level.unchangeableGearCount;
            distanceDownLid = (downLid.transform.localPosition.y - downLidFinalPos.transform.localPosition.y) / LevelManager.instance.level.unchangeableGearCount;
        }

        newUpPos += distanceUpLid; 
        newDownPos -= distanceDownLid; 

        //upLid.transform.localPosition = new Vector3(upLid.transform.localPosition.x, upLid.transform.localPosition.y + distanceUpLid, upLid.transform.localPosition.z);
        //downLid.transform.localPosition = new Vector3(downLid.transform.localPosition.x, downLid.transform.localPosition.y - distanceDownLid, downLid.transform.localPosition.z);        
    }
}
