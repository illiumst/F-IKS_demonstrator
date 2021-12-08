using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class WallRenderer : MonoBehaviour
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
            //TODO: dont make transparent when max x or max y
            var spawner = GameObject.FindWithTag("System").GetComponent<ObjectSpawner>();
            if (this.gameObject.transform.position.x != spawner.GetMaxWallX() && this.gameObject.transform.position.y != spawner.GetMaxWallY())
            {
                MakeWallTransparent(this.gameObject);
            }
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
                //Check for a match with the specified name on any GameObject that collides with your GameObject  
                MakeWallSolid(this.gameObject);
            }
        }

    }
    public void MakeWallTransparent(GameObject wall)
    {
        var material = Resources.Load("Materials/WallMaterialTransparent", typeof(Material)) as Material;
        foreach (MeshRenderer rend in wall.GetComponentsInChildren<MeshRenderer>())
        {
            rend.material = material;
        }

    }

    public void MakeWallSolid(GameObject wall)
    {
        var material = Resources.Load("Materials/WallMaterial", typeof(Material)) as Material;
        foreach (MeshRenderer rend in wall.GetComponentsInChildren<MeshRenderer>())
        {
            rend.material = material;
        }

    }

}
