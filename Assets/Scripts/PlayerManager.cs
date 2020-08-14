using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;
using Photon.Pun.Demo.PunBasics;

namespace Com.MyComapany.MyGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(playerPoints);
            }
            else
            {
                playerPoints = (int)stream.ReceiveNext();
            }
        }

        #endregion



        #region Private Fields

        [Tooltip("The Player's point quantity")]
        [SerializeField]
        public int playerPoints;


        [Tooltip("RigidBody of the player")]
        [SerializeField]
        private Rigidbody rb;
        [Tooltip("Angles per second that the player turns when pressed a key to turn")]
        [SerializeField]
        private float turnRate = 2f;
        [Tooltip("Max velocity that the player can get to")]
        [SerializeField]
        private float maxVelocity = 60f;
        [Tooltip("How much the velocity will increase/decrease every second with the input pressed/unpressed")]
        [SerializeField]
        private float aceleration = 2f;
        [Tooltip("How much will the brakes brake, when the player velocity is higher then 0. !!!WARNING!!! this multiplicates the aceleration, so is the number is less than 1 it will brake the braking")]
        [SerializeField]
        private float brakeCoeficient = 4f;
        private float velocity;

        private bool controlStart, controlFinish;

        //Player velocity to use in late update
        private Vector3 playerVelocity = new Vector3(0, 0, 0);

        #endregion



        #region Public Fields

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;
        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUiPrefab;

        #endregion



        #region MonoBehaviour Callbacks

        private void Awake()
        {
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }
            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
            rb = GetComponent<Rigidbody>();
            if (!rb)
            {
                Debug.LogError("PlayerController is Missing RigidBody Component", this);
            }
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                ProcessInputs();
            }
        }

        void LateUpdate()
        {
            if (photonView.IsMine)
            {
                if (playerVelocity.magnitude != 0)
                {
                    rb.velocity = playerVelocity;
                }
                else
                {
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                }
            }
        }
        
        void OnCollisionEnter(Collision collision)
        {
            if (photonView.IsMine)
            {
                if (collision.gameObject.CompareTag("Wall"))
                {
                    velocity = 0;
                }
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Finish"))
            {
                if (controlStart && controlFinish)
                {
                    playerPoints++;
                }
                controlStart = false;
                controlFinish = false;
            }
            else if (collider.CompareTag("ControlStart"))
            {
                controlStart = true;
            }
            else if (collider.CompareTag("ControlFinish"))
            {
                controlFinish = true;
            }
        }

        #endregion



        #region Private Methods
        


        #endregion



        #region Custom

        void ProcessInputs()
        {
            if (photonView.IsMine)
            {
                if (Input.GetAxisRaw("Horizontal") != 0)
                {
                    transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * turnRate, 0), Space.Self);
                }
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    //Going forward
                    if (velocity < maxVelocity)
                    {
                        //Braking
                        if (velocity < 0)
                        {
                            velocity += brakeCoeficient * aceleration * Time.deltaTime;
                        }
                        else
                        {
                            velocity += aceleration * Time.deltaTime;
                        }
                        if (velocity > maxVelocity)
                        {
                            velocity = maxVelocity;
                        }
                    }
                }
                if (Input.GetAxisRaw("Vertical") < 0)
                {
                    //Going backwards
                    if (velocity > -maxVelocity / 2)
                    {
                        //Braking
                        if (velocity > 0)
                        {
                            velocity -= brakeCoeficient * aceleration * Time.deltaTime;
                        }
                        else
                        {
                            velocity -= aceleration * Time.deltaTime;
                        }
                        if (velocity < -maxVelocity / 2)
                        {
                            velocity = -maxVelocity / 2;
                        }
                    }
                }
                if (Input.GetAxisRaw("Vertical") == 0)
                {
                    if (velocity > 0.25f)
                    {
                        //Stopping from positive velocity
                        velocity -= aceleration * Time.deltaTime;
                    }
                    else if (velocity < -0.25f)
                    {
                        //Stopping from negative velocity
                        velocity += aceleration * Time.deltaTime;
                    }
                    else
                    {
                        //Complete stop
                        velocity = 0;
                    }
                }
                playerVelocity = new Vector3(velocity * transform.forward.x, rb.velocity.y, velocity * transform.forward.z);
            }
        }

        #endregion
    }
}
