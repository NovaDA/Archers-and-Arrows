using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RhinoGame;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class ArrowBehaviour : MonoBehaviour
{

    PhotonView photonView;
    #region Explosion
    public LayerMask enemies;
    public GameObject explosion_particles;
    public AudioClip explosionAudio;
    public float Maxdamage = 100f;
    public float explosionForce = 1000f;
    public float maxLifeTime = 2f;
    public float explosionRadius = 5f;
    #endregion


    Rigidbody rb;
    public Photon.Realtime.Player Owner { get; private set; }
    private Renderer renderer;

    // Start is called before the first frame update
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
    }


    private void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, enemies);

    //    for (int i = 0; i < colliders.Length; i++)
    //    {
    //        Rigidbody target = colliders[i].GetComponent<Rigidbody>();

    //        if (!target)
    //            continue;

    //        target.AddExplosionForce(explosionForce, transform.position, explosionRadius);

    //        Unit enemyHealth = target.GetComponent<Unit>();

    //        if (!enemyHealth)
    //            continue;

    //        float damage = CalculateDamage(target.position);
    //        enemyHealth.TakeDamage((int)damage);
    //    }

    //    explosion_particles.transform.parent = null;
    //    PoolManager.Spawn(explosion_particles, transform.position, transform.rotation);
    //    AudioManager.Play3D(explosionAudio, transform.position);

    //   /// Destroy(explosion_particles.gameObject, explosion_particles.main.duration);
    //    Destroy(gameObject);
    //}

    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDist = explosionToTarget.magnitude;
        float relativeDistance = (explosionRadius - explosionDist) / explosionRadius;
        float damage = relativeDistance * Maxdamage;
        damage = Mathf.Max(0f, damage);
        return damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        #region NOT USED
        //Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, enemies);

        //for (int i = 0; i < colliders.Length; i++)
        //{
        //    Rigidbody target = colliders[i].GetComponent<Rigidbody>();

        //    if (!target)
        //        continue;

        //    target.AddExplosionForce(explosionForce, transform.position, explosionRadius); // v

        //    var playerController = collision.gameObject.GetComponent<PlayerController>();

        //    if (!playerController)
        //        continue;

        //    float damage = CalculateDamage(target.position);
        //    Debug.Log(damage);

        //    // playerController.Damage(damage);
        //    playerController.Damage((int)damage);
        //    if (playerController.Health <= 0)
        //    {
        //        collision.gameObject.GetComponent<PhotonView>().RPC("DestroyPlayer", RpcTarget.All);
        //        Owner.AddScore(1);
        //    }
        //}
        #endregion

        explosion_particles.transform.parent = null;
        PoolManager.Spawn(explosion_particles, transform.position, transform.rotation);
        AudioManager.Play3D(explosionAudio, transform.position);
        CheckForObjectCollision();
        /// Destroy(explosion_particles.gameObject, explosion_particles.main.duration);
        Destroy(gameObject);
    }

    private void CheckForObjectCollision()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.name == "Damage")
            {
                Rigidbody target = colliders[i].GetComponentInParent<Rigidbody>();

                if (!target)
                    continue;

                target.AddExplosionForce(explosionForce, transform.position, explosionRadius); // v

                var Enemy = target.GetComponent<PlayerController>();

                if (!Enemy)
                    continue;

                float damage = CalculateDamage(target.position);
                Debug.Log(damage);
                target.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, (int)damage);
                Enemy.Damage((int)damage);

                if (Enemy.Health <= 0)
                {
                    target.gameObject.GetComponent<PhotonView>().RPC("DestroyPlayer", RpcTarget.All);
                    Owner.AddScore(1);
                }
            }
        }
    }

    public void InitializeArrow(Photon.Realtime.Player owner, Vector3 originalDirection, float lag, float charge)
    {
        Owner = owner;
        transform.forward = originalDirection;
        rb.velocity = originalDirection * charge;
        rb.position += rb.velocity * lag;

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.forward = Vector3.Slerp(gameObject.transform.forward, rb.velocity.normalized, Time.deltaTime*10);
        
    }
}
