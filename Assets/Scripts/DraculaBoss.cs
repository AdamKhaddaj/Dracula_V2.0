using System.Collections.Generic;

using UnityEngine;

public class DraculaBoss : MonoBehaviour {
	private LineRenderer lineRenderer;
	private Dictionary<int, PlayerUnit> targets;
	private List<int> toRemove;

	private void Awake() {
		lineRenderer = GetComponent<LineRenderer>();
		targets = new Dictionary<int, PlayerUnit>();
		toRemove = new List<int>();

		InvokeRepeating(nameof(Attack), 1f, 3f);
	}

	private void Attack() {
		lineRenderer.positionCount = 0;

		List<Vector3> positions = new List<Vector3>();

		foreach (int key in targets.Keys) {
			if (targets[key]) {
				positions.Add(targets[key].transform.position);
				targets[key].RemoveHealth(8);
			} else {
				toRemove.Add(key);
			}
		}

		lineRenderer.positionCount = positions.Count * 2;

		int j = 0;

		for (int i = 0; i < lineRenderer.positionCount;) {
			lineRenderer.SetPosition(i++, transform.position);
			lineRenderer.SetPosition(i++, positions[j++]);
		}

		for (int i = 0; i < toRemove.Count; i++) {
			targets.Remove(toRemove[i]);
		}

		toRemove.Clear();
	}

	private void OnTriggerEnter(Collider other) {
		PlayerUnit player = other.GetComponent<PlayerUnit>();

		if (player && targets.Count < 3) {
			if (!targets.ContainsKey(player.GetID())) {
				targets.Add(player.GetID(), player);
			}
		}
	}

	private void OnTriggerStay(Collider other) {
		PlayerUnit player = other.GetComponent<PlayerUnit>();

		if (player && targets.Count < 3) {
			if (!targets.ContainsKey(player.GetID())) {
				targets.Add(player.GetID(), player);
			}
		}
	}

	private void OnTriggerExit(Collider other) {
		PlayerUnit player = other.GetComponent<PlayerUnit>();

		if (player) {
			foreach (int key in targets.Keys) {
				if (player.GetID() == key) {
					toRemove.Add(key);
				}
			}
		}
	}
}