using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentOcclusionCircleSynch : MonoBehaviour
{
    public Material WallMaterial;

    public static int PosID = Shader.PropertyToID("_agentPos");
    public static int sizeID = Shader.PropertyToID("_size");

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //var dir = camera.transform.position -transform.position;
        //var ray = new Ray(transform.position, dir.normalized);
        var view = Camera.main.WorldToViewportPoint(transform.position);
        WallMaterial.SetVector(PosID, view);


        int layerMask = 1 << 10;
    
        RaycastHit hit;
        //var rayTarget = GameObject.Find("CameraTarget").transform.position;
        var ray1 = new Vector3(transform.position.x, transform.position.y-1, transform.position.z);
        var cameraTarget = Camera.main.transform.position;
        //Debug.DrawLine(ray1, cameraTarget);


        if (Physics.Raycast(ray1, cameraTarget, out hit,  Mathf.Infinity, layerMask)){

            //Debug.DrawLine(transform.position, rayTarget);
            Debug.DrawLine(transform.position, hit.point);


            if(hit.collider!=null){
                if (hit.collider.tag == "Wall"){
                    WallMaterial.SetFloat(sizeID, 1);
                }
            } 
        }
        else{
                WallMaterial.SetFloat(sizeID, 0);
        }
    }
}
