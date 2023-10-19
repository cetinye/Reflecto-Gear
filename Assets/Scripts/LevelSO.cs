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


    [Header("Sprites")]
    public Sprite selected;
    public Sprite unselected;
}
