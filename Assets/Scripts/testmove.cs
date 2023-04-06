using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testmove : MonoBehaviour {
	public EnemyGuard e;
	Vector3 leftpoint, rightpoint;

	void Start() {
		e.animator.SetBool("isRunning", true);
		e.animator.SetBool("isWalking", false);
		e.animator.SetBool("spin", false);

		leftpoint = transform.position + new Vector3(30f, 0, 0);
		rightpoint = transform.position + new Vector3(-30f, 0, 0);

		Invoke("move1", 0f);
	}


	void move1() {
		e.agent.SetDestination(leftpoint);
		Invoke("move2", 4f);
	}

	void move2() {
		e.agent.SetDestination(rightpoint);
		Invoke("move1", 4f);
	}



}
