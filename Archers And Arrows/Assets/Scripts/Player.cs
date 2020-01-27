using System;
using UnityEngine;

namespace TanksMP
{
	public class Player : MonoBehaviour
	{       
		/// <summary>
		/// Delay between shots.
		/// </summary>
		public float fireRate = 0.75f;

		/// <summary>
		/// Movement speed in all directions.
		/// </summary>
		public float moveSpeed = 8f;

		/// <summary>
		/// Clip to play when a shot has been fired.
		/// </summary>
		public AudioClip shotClip;
        
		/// <summary>
		/// Object to spawn on shooting.
		/// </summary>
		public GameObject shotFX;
        
		/// <summary>
		/// Position to spawn new bullets at.
		/// </summary>
		public Transform shotPos;
      
		/// <summary>
		/// Bullet object for shooting.
		/// </summary>
		public GameObject bullet;
        
		/// <summary>
		/// Reference to the camera following component.
		/// </summary>
		[HideInInspector]
		public FollowTarget camFollow;

		//timestamp when next shot should happen
		private float nextFire;

		private Rigidbody rb;

		public void Awake ()
		{
			rb = GetComponent<Rigidbody> ();

			camFollow = Camera.main.GetComponent<FollowTarget>();
			camFollow.target = transform;
		}

		private void FixedUpdate()
		{
			if((rb.constraints & RigidbodyConstraints.FreezePositionY) != RigidbodyConstraints.FreezePositionY)
			{
				if(transform.position.y > 0)
				{
					rb.AddForce(Physics.gravity * 2f, ForceMode.Acceleration);
				}
			}

			Vector2 moveDir;

			if(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
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

			if(Input.GetKey(KeyCode.Space))
			{
				Shoot();
			}
		}

		private void Shoot(Vector2 direction = default(Vector2))
		{
			if(Time.time > nextFire)
			{
				nextFire = Time.time + fireRate;

				GameObject obj = PoolManager.Spawn(bullet, shotPos.position, transform.rotation);
				Bullet blt = obj.GetComponent<Bullet>();

				if (shotFX)
					PoolManager.Spawn(shotFX, shotPos.position, Quaternion.identity);
				if (shotClip)
					AudioManager.Play3D(shotClip, shotPos.position, 0.1f);
			}
		}

		void Move(Vector2 direction = default(Vector2))
		{
			if(direction != Vector2.zero)
			{
				transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y))
					* Quaternion.Euler(0, camFollow.camTransform.eulerAngles.y, 0);
			}

			Vector3 movementDir = transform.forward * moveSpeed * Time.deltaTime;
			rb.MovePosition(rb.position + movementDir);
		}


	}
}