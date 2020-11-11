using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class TitleScreenController : MonoBehaviour
{

    public GameObject mainTitleScreen;
    public GameObject optionsMenu;
    public GameObject loadScreen;
    public GameObject confirmQuitScreen;
    public GameObject confirmDeleteScreen;

    public GameObject playButton;
    public GameObject clearSlot1Button;
    public GameObject clearSlot2Button;
    public GameObject clearSlot3Button;
    public GameObject clearSlot4Button;



    public SlotController slot1;
    public SlotController slot2;
    public SlotController slot3;
    public SlotController slot4;

    public int slotToDelete = 0;

    public EventSystem eventSystem;
    private GameObject defaultSelected;


    // Start is called before the first frame update
    void Start()
    {
        defaultSelected = playButton;
        mainTitleScreen.SetActive(true);
        optionsMenu.SetActive(false);
        loadScreen.SetActive(false);
        confirmQuitScreen.SetActive(false);
        confirmDeleteScreen.SetActive(false);
        RefreshSaveSlotButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (eventSystem.currentSelectedGameObject != defaultSelected)
        {
            if (eventSystem.currentSelectedGameObject == null)
            {
                eventSystem.SetSelectedGameObject(defaultSelected);
            }
            else
            {
                defaultSelected = eventSystem.currentSelectedGameObject;
            }
        }


    }


    public void SetDefaultSelected(GameObject toBeDefault)
    {
        defaultSelected = toBeDefault;
        eventSystem.SetSelectedGameObject(toBeDefault);
    }

    public void EraseSaveFile()
    {
        if(slotToDelete != 0)
        {
            SaveLoadSystem.DeleteSaveFile(slotToDelete);
            GameControl.control.LoadGameSlots();
            RefreshSaveSlotButtons();
        }
        
    }
    public void QuitGame()
    {
        Debug.Log("Quiting Game");
        Application.Quit();
    }
    
    public void RefreshSaveSlotButtons()
    {
        slot1.RefreshInfo();
        slot2.RefreshInfo();
        slot3.RefreshInfo();
        slot4.RefreshInfo();
        if (GameControl.saveSlot1 != null)
            clearSlot1Button.SetActive(true);
        else
            clearSlot1Button.SetActive(false);
        if (GameControl.saveSlot2 != null)
            clearSlot2Button.SetActive(true);
        else
            clearSlot2Button.SetActive(false);
        if (GameControl.saveSlot3 != null)
            clearSlot3Button.SetActive(true);
        else
            clearSlot3Button.SetActive(false);
        if (GameControl.saveSlot4 != null)
            clearSlot4Button.SetActive(true);
        else
            clearSlot4Button.SetActive(false);
    }

    public void RefreshGameOptions()
    {
        // will need to set up an options menu
    }

    public void setSlotToDelete(int i)
    {
        slotToDelete = i;
    }
}
