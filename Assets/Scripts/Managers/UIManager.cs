using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private int countdownTime;
    [SerializeField] private float timeRemaining;
    [SerializeField] private float timeToMove;
    [SerializeField] private GameObject upLid;
    [SerializeField] private GameObject downLid;
    [SerializeField] private GameObject upLidFinalPos;
    [SerializeField] private GameObject downLidFinalPos;
    [SerializeField] private TextMeshProUGUI levelNo;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI time;

    private float distanceUpLid;
    private float distanceDownLid;
    private bool flag = true;
    private float newUpPos;
    private float newDownPos;
    private bool UplidRoutineRunning = false;
    private bool DownlidRoutineRunning = false;
    private float minutes;
    private float seconds;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        StartCoroutine(StartCountdown());

        newUpPos = upLid.transform.localPosition.y;
        newDownPos = downLid.transform.localPosition.y;
    }

    private void Update()
    {
        UpdateTime();
    }

    public void UpdateLevelNo()
    {
        levelNo.text = (LevelManager.instance.levelId + 1).ToString();
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

    private void UpdateTime()
    {
        //timer continue if game is playing
        if (timeRemaining > 0 && GameManager.instance.state == GameManager.GameState.Playing)
        {
            timeRemaining -= Time.deltaTime;
        }
        //stop timer if time ran out
        else if (timeRemaining <= 0 && GameManager.instance.state == GameManager.GameState.Playing)
        {
            GameManager.instance.state = GameManager.GameState.Failed;
            timeRemaining = 0;
        }

        //1:59 format
        //minutes = Mathf.FloorToInt(timeRemaining / 60);
        //seconds = Mathf.FloorToInt(timeRemaining % 60);

        //make timer in 0:00 format
        time.text = string.Format("{0:00}", timeRemaining);
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

    IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            countdownText.text = countdownTime.ToString();
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        countdownText.text = "GO !";
        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(false);
        GameManager.instance.state = GameManager.GameState.Playing;
    }
}
