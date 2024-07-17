using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketMovement : MonoBehaviour
{
	public float movementSpeed = 10f;
	public Camera mainCamera;
	public Vector2 forwardBackwardRange = new Vector2(-20f, -5f); // Define the range for forward and backward 
	public float mouseLeftRightRange = 20f;
	public float mouseXRangeTop = 9f;
	public float mouseXRangeBottom = 18f;

	public float mouseXRotationRangeTop = 9f;
	public float mouseXRotationRangeBottom = 5f;

	public Vector2 mouseUpDownRange = new Vector2(-14f, -9f); // Define the range for forward and backward movement
	public float RacketZRotationRange = 80f;


	void Update()
	{
		MoveRacket();
	}

	void MoveRacket()
	{
		// Get the mouse position in screen space
		Vector3 mousePosition = Input.mousePosition;

		// Convert mouse position to world space
		Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.transform.position.y));

		// Move the racket left and right based on mouse X position
		float mouseXRange = Map(worldPosition.z, mouseUpDownRange.x, mouseUpDownRange.y, mouseXRangeBottom, mouseXRangeTop);
		float mouseXRotationRange = Map(worldPosition.z, mouseUpDownRange.x, mouseUpDownRange.y, mouseXRotationRangeBottom, mouseXRotationRangeTop);
		float newX = Map(worldPosition.x, -mouseXRange, mouseXRange, -mouseXRangeTop, mouseXRangeTop);
		//float newX = worldPosition.x;

		Debug.Log("newX:" + newX + " worldX:" + worldPosition.x + " worldZ:" + worldPosition.z + " XRange:" + mouseXRange);

		// Move the racket forward and backward based on mouse Y position
		float newZ = Map(worldPosition.z, mouseUpDownRange.x, mouseUpDownRange.y, forwardBackwardRange.x, forwardBackwardRange.y);

		// Update the racket's position
		transform.position = new Vector3(newX, transform.position.y, newZ);

		// Make the racket look towards the mouse position
		transform.rotation = Quaternion.Euler(0, 0, Map(newX, -mouseXRotationRange, mouseXRotationRange, RacketZRotationRange, -RacketZRotationRange));

	}

	private float Map(float value, float from1, float to1, float from2, float to2)
	{
		float normalizedValue = Mathf.InverseLerp(from1, to1, value);
		return Mathf.Lerp(from2, to2, normalizedValue);
	}
}
