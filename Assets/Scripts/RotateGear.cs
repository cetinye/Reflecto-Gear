using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGear : MonoBehaviour
{
    [Header("-1 for counterclockwise 1 for clockwise")]
    [SerializeField] private float rotateClockwise;
    [SerializeField] private float zRotationVal;
    [SerializeField] private GameManager.GameState rotateOnState;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.state == rotateOnState)
            transform.Rotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, rotateClockwise * zRotationVal * Time.deltaTime));
    }
}
