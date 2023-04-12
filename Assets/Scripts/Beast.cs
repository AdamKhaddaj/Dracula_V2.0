using UnityEngine;
using UnityEngine.AI;

public class Beast : MonoBehaviour {
	private Animator animator;
	private NavMeshAgent agent;

	private Vector3[] points;
	private int point = 0;

	void Start() {
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();

		points = new Vector3[6];
		points[0] = new Vector3(-54.6f, 2.4f, -43.34132f);
		points[1] = new Vector3(-61f, 2.4f, -127f);
		points[2] = new Vector3(56f, 2.4f, -130.6f);
		points[3] = new Vector3(56f, 2.4f, -100.6f);
		points[4] = new Vector3(-61f, 2.4f, -127f);
		points[5] = new Vector3(-54.6f, 2.4f, -43.34132f);

		agent.SetDestination(points[0]);
	}

	// Update is called once per frame
	void Update() {
		if (Vector3.Distance(transform.position, points[point]) < 5f) {
			point++;
			point %= points.Length;
			agent.SetDestination(points[point]);
		}
	}

    private void OnTriggerEnter(Collider collision)
    {
		if(collision.gameObject.name == "EventSystem")
        {
			return;
        }
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy") || collision.gameObject.layer == LayerMask.NameToLayer("DynamicPlayerUnits") || collision.gameObject.layer == LayerMask.NameToLayer("StaticPlayerUnits"))
		{

			collision.gameObject.GetComponent<Unit>().RemoveHealth(100);

        }
    }

}