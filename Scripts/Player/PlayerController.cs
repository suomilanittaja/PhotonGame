using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{

    [Header("Floats")]
    public float Speed = 10f;
    public float moveSpeed;
    public float sprintSpeedMultiplier = 2f;
    public float jumpHeight = 3f;
    private float _gravity = -10f;
    private float _yAxisVelocity;

    [Header("GameObjects")]
    public GameObject Cam;
    public GameObject Remote;

    [Header("Bools")]
    public bool disableMouselook = false;

    [Header("Others")]
    public CharacterController characterController;
    public const string PlayerTag = "Player";
    public Transform gunTransform;
    public PlayerCamera playerCamera;

    private void Start()
    {
        //check if player is mine
        if (photonView.IsMine)
        {
            gameObject.tag = "Player";
            Remote.SetActive(false);
        }
        else //if not
        {
            Cam.SetActive(false);
            gameObject.tag = "Enemy";
        }

    }

    private void Update()
    {

        if (photonView.IsMine) //if player is mine enable moving
        {
            InputMovement();

            playerCamera = Cam.GetComponent<PlayerCamera>();

            if (Input.GetMouseButtonDown(0)) //if left click then make damage
            {
                photonView.RPC("RPC_hit", RpcTarget.All);
            }

            Cam.gameObject.SetActive(true);

            if (disableMouselook == true)
                playerCamera.enabled = false;
            else
                playerCamera.enabled = true;

        }
    }


    // used as Observed component in a PhotonView, this only reads/writes the position
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Vector3 pos = transform.localPosition;
            stream.Serialize(ref pos);
        }
        else
        {
            Vector3 pos = Vector3.zero;
            stream.Serialize(ref pos);  // pos gets filled-in. must be used somewhere
        }
    }


    void InputMovement() //moving
    {

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
            vertical *= sprintSpeedMultiplier;

        Vector3 movement = horizontal * moveSpeed * Time.deltaTime * transform.right +
                           vertical * moveSpeed * Time.deltaTime * transform.forward;

        if (characterController.isGrounded)
            _yAxisVelocity = -0.5f;


        if (Input.GetKeyDown(KeyCode.Space))  //jumping
        {
            _yAxisVelocity = Mathf.Sqrt(jumpHeight * -2f * _gravity);
        }

        _yAxisVelocity += _gravity * Time.deltaTime;
        movement.y = _yAxisVelocity * Time.deltaTime;

        characterController.Move(movement);
    }

    [PunRPC]
    void RPC_hit() //hit
    {
        Ray ray = new Ray(gunTransform.position, gunTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 4f))
        {
            var enemyHealth = hit.collider.GetComponent<Health>();
            if (enemyHealth)
            {
                enemyHealth.TakeDamage(20);
            }
            
        }
    }
}
