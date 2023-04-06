using UnityEngine;

namespace Dracula {
	public class Camera : MonoBehaviour {
		private static UnityEngine.Camera instance;

		[Header("Speed")]
		[SerializeField] private float moveSpeed = 5.0f;
		[SerializeField] private float zoomSpeed = 10.0f;

		[Header("Move Clamp")]
		[SerializeField] private float positionMinX = -20.0f;
		[SerializeField] private float positionMaxX = 20.0f;
		[SerializeField] private float positionMinZ = -20.0f;
		[SerializeField] private float positionMaxZ = 20.0f;

		[Header("Zoom Clamp")]
		[SerializeField] private float zoomMin = -10.0f;
		[SerializeField] private float zoomMax = 1.0f;

		private Vector3 position = Vector3.zero;
		private float zoom = 0.0f;

		private void Awake() {
			// singleton
			instance = GetComponent<UnityEngine.Camera>();

			position = transform.position;
			zoom = 0.0f;
		}

		private void Update() {
			float x = Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime;
			float y = Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime;

			Vector3 direction = Vector3.right * x + Vector3.forward * y;
			bool speeding = Input.GetKey(KeyCode.LeftShift);

			position += Vector3.ClampMagnitude(direction, moveSpeed) * (speeding ? 2.0f : 1.0f);
			position.x = Mathf.Clamp(position.x, positionMinX, positionMaxX);
			position.z = Mathf.Clamp(position.z, positionMinZ, positionMaxZ);

			float z = Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed;

			zoom += z;
			zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);

			transform.position = position + transform.forward * zoom;

			if (Input.GetKey(KeyCode.Q))
			{
				transform.Rotate(Vector3.right * 15f * Time.deltaTime);
			}

			if (Input.GetKey(KeyCode.E))
			{
				transform.Rotate(-Vector3.right * 15f * Time.deltaTime);
			}


		}
	}
}