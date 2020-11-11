using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounderyManager : MonoBehaviour
{
    private BoxCollider2D managerBox;
    private Transform player;
    public GameObject boundary;
    public int SceneNumber;
    public bool active;
    public Vector3 spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        managerBox = GetComponent<BoxCollider2D>();
    }
    private void OnEnable()
    {
        player = GameControl.control.player.transform;
        if (GameControl.control.firstSceneLoad)
        {
            GameControl.control.player.transform.position = spawnPoint;
            GameControl.control.spawnPointPosition = spawnPoint;
            GameControl.control.firstSceneLoad = false;
        }
        if (GameControl.control.commandChangeRoom)
        {
            GameControl.control.player.transform.position = spawnPoint;
            Debug.Log(GameControl.control.player.transform.position);
            GameControl.control.commandChangeRoom = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        BoundaryCheck();
    }
    void BoundaryCheck()
    {

        if(managerBox.bounds.min.x < player.position.x && player.position.x < managerBox.bounds.max.x && managerBox.bounds.min.y < player.position.y && player.position.y < managerBox.bounds.max.y)
        {
            if (!active)
            {
                boundary.SetActive(true);
                active = true;
                GameControl.control.AddRemoveBoundry(this);
            }    
        }
        else
        {
            if (active)
            {
                active = false;
                boundary.SetActive(false);
                GameControl.control.AddRemoveBoundry(this);
            }
        }
    }

    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnPoint, .3f);
        

    }
}
