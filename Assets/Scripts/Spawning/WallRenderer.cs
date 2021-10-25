using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallRenderer : MonoBehaviour
{
    private void Update()
    {
        GameObject robot = this.transform.parent.gameObject.transform.GetChild(0).gameObject;
        this.transform.position = robot.transform.position;
    }
    void OnTriggerEnter(Collider collider)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collider.tag == "Wall")
        {
            //If the GameObject's name matches the one you suggest, output this message in the console
            Debug.Log("________Robot Field of View hit wall");
            MakeWallTransparent(collider.gameObject);
        }

    }
    void OnTriggerExit(Collider collider)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collider.tag == "Wall")
        {
            //If the GameObject's name matches the one you suggest, output this message in the console
            Debug.Log("________Robot Field of View exited wall");
            MakeWallSolid(collider.gameObject);
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