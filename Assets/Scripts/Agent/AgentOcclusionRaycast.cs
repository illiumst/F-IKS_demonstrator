using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentOcclusionRaycast : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 10;
    
        RaycastHit hit;
        var rayTarget = GameObject.Find("CameraTarget").transform.position;
        var ray1 = new Vector3(transform.position.x, transform.position.y-1, transform.position.z);
        var cameraTarget = Camera.main.transform.position;
        //Debug.DrawLine(ray1, cameraTarget);


        if (Physics.Raycast(ray1, rayTarget, out hit,  Mathf.Infinity, layerMask))
            //Debug.DrawLine(transform.position, rayTarget);
            Debug.DrawLine(transform.position, hit.point);


            if(hit.collider!=null){
                if (hit.collider.tag == "Wall"){
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
