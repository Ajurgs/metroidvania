using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraFollow : MonoBehaviour
{
    private BoxCollider2D cameraBox;
    [SerializeField]
    private Camera mainCamera;
    private Transform player;

    float cameraSize;
    float cameraHeight;

    // Start is called before the first frame update
    void Start()
    {
        GameControl.control.MainCam = mainCamera;
        cameraBox = GetComponent<BoxCollider2D>();
        player = GameControl.control.player.GetComponent<Transform>();
        
        mainCamera.orthographic = false;
        mainCamera.fieldOfView = 100;
        cameraSize = Camera.main.orthographicSize;
        cameraHeight = cameraSize * 2;
        AspectRatioBoxUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        
        AspectRatioBoxUpdate();
        FollowPlayer();
    }

    void AspectRatioBoxUpdate() // resize the cameras hitbox depending on the aspect ratio;
    {
        cameraBox.size = new Vector2(Mathf.Ceil(cameraHeight * Camera.main.aspect), cameraHeight);
    }
    void FollowPlayer()
    {
        if (GameObject.Find("Boundary")) // prevent null ref exception
        {
            float targetX = Mathf.Clamp(player.position.x, GameObject.Find("Boundary").GetComponent<BoxCollider2D>().bounds.min.x + cameraBox.size.x / 2, GameObject.Find("Boundary").GetComponent<BoxCollider2D>().bounds.max.x - cameraBox.size.x / 2);
            float targetY = Mathf.Clamp(player.position.y, GameObject.Find("Boundary").GetComponent<BoxCollider2D>().bounds.min.y + cameraBox.size.y / 2, GameObject.Find("Boundary").GetComponent<BoxCollider2D>().bounds.max.y - cameraBox.size.y / 2);

            transform.position = new Vector3(targetX, targetY , -10);
        }
    }



}
