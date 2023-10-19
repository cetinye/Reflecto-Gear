using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lvl", menuName = "Create Level")]
public class LevelSO : ScriptableObject
{
    [Header("Information")]
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
}
