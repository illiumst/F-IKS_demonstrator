using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//========================================================================================================//
//================================= DEPRECATED CLASS =====================================================//
//========================================================================================================//

/// <summary>Class <c>AgentOcclusionRaycast</c> is a deprecated and unused class that was the old approach for
///  creating transparent walls by changing the material of an entire wallpiece. </summary>
public class AgentOcclusionRaycast : MonoBehaviour
{
    void FixedUpdate()
    {
        int layerMask = 1 << 10;

        RaycastHit hit;
        var rayTarget = GameObject.Find("CameraTarget").transform.position;
        var ray1 = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        var cameraTarget = Camera.main.transform.position;


        if (Physics.Raycast(ray1, rayTarget, out hit, Mathf.Infinity, layerMask))
            Debug.DrawLine(transform.position, hit.point);


        if (hit.collider != null)
        {
            if (hit.collider.tag == "Wall")
            {
                MakeWallTransparent(hit.collider.gameObject);
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
