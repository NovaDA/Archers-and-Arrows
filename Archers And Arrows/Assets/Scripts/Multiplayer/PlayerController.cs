using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Pun.Demo.Asteroids;

namespace RhinoGame
{
    public class PlayerController : MonoBehaviour
    {
        public float Health = 100;
        public float RotationSpeed = 8.0f;
        public float MovementSpeed = 10f;

        /// <summary>
        /// Delay between shots.
        /// </summary>
        /// 
        
        public float FireRate = 0.75f;
        public GameObject BulletPrefab;
        [HideInInspector]
        public FollowTarget camFollow;
        [HideInInspector]
        public bool controllable = true;
        public Transform ShootingPos;

        private PhotonView photonView;
        private new Rigidbody rigidbody;
        //timestamp when next shot should happen
        private float nextFire;

        public Text playername;
        

        public void Awake()
        {
            photonView = GetComponent<PhotonView>();

            rigidbody = GetComponent<Rigidbody>();

            if (photonView.IsMine)
            {
                camFollow = Camera.main.GetComponent<FollowTarget>();
                camFollow.target = transform;
                
            }

            SetPlayerUI();
        }


        [PunRPC]
        public void Damage(float damage)
        {
            Health -= damage;
        }

        public void Update()
        {
            if (!photonView.IsMine || !controllable)
            {
                return;
            }

            Vector2 moveDir;

            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                moveDir.x = 0;
                moveDir.y = 0;
                GetComponent<PhotonView>().RPC("ActivateTrail", RpcTarget.AllViaServer, false);
            }
            else
            {
                moveDir.x = Input.GetAxis("Horizontal");
                moveDir.y = Input.GetAxis("Vertical");
                Move(moveDir);
                GetComponent<PhotonView>().RPC("ActivateTrail", RpcTarget.AllViaServer, true);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                photonView.RPC("Fire", RpcTarget.AllViaServer, transform.rotation);
                
            }
        }

        public void FixedUpdate()
        {
            if (!photonView.IsMine || !controllable)
            {
                return;
            }

            if ((rigidbody.constraints & RigidbodyConstraints.FreezePositionY) != RigidbodyConstraints.FreezePositionY)
            {
                if (transform.position.y > 0)
                {
                    rigidbody.AddForce(Physics.gravity * 2f, ForceMode.Acceleration);
                }
            }
        }

        void Move(Vector2 direction = default(Vector2))
        {
            if (direction != Vector2.zero)
            {
                transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y))
                    * Quaternion.Euler(0, camFollow.camTransform.eulerAngles.y, 0);
            }

            Vector3 movementDir = transform.forward * MovementSpeed * Time.deltaTime;
            rigidbody.MovePosition(rigidbody.position + movementDir);
            
        }


        private void SetPlayerUI()
        {
            if(playername != null)
            {
                playername.text = photonView.Owner.NickName;
                playername.color = AsteroidsGame.GetColor(photonView.Owner.ActorNumber - 1);    // Actor Numbers Starts Always From 1
            }    
        }


        [PunRPC]
        public void Fire(Quaternion rotation, PhotonMessageInfo info)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + FireRate;
                float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

                GameObject bullet = Instantiate(BulletPrefab, ShootingPos.position, Quaternion.identity) as GameObject;
                bullet.GetComponent<PlayerBullet>().InitializeBullet(photonView.Owner, (rotation * Vector3.forward), Mathf.Abs(lag));

            }
        }



    }
}