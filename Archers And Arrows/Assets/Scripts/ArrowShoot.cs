using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace CannonGame
{

    public class ArrowShoot : MonoBehaviour
    {

        #region Projectile Properties
        float chargeLevel;
        public float chargerSpeed;
        public float chargelimit;
        public float minlaunchForce;
        bool fired;
        public Rigidbody projectilePrefab;
        public Transform spawnPos;
        #endregion

        #region Aim Arrow UI
        public Slider AimSlider;
        #endregion

        public float FireRate = 0.75f;
        private float nextFire;
        private PhotonView photonView;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
        }
        private void OnEnable()
        {
            chargeLevel = minlaunchForce;
            AimSlider.value = minlaunchForce;
        }
        private void Start()
        {
            chargerSpeed = chargelimit - minlaunchForce;
        }
        // Update is called once per frame
        void Update()
        {

            if (!photonView.IsMine)
                return;


            AimSlider.value = minlaunchForce;

            if (chargeLevel >= chargelimit && !fired)
            {
                Debug.Log("Condition 1 verified");
                chargeLevel = chargelimit;
                photonView.RPC("Firing", RpcTarget.AllViaServer, transform.rotation, chargeLevel);
                //Fire();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                fired = false;
                chargeLevel = minlaunchForce;
            }
            else if (Input.GetMouseButton(0) && !fired)
            {
                chargeLevel += chargerSpeed * Time.deltaTime;
                AimSlider.value = chargeLevel;
                Debug.Log(chargeLevel);

            }
            else if (Input.GetMouseButtonUp(0) && !fired)
            {
                photonView.RPC("Firing", RpcTarget.AllViaServer, transform.rotation, chargeLevel);
                
            }
            

        }

        [PunRPC]
        private void Firing(Quaternion rotation, float charge, PhotonMessageInfo info)
        {
            if (Time.time > nextFire)
            {
                fired = false;
                nextFire = Time.time + FireRate;
                float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
                Rigidbody bullet = Instantiate(projectilePrefab, spawnPos.position, spawnPos.transform.rotation) as Rigidbody;
                bullet.GetComponent<ArrowBehaviour>().InitializeArrow(photonView.Owner, spawnPos.forward , Mathf.Abs(lag), charge);
                chargeLevel = 0;
            }

        }
    }
}

