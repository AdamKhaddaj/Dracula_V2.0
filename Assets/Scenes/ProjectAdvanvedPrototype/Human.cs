using System.Collections;

using UnityEngine;

public class Human : MonoBehaviour {
	private static Human leader;

	[SerializeField] private bool isLeader = false;

	private bool moving;

	private Animator animator;
	private new Rigidbody rigidbody;

	private void Awake() {
		if (isLeader) {
			// assign the leader of flock
			leader = this;
		}

		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody>();
	}

	private void Update() {
		// leader control
		if (leader == this && !moving) {
			if (Input.GetMouseButtonDown(0)) {
				animator.SetBool("spin", false);

				// walk move
				Move(false);
			} else if (Input.GetMouseButtonDown(1)) {
				animator.SetBool("spin", false);

				// run move
				Move(true);
			} else if (Input.GetKeyDown(KeyCode.Space)) {
				animator.SetBool("spin", !animator.GetBool("spin"));
			}
		} else if (leader != this) {
			// flock control
			if (Vector3.Distance(transform.position, leader.transform.position) > 15.0f) {
				rigidbody.velocity = (leader.transform.position - transform.position).normalized * 5.0f;

				animator.SetBool("isWalking", true);
				animator.SetBool("isRunning", false);
			} else {
				rigidbody.velocity = Vector3.zero;

				animator.SetBool("isWalking", false);
				animator.SetBool("isRunning", false);
			}

			transform.forward = (leader.transform.position - transform.position).normalized;
		}
	}

	private void Move(bool running) {
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
			if (hit.collider != null && hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
				// spline movement
				StartCoroutine(IMove(running, hit.point));
			}
		}
	}

	private IEnumerator IMove(bool running, Vector3 destination) {
		moving = true;

		rigidbody.constraints = RigidbodyConstraints.FreezePosition;

		// animator
		if (running) {
			animator.SetBool("isWalking", false);
			animator.SetBool("isRunning", true);
		} else {
			animator.SetBool("isWalking", true);
			animator.SetBool("isRunning", false);
		}

		Vector3 p1 = transform.position;
		Vector3 p2 = destination;

		// flatten curve
		p2.y = transform.position.y;

		float time = Vector3.Distance(p1, p2) * (running ? 0.08f : 0.2f);

		Vector3 d1 = transform.forward * time * 1.5f;
		Vector3 d2 = (p2 - p1).normalized * time;

		Vector3 prev = p1;

		for (float t = 0; t < time; t += Time.deltaTime) {
			float x = t / time;

			// spline calculations
			Vector3 a = p1 * (2 * Mathf.Pow(x, 3) - 3 * Mathf.Pow(x, 2) + 1);
			Vector3 b = d1 * (Mathf.Pow(x, 3) - 2 * Mathf.Pow(x, 2) + x);
			Vector3 c = p2 * (-2 * Mathf.Pow(x, 3) + 3 * Mathf.Pow(x, 2));
			Vector3 d = d2 * (Mathf.Pow(x, 3) - Mathf.Pow(x, 2));

			transform.position = a + b + c + d;
			transform.forward = (transform.position - prev).normalized;

			// stop animation slightly early
			if (x > 0.95f) {
				animator.SetBool("isWalking", false);
				animator.SetBool("isRunning", false);
			}

			yield return null;
		}

		moving = false;

		rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}
}