using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketMovement : MonoBehaviour {
	public float movementSpeed = 10f;
	public Camera mainCamera;
	public Vector2 forwardBackwardRange = new Vector2(-20f, -5f); // Define the range for forward and backward 
	public float mouseLeftRightRange = 20f;
	public float mouseXRangeTop = 9f;
	public float mouseXRangeBottom = 18f;

	public float mouseXRotationRangeTop = 9f;
	public float mouseXRotationRangeBottom = 5f;

	public Vector2 mouseUpDownRange = new Vector2(-14f, -9f); // Define the range for forward and backward movement
	public float RacketZRotationRange = 90f;

	public float mouseXRotationForYRange = 70f;
	//public float mouseXRotationForYRangeBottom = 18f;
	//public float RacketXRotationRange = 70f;

	[SerializeField] Rigidbody rb;
	[SerializeField] Rigidbody ball;

	void FixedUpdate() {
		if (Input.GetMouseButtonDown(2)) {
			ball.velocity = Vector2.zero;
			ball.position = new(1.98f, 20.657f, -17.61f);
		}

		MoveRacket();
	}

	float rachetZForce = 0;

	void MoveRacket() {
		// Get the mouse position in screen space
		Vector3 mousePosition = Input.mousePosition;

		// Convert mouse position to world space
		Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.transform.position.y));

		// Move the racket left and right based on mouse X position
		float mouseXRange = Map(worldPosition.z, mouseUpDownRange.x, mouseUpDownRange.y, mouseXRangeBottom, mouseXRangeTop);
		float mouseXRotationRange = Map(worldPosition.z, mouseUpDownRange.x, mouseUpDownRange.y, mouseXRotationRangeBottom, mouseXRotationRangeTop);
		float newX = Map(worldPosition.x, -mouseLeftRightRange, mouseLeftRightRange, -mouseXRange, mouseXRange);


		// Move the racket forward and backward based on mouse Y position
		float newZ = Map(worldPosition.z, mouseUpDownRange.x, mouseUpDownRange.y, forwardBackwardRange.x, forwardBackwardRange.y);

		rachetZForce = newZ - transform.position.z;

    // Update the racket's position
    rb.MovePosition(new Vector3(newX, transform.position.y, newZ));

		// Make the racket look towards the mouse position
		float rotationZ = Map(newX, -mouseXRotationRange, mouseXRotationRange, RacketZRotationRange, -RacketZRotationRange);

		float rotationY = 0;

		if (Mathf.Abs(newX) > mouseXRangeBottom) {
			if (newX > 0) {
				rotationY = Map(newX, mouseXRangeBottom, mouseXRangeTop, 0f, -mouseXRotationForYRange);
			}
			else {
				rotationY = Map(newX, -mouseXRangeBottom, -mouseXRangeTop, 0f, mouseXRotationForYRange);
			}
		}

		//		Debug.Log("newX:" + newX + " worldX:" + worldPosition.x + " worldZ:" + worldPosition.z + " XRange:" + mouseXRange + " rotationY:" + rotationY);

		transform.rotation = Quaternion.Euler(0, rotationY, rotationZ);

	}

	private float Map(float value, float from1, float to1, float from2, float to2) {
		float normalizedValue = Mathf.InverseLerp(from1, to1, value);
		return Mathf.Lerp(from2, to2, normalizedValue);
	}

  private void OnCollisionEnter(Collision collision) {
    if (!collision.collider.name.Equals("Ball1")) return;

		var contact = collision.GetContact(0);
		//		Debug.DrawLine(contact.point, contact.point - contact.normal * 2, Color.red, 1);

		Vector3 normal = contact.normal;
    float z = normal.z;
    if (z < 0) return;
		normal.x = Mathf.Clamp(normal.x, -.1f, .1f); // Reduce the horizontal angle
		normal.y = Mathf.Abs(normal.y) + .1f; // Try to always move the ball a little bit up
		if (normal.z < rachetZForce) normal.z = rachetZForce; // Check if the speed of the rachet is better than normal velocity
		if (normal.z < 1) normal.z = 1; // Be sure forward velocity is at least 1

    Debug.DrawRay(contact.point, normal * 2, Color.red, 1);
		if (collision.collider.TryGetComponent(out Rigidbody ballRB)) {
			ballRB.AddForce(normal, ForceMode.Impulse);
			Debug.Log(normal.z);
		}
  }
}
