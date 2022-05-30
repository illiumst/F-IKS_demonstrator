using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    public float animSpeed = 1;
    float animDuration = 1;
    Animator animator;

    public float speed;
    Vector3 goalPosition;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("animSpeed", animSpeed);
    }

    public void MoveRobotNew()
    {
        //SetAnimDuration("Action[NORTH]");
        StartCoroutine(moveRobot());
    }

    IEnumerator moveRobot()
    {
        Debug.Log("Start");
        animator.SetBool("moving", true);
        yield return StartCoroutine(WaitFor.Frames(120)); // wait for 120 frames
        //animator.SetBool("moving", false);
        Debug.Log("Hello.........");
    }

    public void UpdateGoalPosition()
    {
        goalPosition = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
    }

    void SetAnimDuration(string action)
    {
        if (action == "Action[NORTH]" | action == "Action[NORTHEAST]" | action == "Action[NORTHWEST]" | action == "Action[EAST]" | action == "Action[SOUTHEAST]"
        | action == "Action[SOUTH]" | action == "Action[SOUTHWEST]" | action == "Action[WEST]")
        {
            animDuration = 120 * animSpeed;
        }
        if (action == "Action[CLEAN_UP]")
        {
            animDuration = 120 * animSpeed;
        }
    }
}

