using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller {

	public float maxClimbAngle = 75;
	public float maxDescendAngle = 75;

	Player _player;
	Raycaster caster;
	Vector3 velocity;
	Vector3 oldVelocity;
	CollisionInfo collisions;

	public Controller(Player player) {
		_player = player;
		caster = new Raycaster(player.GetComponent<Collider2D>(), player.collisionMask);
	}
	// Movement handling
	public void Move(Vector3 newVelocity) {
		oldVelocity = velocity;
		velocity = newVelocity;
		caster.UpdateRaycastOrigins();
		collisions.Reset();

		if (velocity.y < 0) {
			DescendSlope();
		}
		if (velocity.x != 0) {
			MoveX();
		}
		if (velocity.y != 0) {
			MoveY();
		}
		_player.transform.Translate(velocity);
	}
	void MoveX() {
		caster.UpdateHorizontalRayParams(velocity.x);
		for (int rayIndex = 0; rayIndex < caster.horizontalCount; rayIndex++) {
			caster.CastHorizontal(rayIndex, 0);
			if (caster.hit) {
				HandleHorizontalCollision();
			}
		}
	}
	void MoveY() {
		caster.UpdateVerticalRayParams(velocity.y);
		for (int rayIndex = 0; rayIndex < caster.verticalCount; rayIndex++) {
			caster.CastVertical(rayIndex, velocity.x);
			if (caster.hit) {
				HandleVerticalCollision();
			}
		}
		if (IsClimbing()) {
			caster.UpdateHorizontalRayParams(velocity.x);
			caster.CastHorizontal(0, velocity.y);
			if (caster.hit && WillFindNewSlope()) {
				velocity.x = caster.HitDistance() * caster.directionX;
				caster.UpdateRayLength();
			}
		}
	}
	// Collision handling
	void HandleHorizontalCollision() {
		if (caster.IsOnFirstRay() && CanClimb()) {
			if (IsDescending()) {
				collisions.descending = false;
				velocity = oldVelocity;
			}
			float distanceToSlope = 0;
			if (FoundNewSlope()) {
				distanceToSlope = caster.HitDistance() * caster.directionX;
				velocity.x -= distanceToSlope;
			}
			ClimbSlope();
			velocity.x += distanceToSlope;
		}

		if (!IsClimbing() || !CanClimb()) {
			velocity.x = caster.HitDistance() * caster.directionX;
			caster.UpdateRayLength();

			if (IsClimbing()) {
				velocity.y = Mathf.Abs(velocity.x) * Tan(collisions.slopeAngle);
			}

			collisions.left = caster.directionX == -1;
			collisions.right = caster.directionX == 1;
		}
	}
	void HandleVerticalCollision() {
		velocity.y = caster.HitDistance() * caster.directionY;
		caster.UpdateRayLength();

		if (IsClimbing()) {
			velocity.x = caster.directionX * velocity.y / Tan(collisions.slopeAngle);
		}

		collisions.below = caster.directionY == -1;
		collisions.above = caster.directionY == 1;
	}
	// Slope handling
	void ClimbSlope() {
		float slopeTravelDistance = Mathf.Abs(velocity.x);
		float slopeVelocityY = slopeTravelDistance * Sin(caster.hitAngle);
		if (velocity.y <= slopeVelocityY) {
			velocity.y = slopeVelocityY;
			velocity.x = caster.directionX * slopeTravelDistance * Cos(caster.hitAngle);

			collisions.below = true;
			collisions.climbing = true;
			collisions.slopeAngle = caster.hitAngle;
		}
	}
	void DescendSlope() {
		caster.CastDescentRay(velocity.x);

		if (caster.hit) {
			if (!FoundFlatGround() && 
					CanDescend() && 
					MovingDownhill() && 
					FallIsInSlopeRange()) {
				float slopeTravelDistance = Mathf.Abs(velocity.x);
				float slopeVelocityY = slopeTravelDistance * Sin(caster.hitAngle);
				velocity.x = caster.directionX * slopeTravelDistance * Cos(caster.hitAngle);
				velocity.y -= slopeVelocityY;

				collisions.slopeAngle = caster.hitAngle;
				collisions.descending = true;
				collisions.below = true;
			}
		}
	}
	// Helper functions
	public bool IsGrounded() {
		return collisions.below;
	}
	public bool BumpedHead() {
		return collisions.above;
	}
	bool CanClimb() {
		return caster.hitAngle <= maxClimbAngle;
	}
	bool CanDescend() {
		return caster.hitAngle <= maxDescendAngle;
	}
	bool IsClimbing() {
		return collisions.climbing;
	}
	bool IsDescending() {
		return collisions.descending;
	}
	bool FoundNewSlope() {
		return caster.hitAngle != collisions.slopeAngleOld;
	}
	bool WillFindNewSlope() {
		return caster.hitAngle != collisions.slopeAngle;
	}
	bool FoundFlatGround() {
		return caster.hitAngle == 0;
	}
	bool MovingDownhill() {
		return Mathf.Sign(caster.hit.normal.x) == caster.directionX;
	}
	bool FallIsInSlopeRange() {
		return caster.HitDistance() <= Mathf.Abs(velocity.x) * Tan(caster.hitAngle);
	}
	// Wrappers for trig functions to prevent spread of Deg2Rad
	float Sin(float angle) {
		return Mathf.Sin(angle * Mathf.Deg2Rad);
	}
	float Cos(float angle) {
		return Mathf.Cos(angle * Mathf.Deg2Rad);
	}
	float Tan(float angle) {
		return Mathf.Tan(angle * Mathf.Deg2Rad);
	}
}

public struct CollisionInfo {
	public bool below, above, left, right;
	public float slopeAngle, slopeAngleOld;
	public bool climbing, descending;

	public void Reset() {
		below = above = left = right = false;
		climbing = descending = false;
		slopeAngleOld = slopeAngle;
		slopeAngle = 0;
	}
}