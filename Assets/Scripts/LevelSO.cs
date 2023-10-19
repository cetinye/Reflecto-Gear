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
    public int mirrorXPos;
    public int mirrorYPos;
    public bool randomizeMirrorOnX;
    public bool randomizeMirrorOnY;

    [Header("Sprites")]
    public Sprite unselected;
    public Sprite selected;

    [Header("Grid Variables")]
    public bool autoFill;
    public float cellsize;
    public GridLayoutGroup.Constraint constraint;
    public int constraintCount;
}
