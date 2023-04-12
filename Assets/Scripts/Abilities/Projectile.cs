using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private Vector3 target, start;
    public AnimationCurve curve;

    private float time;


    private void Start()
    {
        start = transform.position;
    }

    public void SetTarget(Vector3 s, Vector3 end)
    {
        start = s;
        target = end;
    }

    // Update is called once per frame
    void Update()
    {

        time += Time.deltaTime;

        if (Vector3.Distance(transform.position, target) < 1f)
        {
            int layer_mask = LayerMask.GetMask("Enemy");
            Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 6f, layer_mask);

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].GetComponent<Unit>().RemoveHealth(8);
            }
            Destroy(gameObject);
        }
        else
        {
            Vector3 pos = Vector3.Lerp(start, target, time*0.8f);
            pos.y += 5*curve.Evaluate(time*0.8f);
            transform.position = pos;
            
        }

    }
}
