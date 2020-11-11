using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum PlayerState { Walking,Dash,Dead, Cutsceen, Rest, Knockback,MoveTo,Menu,Map};
public enum EnemyState { Active, Inactive, Knockback,Target, Dead, Cutsceen}

public enum EnemyType { Patrol, Sprint, Jumping}
public enum WoodcutterState { Inactive, Into, Walk, SawToss, AxeAttack, LogKick, Jump, Dead, Turn, Knockback}
public enum StartDirection { Left, Up, Right, Down};
public class GameControl : MonoBehaviour
{

    public delegate void Reset();
    public static event Reset ResetOnRest;

    public static GameControl control;
    public static int currentSaveSlot;
    public static string savePath;
    public static GameConfig gameConfig;
    public static SaveFile saveSlot1;
    public static SaveFile saveSlot2;
    public static SaveFile saveSlot3;
    public static SaveFile saveSlot4;
    public GameObject player;

    public static float Gravity = -4f;
    public static float TerminalVelocity = -6f;
    public static float UpSlopeSpeedOffset = .8f;
    public static float DownSlopeSpeedOffset = 1.4f;
    public static float HitStopDuration = .2f;
    public static float fixedDeltaTime;
    
    // Bosses 
    public bool woodcutterEncountered = false;
    public bool woodcutterDefeated = false;

    public PlayerStats playerStats;

    bool waitingForHitStop;


    public bool inMenu = true;
    public bool inInventroy = false; 
    public bool onQuickMap = false;
    public Camera MainCam;

    public Vector2 gameInput;

    bool downLastLS = false;
    bool upLastLS = false;
    bool rightLastLS = false;
    bool leftLastLS = false;

    public bool firstDownLS = false;
    public bool firstUpLS = false;
    public bool firstRightLS = false;
    public bool firstLeftLS = false;

    public float resetInputTimer;
    public float resetInputTime = .01f;

    float shakeAmount = 0;

    // scene change information
    public int currentScene;
    public Vector3 spawnPointPosition;
    public int restPointRoom = 1;
    public List<int> activeScenes;
    public List<BounderyManager> activeBoundrys;
    List<int> toRemove;
    public bool firstSceneLoad = true;
    public bool commandChangeRoom = false;
    // on Awake
    void Awake()
    {
        activeScenes.Add(0);
        savePath = Application.persistentDataPath + "/Slot";
        Debug.Log(savePath);
        LoadGameSlots();
        currentScene = 0;
        if (control == null)
        {
            //DontDestroyOnLoad(gameObject);
            control = this;
        }
        //else if (control != this)
        //{
        //    Destroy(gameObject);
        //}
        Time.timeScale = 1f;
        fixedDeltaTime = Time.fixedDeltaTime;
        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
        
        
        if (playerStats == null)
        {
            playerStats = new PlayerStats();
        }
        if (MainCam == null)
        {
            MainCam = Camera.main;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateQuality();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            PrintScenelist();
        }
        gameInput = LeftStickInput();
        //if (Input.GetKeyDown(KeyCode.K)) 
        //{
        //    playerStats.HitsLeft -= 1;
        //}
    }
    public void UpdateQuality()
    {
        QualitySettings.vSyncCount = 1;
    }

    public Vector2 LeftStickInput()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        if(inputX != 0 || inputY !=0)
        {
            resetInputTimer = resetInputTime;
            if (inputY < 0)
            {
                if (!downLastLS)
                {
                    Debug.Log("got key DOWN down");
                    firstDownLS = true;
                }
                else
                {
                    firstDownLS = false;
                }
                downLastLS = true;
            }

        }
        else if(inputX == 0 && inputY == 0)
        {
            if(resetInputTimer > 0)
            {
                resetInputTimer -= Time.deltaTime;
            }
            else
            {
                downLastLS = false;
            }
            
            
        }
        
        return new Vector2(inputX, inputY);

    }

    public void Rest()
    {
        restPointRoom = currentScene;
        playerStats.HitsLeft = playerStats.MaxHits;
        playerStats.EnergyLeft = playerStats.MaxEnergy;
        UnloadScenes();
        Debug.Log("Level Reset");
        spawnPointPosition = activeBoundrys[0].spawnPoint;
        Debug.Log("Rest Point Room: " + restPointRoom);
        SaveGameSlot();
    }
    public void Respawn()
    {
        player.transform.position = spawnPointPosition;
        playerStats.HitsLeft = playerStats.MaxHits;
        playerStats.EnergyLeft = playerStats.MaxEnergy;
        if(restPointRoom != 1)
        {
            playerStats.PlayerState = PlayerState.Rest;
        }
        else
        {
            playerStats.PlayerState = PlayerState.Walking;
        }
        UnloadScenes();
        SaveGameSlot();
    }
    void UnloadScenes()
    {
        foreach (int i in activeScenes)
        {
            if (i == 0 || i == restPointRoom)
            {
                continue;
            }
            else
            {
                SceneManager.UnloadSceneAsync(i);
            }
        }
        CleanActiveSceneList();
    }
    void CleanActiveSceneList()
    {
        activeScenes.Clear();
        activeScenes.Add(0);
        activeScenes.Add(restPointRoom);
        activeScenes.TrimExcess();
    }
    //Camera shake function
    public void Shake(float Amount, float length)
    {
        shakeAmount = Amount;
        InvokeRepeating("DoShake", 0, 0.01f);
        Invoke("StopShake", length);
    }
    void PrintScenelist()
    {
        Debug.Log("capacity:" + activeScenes.Capacity);
        Debug.Log("count:" + activeScenes.Count);
        Debug.Log("int foreach:");
        foreach (int i in activeScenes)
        {
            Debug.Log(i);
        }
        
    }

    void DoShake()
    {
        if (shakeAmount >= 0)
        {
            Vector3 camPos = MainCam.transform.position;

            float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
            float offsetY = Random.value * shakeAmount * 2 - shakeAmount;
            camPos.x += offsetX;
            camPos.y += offsetY;

            MainCam.transform.position = camPos;
        }
    }
    void StopShake()
    {
        CancelInvoke("DoShake");
        MainCam.transform.localPosition = Vector3.zero;
    }
    public void HitStop()
    {
        //exit if allready in a hit stop
        if (waitingForHitStop)
            return;
        Time.timeScale = 0.0f; // stop the time
        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
        StartCoroutine(WaitForHitStop(HitStopDuration)); // start the coroutine
    }
    // return the game to normal speed
    IEnumerator WaitForHitStop(float durtation)
    {
        waitingForHitStop = true;
        yield return new WaitForSecondsRealtime(durtation);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
        waitingForHitStop = false;
    }
    public void LoadGameSlots()
    {
        saveSlot1 = SaveLoadSystem.LoadGameData(1);
        saveSlot2 = SaveLoadSystem.LoadGameData(2);
        saveSlot3 = SaveLoadSystem.LoadGameData(3);
        saveSlot4 = SaveLoadSystem.LoadGameData(4);
    }
    public void LaunchGame(int slot)
    {
        // set what save slot will be saved to
        currentSaveSlot = slot;
        switch (slot) 
        {
            case 1:
                {
                    if(saveSlot1 != null)
                    {
                        playerStats = new PlayerStats(saveSlot1);
                        restPointRoom = saveSlot1.restPointRoom;
                    }
                    else // new save slot
                    {
                        playerStats = new PlayerStats();
                        restPointRoom = 1;
                        SaveLoadSystem.SaveGameData(currentSaveSlot);
                    }
                    break;
                }
            case 2:
                {
                    if (saveSlot1 != null)
                    {
                        playerStats = new PlayerStats(saveSlot2);
                        restPointRoom = saveSlot2.restPointRoom;
                    }
                    else // new save slot
                    {
                        playerStats = new PlayerStats();
                        restPointRoom = 1;
                        SaveLoadSystem.SaveGameData(currentSaveSlot);
                    }
                    break;
                }
            case 3:
                {
                    if (saveSlot1 != null)
                    {
                        playerStats = new PlayerStats(saveSlot3);
                        restPointRoom = saveSlot3.restPointRoom;
                    }
                    else // new save slot
                    {
                        playerStats = new PlayerStats();
                        SaveLoadSystem.SaveGameData(currentSaveSlot);
                    }
                    break;
                }
            case 4:
                {
                    if (saveSlot1 != null)
                    {
                        playerStats = new PlayerStats(saveSlot4);
                        restPointRoom = saveSlot4.restPointRoom;
                    }
                    else // new save slot
                    {
                        playerStats = new PlayerStats();
                        restPointRoom = 1;
                        SaveLoadSystem.SaveGameData(currentSaveSlot);
                    }
                    break;
                }
        }
        LoadStartingScene(restPointRoom);
        inMenu = false;
    }
    public void SaveGameSlot()
    {
        SaveLoadSystem.SaveGameData(currentSaveSlot);
    }
    public void ClearSave(int slot)
    {
        File.Delete(savePath + slot + ".sav");
        LoadGameSlots();
    }
    public void LoadScene(int sceneToLoad)
    {
        if (!activeScenes.Contains(sceneToLoad))
        {
            Debug.Log("Loaded scene " + sceneToLoad);
            activeScenes.Add(sceneToLoad);
            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        }
        else
        {
            if (commandChangeRoom)
            {
                activeScenes.Remove(sceneToLoad);
                SceneManager.UnloadSceneAsync(sceneToLoad);
                LoadScene(sceneToLoad);
            }
            else
            {
                Debug.LogWarning("Scene " + sceneToLoad + " has previously been loaded");
            }
            
        }
    }
    public void LoadStartingScene(int sceneToLoad)
    {
        if (!activeScenes.Contains(sceneToLoad))
        {
            activeScenes.Add(sceneToLoad);
            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            currentScene = sceneToLoad;
        }
    } 
    public void AddRemoveBoundry(BounderyManager check)
    {
        if (!activeBoundrys.Contains(check))
        {
            Debug.Log("Added Room Boundry " + check.SceneNumber);
            activeBoundrys.Add(check);
            currentScene = check.SceneNumber;
        }
        else
        {
            Debug.Log("Removed Room Boundry " + check.SceneNumber);
            activeBoundrys.Remove(check);
            activeBoundrys.TrimExcess();
        }
        if (activeBoundrys.Capacity == 1)
        {
            currentScene = activeBoundrys[0].SceneNumber; 
            Debug.Log("current scene: " + currentScene);
        }
    }
}

public class PlayerStats
{
    private PlayerState playerState = PlayerState.Walking;
    private PlayerState previousPalyerState = PlayerState.Walking;
    private int maxHits = 3;
    private float maxEnergy = 10;
    private int hitsLeft;
    private float energyLeft;

    // player unlocks
    private bool hasWallJump = false;
    private bool hasDoubleJump = false;
    private bool hasDash = false;
    private bool hasShootEctoplasm = false;

    private float phaseCost = 3;
    private float energyRechargeRate = 2;

    private int damage = 1;
    private bool isPhaseing = false;
    private RestPoint mostRecentRestPoint;

    private int RestPointRoom;

    public PlayerState PlayerState { get => playerState; set => playerState = value; }
    public int MaxHits { get => maxHits; set => maxHits = value; }
    public float MaxEnergy { get => maxEnergy; set => maxEnergy = value; }
    public int HitsLeft { get => hitsLeft; set => hitsLeft = value; }
    public float EnergyLeft { get => energyLeft; set => energyLeft = value; }
    public float PhaseCost { get => phaseCost; set => phaseCost = value; }
    public float EnergyRechargeRate { get => energyRechargeRate; set => energyRechargeRate = value; }
    public int Damage { get => damage; set => damage = value; }
    public RestPoint MostRecentRestPoint { get => mostRecentRestPoint; set => mostRecentRestPoint = value; }
    public bool IsPhaseing { get => isPhaseing; set => isPhaseing = value; }
    public bool HasWallJump { get => hasWallJump; set => hasWallJump = value; }
    public bool HasDoubleJump { get => hasDoubleJump; set => hasDoubleJump = value; }
    public bool HasDash { get => hasDash; set => hasDash = value; }
    public bool HasShootEctoplasm { get => hasShootEctoplasm; set => hasShootEctoplasm = value; }
    public PlayerState PreviousPalyerState { get => previousPalyerState; set => previousPalyerState = value; }

    public PlayerStats()
    {
        MaxHits = 3;
        MaxEnergy = 10;
        HitsLeft = MaxHits;
        EnergyLeft = MaxEnergy;
    }

    public PlayerStats(SaveFile saveFile)
    {
        maxHits = saveFile.maxHealth;
        maxEnergy = saveFile.maxEnergy;
        hasDoubleJump = saveFile.canDoubleJump;
        hasDash = saveFile.canDash;
        hasWallJump = saveFile.canWallJump;
        hasShootEctoplasm = saveFile.canShoot;
        HitsLeft = MaxHits;
        EnergyLeft = MaxEnergy;
    }
}