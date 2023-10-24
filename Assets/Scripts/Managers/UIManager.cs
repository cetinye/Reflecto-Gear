using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public float timeToColor;
    public bool updateProgressbarFlag = true;

    [SerializeField] private int countdownTime;
    [SerializeField] private float timeRemaining;
    [SerializeField] private float timeToMove;
    [SerializeField] private GameObject upLid;
    [SerializeField] private GameObject downLid;
    [SerializeField] private GameObject upLidFinalPos;
    [SerializeField] private GameObject downLidFinalPos;
    [SerializeField] private float timeToOpenLidAtStart;
    [SerializeField] private Transform upLidOpenPos;
    [SerializeField] private Transform downLidOpenPos;
    [SerializeField] private GameObject indicatorCircle;
    [SerializeField] private TextMeshProUGUI levelNo;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI time;

    private float distanceUpLid;
    private float distanceDownLid;
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
        if (updateProgressbarFlag)
        {
            updateProgressbarFlag = false;
            distanceUpLid = (upLidFinalPos.transform.localPosition.y - upLidOpenPos.localPosition.y) / LevelManager.instance.level.unchangeableGearCount;
            distanceDownLid = (downLidOpenPos.localPosition.y - downLidFinalPos.transform.localPosition.y) / LevelManager.instance.level.unchangeableGearCount;
        }

        newUpPos = upLidOpenPos.localPosition.y + (distanceUpLid * GameManager.instance.counter); 
        newDownPos = downLidOpenPos.localPosition.y - (distanceDownLid * GameManager.instance.counter);

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
        //GameManager.instance.state = GameManager.GameState.Playing;
        StartCoroutine(LevelManager.instance.AnimateLoadLevel());
    }

    public IEnumerator OpenLid()
    {
        newUpPos = upLid.transform.localPosition.y;
        newDownPos = downLid.transform.localPosition.y;

        float timeElapsed = 0;
        Vector3 startValUp = upLid.transform.localPosition;
        Vector3 startValDown = downLid.transform.localPosition;
        while (timeElapsed < timeToMove)
        {
            upLid.transform.localPosition = Vector3.Lerp(startValUp, upLidOpenPos.localPosition, timeElapsed / timeToOpenLidAtStart);
            downLid.transform.localPosition = Vector3.Lerp(startValDown, downLidOpenPos.localPosition, timeElapsed / timeToOpenLidAtStart);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        upLid.transform.localPosition = upLidOpenPos.localPosition;
        downLid.transform.localPosition = downLidOpenPos.localPosition;
    }

    public void LightRed()
    {
        StartCoroutine(TurnRed());
    }

    public void LightGreen()
    {
        StartCoroutine(TurnGreen());
    }

    IEnumerator TurnRed()
    {
        float timeElapsed = 0;
        Color orgColor = indicatorCircle.GetComponent<Image>().color;
        while (timeElapsed < timeToMove)
        {
            indicatorCircle.GetComponent<Image>().color = Color.Lerp(orgColor, Color.red, timeElapsed / timeToColor);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        indicatorCircle.GetComponent<Image>().color = Color.red;
        StartCoroutine(TurnWhite());
    }

    IEnumerator TurnGreen()
    {
        float timeElapsed = 0;
        Color orgColor = indicatorCircle.GetComponent<Image>().color;
        while (timeElapsed < timeToMove)
        {
            indicatorCircle.GetComponent<Image>().color = Color.Lerp(orgColor, Color.green, timeElapsed / timeToColor);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        indicatorCircle.GetComponent<Image>().color = Color.green;
        StartCoroutine(TurnWhite());
    }

    IEnumerator TurnWhite()
    {
        float timeElapsed = 0;
        Color orgColor = indicatorCircle.GetComponent<Image>().color;
        while (timeElapsed < timeToMove)
        {
            indicatorCircle.GetComponent<Image>().color = Color.Lerp(orgColor, Color.white, timeElapsed / timeToColor);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        indicatorCircle.GetComponent<Image>().color = Color.white;
    }
}
