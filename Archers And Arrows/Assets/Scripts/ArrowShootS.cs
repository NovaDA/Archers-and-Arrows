using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CannonGame
{
    public class ArrowShootS : MonoBehaviour
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

        private void Awake()
        { 
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

            AimSlider.value = minlaunchForce;

            if (chargeLevel >= chargelimit && !fired)
            {
                chargeLevel = chargelimit;
                Fire();
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
            }
            else if (Input.GetMouseButtonUp(0) && !fired)
            {
                Fire();
                
            }
        }

        private void Fire()
        {
            fired = false;
            Rigidbody bullet = Instantiate(projectilePrefab, spawnPos.position, gameObject.transform.rotation) as Rigidbody;
            bullet.velocity = chargeLevel * spawnPos.forward;
            chargeLevel = minlaunchForce;
        }


    }
}

