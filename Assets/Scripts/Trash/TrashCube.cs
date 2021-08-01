using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCube : MonoBehaviour, IPooledObject
{

    public float upForce = 1f;
    public float sideForce = .1f;

    public float xVar = 0.2f;
    public float zVar = 0.2f;


    // Start is called before the first frame update
    public void OnObjectSpawn()
    {
        /*float xForce = Random.Range(-sideForce, sideForce);
        float yForce = Random.Range(upForce / 2f, upForce);
        float zForce = Random.Range(-sideForce, sideForce);

        Vector3 force = new Vector3(xForce, yForce, zForce);

        GetComponent<Rigidbody>().velocity = force;*/

        /*float xTrans = Random.Range(0, xVar);
        float zTrans = Random.Range(0, zVar);

        this.transform.position = new Vector3(this.transform.position.x + xTrans, 0, this.transform.position.z + zTrans);*/

    }

    // Update is called once per frame
    void Update()
    {

    }
}
