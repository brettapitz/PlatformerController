using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycaster {
	Player _player;
	LayerMask mask;
	Collider2D collider;

	RaycastOrigins origins;
	Vector2 rayOrigin;
	float rayLength;
	public int horizontalCount = 4;
	public int verticalCount = 4;
	float horizontalSpacing;
	float verticalSpacing;
	float skinWidth = 0.015f;

	public RaycastHit2D hit;
	public float hitAngle = 0;
	public float directionX, directionY;

	int index;

	public Raycaster(Collider2D selfCollider, LayerMask collisionMask) {
		collider = selfCollider;
		mask = collisionMask;
		CalculateRaySpacing();
	}

	public void UpdateHorizontalRayParams(float deltaX) {
		directionX = Mathf.Sign(deltaX);
		rayLength = Mathf.Abs(deltaX) + skinWidth;
		rayOrigin = directionX == -1 ? origins.bottomLeft : origins.bottomRight;
	}
	public void UpdateVerticalRayParams(float deltaY) {
		directionY = Mathf.Sign(deltaY);
		rayLength = Mathf.Abs(deltaY) + skinWidth;
		rayOrigin = directionY == -1 ? origins.bottomLeft : origins.topLeft;
	}

	public void CastHorizontal(int rayIndex, float verticalOffset) {
		// Debug.DrawRay(
		// 	rayOriginX + Vector2.up * horizontalRaySpacing * rayIndex,
		// 	Vector2.right * directionX * rayLength,
		// 	Color.red
		// );
		index = rayIndex;
		hit = Physics2D.Raycast(
			rayOrigin + Vector2.up * (horizontalSpacing * index + verticalOffset),
			Vector2.right * directionX,
			rayLength,
			mask
		);
		UpdateAngle();
	}
	public void CastVertical(int rayIndex, float horizontalOffset) {
		// Debug.DrawRay(
		// 	rayOriginY + Vector2.right * (verticalRaySpacing * rayIndex + velocity.x),
		// 	Vector2.up * directionY * rayLength,
		// 	Color.red
		// );
		index = rayIndex;
		hit = Physics2D.Raycast(
			rayOrigin + Vector2.right * (verticalSpacing * index + horizontalOffset),
			Vector2.up * directionY,
			rayLength,
			mask
		);
		UpdateAngle();
	}
	public void CastDescentRay(float deltaX) {
		directionX = Mathf.Sign(deltaX);
		rayOrigin = directionX == -1 ? origins.bottomRight : origins.bottomLeft;
		hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, mask);
		UpdateAngle();
	}
	void UpdateAngle() {
		if (!hit) return;
		hitAngle = Vector2.Angle(hit.normal, Vector2.up);
	}

	public float HitDistance() {
		return hit.distance - skinWidth;
	}

	public void UpdateRayLength() {
		rayLength = hit.distance;
	}

	public void UpdateRaycastOrigins() {
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);

		origins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		origins.topRight = new Vector2(bounds.max.x, bounds.max.y);
		origins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		origins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
	}

	void CalculateRaySpacing() {
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);

		horizontalSpacing = bounds.size.y / (horizontalCount - 1);
		verticalSpacing = bounds.size.x / (verticalCount - 1);
	}

	public bool IsOnFirstRay() {
		return index == 0;
	}
}

public struct RaycastOrigins {
	public Vector2 topLeft, topRight, bottomLeft, bottomRight;
}