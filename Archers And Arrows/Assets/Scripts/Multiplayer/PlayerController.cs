using UnityEngine;
using Photon.Pun;

namespace RhinoGame
{
    public class PlayerController : MonoBehaviour
    {
        public float RotationSpeed = 8.0f;
        public float MovementSpeed = 10f;

        /// <summary>
        /// Delay between shots.
        /// </summary>
        public float FireRate = 0.75f;

        public ParticleSystem Destruction;
        public GameObject Trail;
        public GameObject BulletPrefab;
        [HideInInspector]
        public FollowTarget camFollow;
        public Transform ShootingPos;

        private PhotonView photonView;
        private new Rigidbody rigidbody;
        //timestamp when next shot should happen
        private float nextFire;

        public void Awake()
        {
            photonView = GetComponent<PhotonView>();

            rigidbody = GetComponent<Rigidbody>();

            if (photonView.IsMine)
            {
                camFollow = Camera.main.GetComponent<FollowTarget>();
                camFollow.target = transform;
            }
        }

        public void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            Vector2 moveDir;

            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                moveDir.x = 0;
                moveDir.y = 0;
            }
            else
            {
                moveDir.x = Input.GetAxis("Horizontal");
                moveDir.y = Input.GetAxis("Vertical");
                Move(moveDir);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                photonView.RPC("Fire", RpcTarget.AllViaServer, transform.rotation);
            }
        }

        public void FixedUpdate()
        {
            if (!photonView.IsMine)
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


        [PunRPC]
        public void Fire(Quaternion rotation, PhotonMessageInfo info)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + FireRate;
                float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

                GameObject bullet = Instantiate(BulletPrefab, ShootingPos.position, Quaternion.identity) as GameObject;
                bullet.GetComponent<Photon.Pun.Demo.Asteroids.Bullet>().InitializeBullet(photonView.Owner, (rotation * Vector3.forward), Mathf.Abs(lag));

            }
        }

    }
}