using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public float timeToColor;
    public bool updateProgressbarFlag = true;
    public TextMeshProUGUI nextText;
    public int counterIndicator = 0;

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
    [SerializeField] private Image bottomGearUp;
    [SerializeField] private Image bottomGearDown;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoOnCanvas;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private ParticleSystem smokeParticle;
    [SerializeField] private float smokeEveryXTime;

    private float distanceUpLid;
    private float distanceDownLid;
    private float newUpPos;
    private float newDownPos;
    private bool UplidRoutineRunning = false;
    private bool DownlidRoutineRunning = false;
    private bool isRedFinished = true;
    private bool isGreenFinished = true;
    private int introWatchedBefore;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        StartIntro();

        StartCoroutine(StartCountdown());

        InvokeRepeating("PlaySmoke", 1f, smokeEveryXTime);

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

    public void UpdateBottomGearImage()
    {
        bottomGearDown.sprite = LevelManager.instance.level.gearOnBottomDown;
        bottomGearUp.sprite = LevelManager.instance.level.gearOnBottomUp;
    }

    public void UpdateProgressBar()
    {
        //increment counter everytime correct move is played and progressbar update
        counterIndicator++;

        //calculate the distance between starting pos and end pos only once for each level
        if (updateProgressbarFlag)
        {
            updateProgressbarFlag = false;
            distanceUpLid = (upLidFinalPos.transform.localPosition.y - upLidOpenPos.localPosition.y) / GameManager.instance.AnswerList.Count;
            distanceDownLid = (downLidOpenPos.localPosition.y - downLidFinalPos.transform.localPosition.y) / GameManager.instance.AnswerList.Count;
        }

        //add the distance needs to be travelled 
        newUpPos = upLidOpenPos.localPosition.y + (distanceUpLid * counterIndicator); 
        newDownPos = downLidOpenPos.localPosition.y - (distanceDownLid * counterIndicator);

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

        //make timer in 0:00 format
        time.text = string.Format("{0:00}", timeRemaining);
    }

    private void StartIntro()
    {
        videoPlayer.Play();
        countdownTime += (int)videoPlayer.clip.length;
        Invoke("EndReached", (float)videoPlayer.clip.length);
        introWatchedBefore = PlayerPrefs.GetInt("introWatchedBefore", 0);
        if (introWatchedBefore == 1)
            skipButton.SetActive(true);
    }

    private void EndReached()
    { 
        //close the gameobject and stop the video
        videoOnCanvas.SetActive(false);
        PlayerPrefs.SetInt("introWatchedBefore", 1);
        skipButton.SetActive(false);
        videoPlayer.Stop();
        GameManager.instance.state = GameManager.GameState.Idle;
        AudioManager.instance.Play("BackgroundMusic");
    }

    public void SkipIntro()
    {
        CancelInvoke();
        videoPlayer.Stop();
        skipButton.SetActive(false);
        videoOnCanvas.SetActive(false);
        GameManager.instance.state = GameManager.GameState.Idle;
        //update countdown
        countdownTime = 3;
        countdownText.text = countdownTime.ToString();
        AudioManager.instance.Play("BackgroundMusic");
    }

    IEnumerator UplidLerp()
    {
        UplidRoutineRunning = true;
        float timeElapsed = 0;
        Vector3 startVal = upLid.transform.localPosition;
        while (timeElapsed < timeToMove)
        {
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
        if (isRedFinished)
            StartCoroutine(TurnRed());
        else
        {
            StopCoroutine(TurnRed());
            StopCoroutine(TurnWhite());
            StartCoroutine(TurnRed());
        }
    }

    public void LightGreen()
    {
        Gear.isTappable = true;
        if (isGreenFinished)
            StartCoroutine(TurnGreen());
        else
        {
            StopCoroutine(TurnGreen());
            StopCoroutine(TurnWhite());
            StartCoroutine(TurnGreen());
        }
    }

    IEnumerator TurnRed()
    {
        isRedFinished = false;
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
        isGreenFinished = false;
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
        isRedFinished = true;
        isGreenFinished = true;
    }

    public void PlaySmoke()
    {
        smokeParticle.Play();
        AudioManager.instance.Play("Steam");
    }
}
