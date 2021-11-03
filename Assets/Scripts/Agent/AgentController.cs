using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AgentController : MonoBehaviour
{

    public Vector3 currentpos;
    public Vector3 goalPosition;
    public Vector3 endGoalPosition;
    public Vector3 lastReachedGoalPosition;
    public int lastReachedGoalPositionIndex;

    [SerializeField] float playBackSpeed;

    [SerializeField] float speed;

    GameObject system;
    Text positionText;

    public int currentArrayPosOnSlider = 0;

    public int timeStep;

    public float sliderValue;

    bool isCleaning = false;
    bool collision;

    public bool stopMoving = false;

    Button playButton;
    Button pauseButton;


    public bool playingSequence;

    bool manualRobotControl;

    Toggle robotControlToggle;

    float distance;

    public Canvas canvas;

    Animator animator;

    Agent parentAgent;

    // Start is called before the first frame update
    void Start()
    {
        currentpos = this.transform.position;
        endGoalPosition = currentpos;
        lastReachedGoalPosition = currentpos;
        lastReachedGoalPositionIndex = 0;
        sliderValue = 0;

        system = GameObject.FindWithTag("System");
        robotControlToggle = GameObject.FindWithTag("RobotControlToggle").GetComponent<Toggle>();

        playingSequence = false;
        distance = 0f;

        animator = GetComponent<Animator>();

        manualRobotControl = false;
    }

    // Update is called once per frame
    void Update()
    {
        manualRobotControl = robotControlToggle.isOn;
        //manualRobotControl = false;
        collision = this.GetComponent<AgentCollision>().GetCollision();
        currentpos = this.transform.position;

        if(playingSequence){
            Debug.Log("Trying to move robot in sequence");
            MoveRobotTo(endGoalPosition);
        }

        if (!stopMoving && !manualRobotControl && !playingSequence)
        {

            if (currentpos != goalPosition && goalPosition != null && !isCleaning && !collision)
            {
                MoveRobotTo(goalPosition);
            }

            if (currentpos == goalPosition && goalPosition != endGoalPosition)
            {
                Debug.Log("---------------------Reached Endgoal!");
                float diff = sliderValue - lastReachedGoalPositionIndex;
                if (diff > 0)
                {
                    //var index = positions.IndexOf(goalPosition) + 1;
                    //goalPosition = positions[index];
                }
                else
                {
                    //var index = positions.IndexOf(goalPosition) - 1;
                    //goalPosition = positions[index];
                }
            }
            if (currentpos == endGoalPosition)
            {
                lastReachedGoalPosition = endGoalPosition;
                //lastReachedGoalPositionIndex = positions.IndexOf(endGoalPosition);
            }

            if (collision)
            {
                BounceOff();
            }
        }

        if (manualRobotControl)
        {
            if (Input.GetKey("left"))
            {
                MoveRobotLeft();
            }
            if (Input.GetKey("right"))
            {
                MoveRobotRight();
            }
            if (Input.GetKey("up"))
            {
                MoveRobotUp();
            }
            if (Input.GetKey("down"))
            {
                MoveRobotDown();
            }
        }

        if (isCleaning)
        {
            stopMoving = true;
            //clean --> call in different script
        }
        if(stopMoving){
            Debug.Log("!!!!!!!!!!!!!!!!!!!!! Agent stopped moving !!!!!!!!!!!!!!!!!!!!!");
        }
    }

    void MoveRobotLeft()
    {
        Vector3 position = this.transform.position;
        position.x -= Time.deltaTime * speed;
        this.transform.position = position;
    }
    void MoveRobotRight()
    {
        Vector3 position = this.transform.position;
        position.x += Time.deltaTime * speed;
        this.transform.position = position;
    }
    void MoveRobotUp()
    {
        Vector3 position = this.transform.position;
        position.z += Time.deltaTime * speed;
        this.transform.position = position;
    }
    void MoveRobotDown()
    {
        Vector3 position = this.transform.position;
        position.z -= Time.deltaTime * speed;
        this.transform.position = position;
    }

    public void UpdateGoalPositionAccordingToSlider()
    {
        //TODO does not work yet when slider moved to fast
        stopMoving = false;
        float diff = sliderValue - lastReachedGoalPositionIndex;
        if (playingSequence)
        {
            speed = playBackSpeed;
        }
        else
        {
            speed = speed * Mathf.Abs(diff);
        }

        currentArrayPosOnSlider = (int)sliderValue;
        //endGoalPosition = positions[currentArrayPosOnSlider];

        if (diff > 1)
        {
            //goalPosition = positions[lastReachedGoalPositionIndex + 1];
        }
        if (diff == 1 || diff == -1)
        {
            goalPosition = endGoalPosition;
        }
        //TODO not yet working; check if end of positions list reached
        if (diff < 0)
        {
            //goalPosition = positions[lastReachedGoalPositionIndex - 1];
        }
        distance = Vector3.Distance(currentpos, goalPosition);

    }


    public void MoveRobotTo(Vector3 position)
    {
        animator.SetBool("moving", true);
        animator.SetFloat("speed", speed);

        this.transform.position = Vector3.MoveTowards(transform.position, goalPosition, Time.deltaTime * speed);
        Vector3 difference = goalPosition - currentpos;
        this.transform.forward = difference;
    }

    void BounceOff()
    {
        Debug.Log("-------------- Bouncing off...");
        this.GetComponent<AgentCollision>().SetCollision(false);
        if (goalPosition == endGoalPosition)
        {
            goalPosition = transform.position;
        }
        else
        {
            //var index = positions.IndexOf(goalPosition) + 1;
            //goalPosition = positions[index];
        }
    }

    public Vector3 GetRecalculatedPosition(float x, float y, float z)
    {
        Vector3 center = system.GetComponent<EnvironmentStateMachine>().environmentCenter;
        return new Vector3(x - center.x, y, z - center.z);
    }

    public void setPositionText(Text text)
    {
        this.positionText = text;
    }

    public void UpdateAgentListItems(GameObject agentObject, GameObject listItem, int x, int y, string name, string action, bool valid)
    {
        var agentBody = agentObject.transform.GetChild(0).gameObject;
        var itemHeader = listItem.transform.GetChild(0).gameObject;
        var itemHeaderText = itemHeader.transform.GetChild(1).gameObject;
        itemHeaderText.GetComponent<Text>().text = name;

        var itemContent = listItem.transform.GetChild(1).gameObject;
        var data = itemContent.transform.GetChild(1).gameObject;
        var contentPositionText = data.transform.GetChild(0).gameObject;
        contentPositionText.GetComponent<Text>().text = "x: " + x + " y: " + y;
        var contentActionText =data.transform.GetChild(1).gameObject;
        contentActionText.GetComponent<Text>().text = action;
        var contentValidityText = data.transform.GetChild(2).gameObject;
        string validityString = "invalid";
        var color = Color.red;
        if (valid)
        {
            color = Color.green;
            validityString = "valid";
        }
        contentValidityText.GetComponent<Text>().text = validityString;
        contentValidityText.GetComponent<Text>().color = color;

        var validityLight = agentBody.transform.GetChild(5).gameObject;
        validityLight.GetComponent<Light>().color = color;

        var canvas = agentBody.transform.GetChild(4).gameObject;
        var nameTag = canvas.transform.GetChild(0).gameObject;
        nameTag.GetComponent<TextMeshProUGUI>().color = color;
    }


}
