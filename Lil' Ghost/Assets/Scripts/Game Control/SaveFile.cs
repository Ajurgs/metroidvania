using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


[System.Serializable]
public class SaveFile
{
    //save slot info
    public string location;
    public int playTime;
    public int restPointRoom;
    // player advancement
    public int maxHealth;
    public float maxEnergy;
    public bool canDoubleJump;
    public bool canWallJump;
    public bool canDash;
    public bool canShoot;
    // Boss Save information
    public bool woodcutterEncountered;
    public bool woodcutterDefeated;
    // Switches 
    public bool[] switchesArray;
    // Invintory
    public bool[] invintoryList;

    public SaveFile(PlayerStats player)
    {
        maxHealth = player.MaxHits;
        maxEnergy = player.MaxEnergy;
        canDoubleJump = player.HasDoubleJump;
        canDash = player.HasDash;
        canWallJump = player.HasWallJump;
        canShoot = player.HasShootEctoplasm;
        woodcutterEncountered = GameControl.control.woodcutterEncountered;
        woodcutterDefeated = GameControl.control.woodcutterDefeated;
        restPointRoom = GameControl.control.restPointRoom;
    }
}
