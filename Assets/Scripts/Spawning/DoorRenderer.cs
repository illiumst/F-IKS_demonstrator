using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorRenderer : MonoBehaviour
{
    public List<Collider> TriggerList = new List<Collider>();

    private void Update()
    {

    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Agent")
        {
            if (!TriggerList.Contains(collider))
            {
                TriggerList.Add(collider);
            }
            MakeDoorTransparent(this.gameObject);
        }


    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Agent")
        {
            if (TriggerList.Contains(collider))
            {
                TriggerList.Remove(collider);
            }
            if (TriggerList.Count == 0)
            {
                MakeDoorSolid(this.gameObject);
            }
        }

    }
    public void MakeDoorTransparent(GameObject door)
    {
        var material = Resources.Load("Materials/DoorMaterialTransparent", typeof(Material)) as Material;
        foreach (MeshRenderer rend in door.GetComponentsInChildren<MeshRenderer>())
        {
            rend.material = material;
        }

    }

    public void MakeDoorSolid(GameObject door)
    {
        var material = Resources.Load("Materials/DoorMaterial", typeof(Material)) as Material;
        foreach (MeshRenderer rend in door.GetComponentsInChildren<MeshRenderer>())
        {
            rend.material = material;
        }

    }
}
