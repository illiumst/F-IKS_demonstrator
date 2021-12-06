using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AgentController : MonoBehaviour
{

    public Vector3 currentpos;
    public Vector3 goalPosition;

    [SerializeField] float playBackSpeed;

    public float speed;

    GameObject system;

    public float sliderValue;

    Button playButton;
    Button pauseButton;


    public bool playingSequence;

    bool manualRobotControl;

    public Animator animator;

    Agent parentAgent;

    // Start is called before the first frame update
    void Start()
    {
        currentpos = this.transform.position;
        goalPosition = currentpos;
        sliderValue = 0;

        system = GameObject.FindWithTag("System");

        playingSequence = false;

        animator = GetComponent<Animator>();

        manualRobotControl = false;
    }

    // Update is called once per frame
    void Update()
    {
        currentpos = this.transform.position;

        playBackSpeed = system.GetComponent<EnvironmentStateMachine>().playBackSpeed;

        //sequence with playbutton is being played
        if (playingSequence)
        {
            speed = playBackSpeed * 3;
            MoveRobotTo(goalPosition);
        }

        if (!playingSequence && currentpos != goalPosition && goalPosition != null)
        {
            MoveRobotTo(goalPosition);
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

    public void MoveRobotTo(Vector3 position)
    {
        animator.SetBool("moving", true);
        animator.SetFloat("speed", speed);

        this.transform.position = Vector3.MoveTowards(transform.position, goalPosition, Time.deltaTime * speed);
        Vector3 difference = goalPosition - currentpos;
        var name = this.transform.GetChild(4);
        if (difference != Vector3.zero)
        {
            this.transform.forward = difference;
            name.transform.eulerAngles = Vector3.zero;
        }
    }

    public Vector3 GetRecalculatedPosition(float x, float y, float z)
    {
        Vector3 center = system.GetComponent<EnvironmentStateMachine>().environmentCenter;
        return new Vector3(x - center.x, y, z - center.z);
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
        var contentActionText = data.transform.GetChild(1).gameObject;
        contentActionText.GetComponent<Text>().text = action;
        var contentValidityText = data.transform.GetChild(2).gameObject;
        string validityString = "invalid";
        var color = Color.red;
        var textColor = Color.red;
        if (valid)
        {
            textColor = new Color32(0, 160, 20, 255);
            color = Color.green;
            validityString = "valid";
        }
        contentValidityText.GetComponent<Text>().text = validityString;
        contentValidityText.GetComponent<Text>().color = textColor;

        var validityLight = agentBody.transform.GetChild(5).gameObject;
        validityLight.GetComponent<Light>().color = color;

        var canvas = agentBody.transform.GetChild(4).gameObject;
        var nameTag = canvas.transform.GetChild(0).gameObject;
        nameTag.GetComponent<TextMeshProUGUI>().color = textColor;
    }


}
