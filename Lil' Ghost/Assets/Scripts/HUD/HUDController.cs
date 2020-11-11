using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class HUDController : MonoBehaviour
{

    public static HUDController HUD;
    public GameObject inGameMenu;
    public GameObject inventroyMenu;
    public GameObject optionsMenu;
    public GameObject confirmQuitMenu;
    public GameObject menuBackground;
    public GameObject continueButton;
    public GameObject yesButton;
    
    public EventSystem eventSystem;
    private GameObject defaultSelected;


    [SerializeField]
    private Slider energySlider;
    [SerializeField]
    private Slider healthSlider;
    

    void Awake()
    {
        HUD = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        healthSlider.maxValue = GameControl.control.playerStats.MaxHits;
        energySlider.maxValue = GameControl.control.playerStats.MaxEnergy;

    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = GameControl.control.playerStats.HitsLeft;
        energySlider.value = GameControl.control.playerStats.EnergyLeft;


        if(eventSystem.currentSelectedGameObject != defaultSelected)
        {
            if(eventSystem.currentSelectedGameObject == null)
            {
                eventSystem.SetSelectedGameObject(defaultSelected);
            }
            else
            {
                defaultSelected = eventSystem.currentSelectedGameObject;
            }
        }

        if (Input.GetButtonDown("Menu"))
        {
            if(GameControl.control.inMenu)
            {
                if (confirmQuitMenu.activeSelf)
                {
                    NoQuitButton();
                }
                else if (inGameMenu.activeSelf)
                {
                    Continue();
                }
            }
            else if(!GameControl.control.inMenu)
            {
                OpenMenu();
            }
        }
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        inGameMenu.SetActive(false);
        confirmQuitMenu.SetActive(false);
        optionsMenu.SetActive(false);
        menuBackground.SetActive(false);
        GameControl.control.inMenu = false;
        GameControl.control.playerStats.PlayerState = GameControl.control.playerStats.PreviousPalyerState;
    }
    void OpenMenu()
    {
        Time.timeScale = 0f;
        inGameMenu.SetActive(true);
        menuBackground.SetActive(true);
        eventSystem.SetSelectedGameObject(continueButton);
        defaultSelected = continueButton;
        GameControl.control.inMenu = true;
        GameControl.control.playerStats.PreviousPalyerState = GameControl.control.playerStats.PlayerState;
    }
    void OpenInventory()
    {
        menuBackground.SetActive(true);
        inventroyMenu.SetActive(true);
        GameControl.control.inInventroy = true;
    }

    void CloseInventory()
    {
        menuBackground.SetActive(false);
        inventroyMenu.SetActive(false);
        GameControl.control.inInventroy = false;
    }
    public void QuitButton()
    {
        confirmQuitMenu.SetActive(true);
        inGameMenu.SetActive(false);
        eventSystem.SetSelectedGameObject(yesButton);
        defaultSelected = yesButton;
    }
    public void NoQuitButton()
    {
        confirmQuitMenu.SetActive(false);
        inGameMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(continueButton);
        defaultSelected = continueButton;
    }
    public void QuitGame() 
    {
        Debug.Log("Going to Title Screen");
    }

    public Slider EnergySlider { get => energySlider; set => energySlider = value; }
    public Slider HealthBar { get => healthSlider; set => healthSlider = value; }
}
