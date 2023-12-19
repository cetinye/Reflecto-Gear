using UnityEngine;

public class RotateGear : MonoBehaviour
{
    [Header("-1 for counterclockwise 1 for clockwise")]
    [SerializeField] private float rotateClockwise;
    [SerializeField] private float zRotationVal;
    [SerializeField] private GameManager.GameState rotateOnState;
    private bool isLevelGearsPlaying = false;
    private bool isBottomLeftGearsPlaying = false;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.state == rotateOnState)
            transform.Rotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, rotateClockwise * zRotationVal * Time.deltaTime));

        if (GameManager.instance.state == GameManager.GameState.Idle && !isLevelGearsPlaying)
        {
            isLevelGearsPlaying = true;
            isBottomLeftGearsPlaying = false;
            AudioManager.instance.Stop("BottomLeftGears");
            AudioManager.instance.Play("LevelGears");
        }

        if (GameManager.instance.state == GameManager.GameState.Playing && !isBottomLeftGearsPlaying)
        {
            isBottomLeftGearsPlaying = true;
            isLevelGearsPlaying = false;
            AudioManager.instance.Stop("LevelGears");
            AudioManager.instance.Play("BottomLeftGears");
        }
    }
}
