using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class Player : MonoBehaviour
{
	 	public float moveSpeed = 6;
		public float jumpHeight = 4;
		public float timeToJumpApex = .4f;
		public float accelerationTimeAir = .4f;
		public float accelerationTimeGround = .05f;

		float gravity;
		float jumpSpeed;
		Vector3 velocity;
		Controller controller;
		public LayerMask collisionMask;
		float tempVelocityX;

    void Start()
    {
			controller = new Controller(this);
			gravity = -2 * jumpHeight / Mathf.Pow(timeToJumpApex, 2);
			jumpSpeed = -gravity * timeToJumpApex;
    }

    void Update()
    {
			if (controller.IsGrounded() || controller.BumpedHead()) {
				velocity.y = 0;
			}

			Vector2 input = new Vector2(
				Input.GetAxisRaw("Horizontal"),
				Input.GetAxisRaw("Vertical")
			);

			if (Input.GetKeyDown(KeyCode.Space) && controller.IsGrounded()){
				velocity.y = jumpSpeed;
			}

			float targetVelocityX = input.x * moveSpeed;
			velocity.x = Mathf.SmoothDamp(
				velocity.x, 
				targetVelocityX,
				ref tempVelocityX,
				controller.IsGrounded() ? accelerationTimeGround : accelerationTimeAir
			);
			velocity.y += gravity * Time.deltaTime;

			controller.Move(velocity * Time.deltaTime);
    }
}
