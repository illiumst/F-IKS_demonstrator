using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorRenderer : MonoBehaviour
{
    private void Update()
    {
        GameObject robot = this.transform.parent.gameObject.transform.GetChild(0).gameObject;
        this.transform.position = robot.transform.position;
    }
    void OnTriggerEnter(Collider collider)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collider.tag == "Door")
        {
            //If the GameObject's name matches the one you suggest, output this message in the console
            Debug.Log("________Robot Field of View hit door");
            MakeDoorTransparent(collider.gameObject);
        }

    }
    void OnTriggerExit(Collider collider)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collider.tag == "Door")
        {
            //If the GameObject's name matches the one you suggest, output this message in the console
            //Debug.Log("________Robot Field of View exited wall");
            MakeDoorSolid(collider.gameObject);
        }

    }
    public void MakeDoorTransparent(GameObject door)
    {
        var material = Resources.Load("Materials/DoorMaterialTransparent", typeof(Material)) as Material;
        door.GetComponent<MeshRenderer>().material = material;
        foreach (MeshRenderer rend in door.GetComponentsInChildren<MeshRenderer>())
        {
            rend.material = material;
        }

    }

    public void MakeDoorSolid(GameObject door)
    {
        var material = Resources.Load("Materials/DoorMaterial", typeof(Material)) as Material;
        door.GetComponent<MeshRenderer>().material = material;

        foreach (MeshRenderer rend in door.GetComponentsInChildren<MeshRenderer>())
        {
            rend.material = material;
        }

    }
}