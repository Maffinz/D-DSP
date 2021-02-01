using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    protected Transform PlayerTransform;
    private float MoveSpeed = 5;
    private PhotonView photonView;

    public static bool isEditing { get; set; }

    // Testin
    private GameObject GridManager;
    [SerializeField] private Tilemap tilemap;

    private Vector3Int TargetPosition;
    private Vector3 NewTargePosition;
    
    //Zoom 
    [SerializeField] private Camera myCamera;
    public float ZoomSpeed = 0.5f; // Default 5

    public void Start()
    {
        // Set PlayerTransform to its transform
        PlayerTransform = this.GetComponent<Transform>();
        photonView = this.GetComponent<PhotonView>();
        myCamera = this.transform.GetChild(0).GetComponent<Camera>();

        // Testing
        GridManager = GameObject.Find("GridManager");
        tilemap = GridManager.transform.GetChild(1).GetComponent<Tilemap>();

        TargetPosition = new Vector3Int(0, 0, 0);

        if(!photonView.IsMine)
        {
            myCamera.enabled = false;
        }

        isEditing = false;
    }

    public void Update()
    {
        if(photonView.IsMine)
        {
            if(!isEditing)
                CheckInput();
        }

        // Scroll
        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            myCamera.orthographicSize += ZoomSpeed;
        }
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && myCamera.orthographicSize > 1.5)
        {
            if (myCamera.orthographicSize < 1.5) myCamera.orthographicSize = 1.5f;
            else myCamera.orthographicSize -= ZoomSpeed;
        }

    }

    private void CheckInput()
    {
        //var move = new Vector3(Input.GetAxis("Horizontal"), 0);
        //transform.position += move * MoveSpeed * Time.deltaTime;

        // Get Position Move Towards
        if(Input.GetMouseButtonDown(0))
        {
            // Testing
            NewTargePosition = ClickPosition();
            // Set Click Position
            TargetPosition = CellPosition(ClickPosition());

            // Offset
            TargetPosition.x = TargetPosition.x + 1;
            TargetPosition.y = TargetPosition.y + 1;

            NewTargePosition.x = (float)((double)TargetPosition.x - .50);
            NewTargePosition.y = (float)((double)TargetPosition.y - .50);

            // Debug
            Debug.Log("Target Position: " + NewTargePosition);
        }

        // Move Towards
        if(PlayerTransform.position != NewTargePosition)
        {
            PlayerTransform.position = Vector2.MoveTowards(PlayerTransform.position, new Vector2(NewTargePosition.x, NewTargePosition.y), MoveSpeed * Time.deltaTime);
        }

        
    }

    private Vector3 ClickPosition()
    {
        return myCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private Vector3Int CellPosition(Vector3 clickPos)
    {
        return tilemap.WorldToCell(clickPos);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hitting Something: " + collision.collider.offset);
    }
}
