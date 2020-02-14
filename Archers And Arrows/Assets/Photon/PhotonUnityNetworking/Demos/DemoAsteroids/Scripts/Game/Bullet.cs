using Photon.Realtime;
using UnityEngine;

namespace Photon.Pun.Demo.Asteroids
{
    public class Bullet : MonoBehaviour
    {
        public Player Owner { get; private set; }


        public AudioClip explosionClip;
        public GameObject explosionFX;
        private SphereCollider sphereCol;

        public void Start()
        {
            //Destroy(gameObject, 3.0f);
        }

        public void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);
        }

        public void InitializeBullet(Player owner, Vector3 originalDirection, float lag)
        {
            Owner = owner;

            transform.forward = originalDirection;

            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.velocity = originalDirection * 18f;
            rigidbody.position += rigidbody.velocity * lag;
        }
        void OnDespawn()
        {
            if (explosionFX)
                PoolManager.Spawn(explosionFX, transform.position, transform.rotation);
            if (explosionClip)
                AudioManager.Play3D(explosionClip, transform.position);

            myRigidbody.velocity = Vector3.zero;
            myRigidbody.angularVelocity = Vector3.zero;
        }
    }
}