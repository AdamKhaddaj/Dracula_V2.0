using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PylonModel : MonoBehaviour
{

    public GameObject prefab;
    public PlayerPylon parentpylon;

    private Color origcolor;

    bool canbuild;

    void Start()
    {
        //putting this in cause it kinda bugs out on the frame it first spawns
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                transform.position = hit.point;
            }
        }
        origcolor = transform.GetChild(0).GetComponent<Renderer>().material.color;
        canbuild = false;
    }

    public void SetParentPylon(PlayerPylon p)
    {
        parentpylon = p;
    }

    // Update is called once per frame
    void Update()
    {

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                transform.position = hit.point;
            }

            if (Vector3.Distance(hit.point, parentpylon.transform.position) > 12f)
            {
                transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
                transform.GetChild(1).GetChild(0).GetComponent<Renderer>().material.color = Color.red;
                canbuild = false;
            }
            else
            {
                transform.GetChild(0).GetComponent<Renderer>().material.color = origcolor;
                transform.GetChild(1).GetChild(0).GetComponent<Renderer>().material.color = origcolor;
                canbuild = true;
            }
        }

        if (Input.GetMouseButtonUp(0) && canbuild)
        {
            Instantiate(prefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }

    }
}
