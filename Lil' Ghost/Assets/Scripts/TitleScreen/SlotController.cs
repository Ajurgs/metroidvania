using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SlotController : MonoBehaviour
{
    public Button slotButton;
    public Text newGameText;
    public int slotNumber;
    public Text locationText;
    public Text timeText;

    public  void RefreshInfo()
    {
        switch (slotNumber) 
        {
            case 1:
                {
                    if (GameControl.saveSlot1 != null)
                    {
                        newGameText.enabled = false;
                        locationText.text = GameControl.saveSlot1.location;
                        timeText.text = PlaytimeToString(GameControl.saveSlot1.playTime);
                    }
                    else
                    {
                        newGameText.enabled = true;
                        locationText.text = "";
                        timeText.text = "";
                    }
                    break;
                }
            case 2:
                {
                    if (GameControl.saveSlot2 != null)
                    {
                        newGameText.enabled = false;
                        locationText.text = GameControl.saveSlot2.location;
                        timeText.text = PlaytimeToString(GameControl.saveSlot2.playTime);
                    }
                    else
                    {
                        newGameText.enabled = true;
                        locationText.text = "";
                        timeText.text = "";
                    }
                    break;
                }
            case 3:
                {
                    if (GameControl.saveSlot3 != null)
                    {
                        newGameText.enabled = false;
                        locationText.text = GameControl.saveSlot3.location;
                        timeText.text = PlaytimeToString(GameControl.saveSlot3.playTime);
                    }
                    else
                    {
                        newGameText.enabled = true;
                        locationText.text = "";
                        timeText.text = "";
                    }
                    break;
                }
            case 4:
                {
                    if (GameControl.saveSlot4 != null)
                    {
                        newGameText.enabled = false;
                        locationText.text = GameControl.saveSlot4.location;
                        timeText.text = PlaytimeToString(GameControl.saveSlot4.playTime);
                    }
                    else
                    {
                        newGameText.enabled = true;
                        locationText.text = "";
                        timeText.text = "";
                    }
                    break;
                }
        }

        
    }

    public string PlaytimeToString(int playTime)
    {

        int minutes = (playTime / 60) % 60;
        int hours = (playTime / 3600) % 24;

        string timeStamp = hours.ToString() + "H " + minutes.ToString() + "M";
        return timeStamp;
    }
}
