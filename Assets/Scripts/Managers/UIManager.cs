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
    private bool UplidRoutineRunning = false;
    private bool DownlidRoutineRunning = false;

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

        if (UplidRoutineRunning == true)
        {
            StopCoroutine(UplidLerp());
            UplidRoutineRunning = false;
        }
        StartCoroutine(UplidLerp());

        if (DownlidRoutineRunning == true)
        {
            StopCoroutine(DownlidLerp());
            DownlidRoutineRunning = false;
        }
        StartCoroutine(DownlidLerp());       
    }

    IEnumerator UplidLerp()
    {
        UplidRoutineRunning = true;
        float timeElapsed = 0;
        Vector3 startVal = upLid.transform.localPosition;
        while (timeElapsed < timeToMove)
        {
            //upLid.transform.localPosition = new Vector3 (upLid.transform.localPosition.x, Mathf.Lerp(upLid.transform.localPosition.y, newUpPos, timeElapsed / timeToMove), upLid.transform.localPosition.z);
            upLid.transform.localPosition = Vector3.Lerp(startVal, new Vector3 (upLid.transform.localPosition.x, newUpPos, upLid.transform.localPosition.z), timeElapsed / timeToMove);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        upLid.transform.localPosition = new Vector3(upLid.transform.localPosition.x, newUpPos, upLid.transform.localPosition.z);
        UplidRoutineRunning = false;
    }

    IEnumerator DownlidLerp()
    {
        DownlidRoutineRunning = true;
        float timeElapsed = 0;
        Vector3 startVal = downLid.transform.localPosition;
        while (timeElapsed < timeToMove)
        {
            //downLid.transform.localPosition = new Vector3(downLid.transform.localPosition.x, Mathf.Lerp(downLid.transform.localPosition.y, newDownPos, timeElapsed / timeToMove), downLid.transform.localPosition.z);
            downLid.transform.localPosition = Vector3.Lerp(startVal, new Vector3(downLid.transform.localPosition.x, newDownPos, downLid.transform.localPosition.z), timeElapsed / timeToMove);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        downLid.transform.localPosition = new Vector3(downLid.transform.localPosition.x, newDownPos, downLid.transform.localPosition.z);
        DownlidRoutineRunning = false;
    }
}
