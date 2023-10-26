using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Lvl", menuName = "Create Level")]
public class LevelSO : ScriptableObject
{
    [Header("Variables")]
    public int rowCount;
    public int columnCount;
    public int unchangeableGearCount;
    [Header("Write 0 if random enabled")]
    public int mirrorXPos;
    public int mirrorYPos;
    public bool randomizeMirrorOnX;
    public bool randomizeMirrorOnY;
    public bool Lshape;
    public GameManager.LshapePosition LshapePosition;

    [Header("Sprites")]
    public Sprite unselected;
    public Sprite selected;
    public Sprite gearOnBottomUp;
    public Sprite gearOnBottomDown;

    [Header("Grid Variables")]
    public bool autoFill;
    public float cellsize;
    public GridLayoutGroup.Constraint constraint;
    public int constraintCount;
}
