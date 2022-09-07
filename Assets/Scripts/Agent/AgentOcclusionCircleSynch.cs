using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>AgentOcclusionCircleSynch</c> is a helping class used for the wall and door shader 
///  to realize the transparent holes in the walls and doors when they are occluding an agent.
///  This script is assigned to the Robo3 of the Agent prefab.
///  It would be nice to find a way to generate the shader properties like <c>Shader.PropertyToID("_agent1Pos"); </c> 
///  dynamically per agent (4 agent positions are now hardcoded). Once dynamic creation of shader properties is possible,
///  this class should also be dynamic. 
///  This video served as inspiration: https://www.youtube.com/watch?v=S5gdvibmsV0 </summary>
public class AgentOcclusionCircleSynch : MonoBehaviour
{
    public Material WallMaterial;
    public Material DoorMaterial;

    // IDs for agentPositions and sizes taken from the shader properties
    //TODO find a way to create shader properties dynamically!!
    public static int Pos1ID = Shader.PropertyToID("_agent1Pos");
    public static int Pos2ID = Shader.PropertyToID("_agent2Pos");
    public static int Pos3ID = Shader.PropertyToID("_agent3Pos");
    public static int Pos4ID = Shader.PropertyToID("_agent4Pos");

    public static int size1ID = Shader.PropertyToID("_size1");
    public static int size2ID = Shader.PropertyToID("_size2");
    public static int size3ID = Shader.PropertyToID("_size3");
    public static int size4ID = Shader.PropertyToID("_size4");

    int posID;
    int sizeID;



    void Start()
    {

        if (transform.parent.name.Equals("Agent[0]"))
        {
            posID = Pos1ID;
            sizeID = size1ID;
        }
        else if (transform.parent.name.Equals("Agent[1]"))
        {
            posID = Pos2ID;
            sizeID = size2ID;
        }
        else if (transform.parent.name.Equals("Agent[2]"))
        {
            posID = Pos3ID;
            sizeID = size3ID;
        }
        else if (transform.parent.name.Equals("Agent[3]"))
        {
            posID = Pos4ID;
            sizeID = size4ID;
        }


    }

    void Update()
    {
        var view1 = Camera.main.WorldToViewportPoint(transform.position);
        WallMaterial.SetVector(posID, view1);
        DoorMaterial.SetVector(posID, view1);


        int layerMask = 1 << 10;

        RaycastHit hit;
        var ray1 = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        var cameraTarget = Camera.main.transform.position;


        if (Physics.Raycast(ray1, cameraTarget, out hit, Mathf.Infinity, layerMask))
        {

            Debug.DrawLine(transform.position, hit.point);


            if (hit.collider != null)
            {
                if (hit.collider.tag == "Wall")
                {
                    WallMaterial.SetFloat(sizeID, 0.7f);
                }
                if (hit.collider.tag == "Door")
                {
                    DoorMaterial.SetFloat(sizeID, 0.7f);
                }
            }
        }
        else
        {
            WallMaterial.SetFloat(sizeID, 0);
            DoorMaterial.SetFloat(sizeID, 0);
        }


    }
}
