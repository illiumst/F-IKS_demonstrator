using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;


/// <summary>Class <c>AgentController</c> is responsible for handling the behavior of Agent objects in the scene 
/// like movement, action animations and displaying the little displays over each agent. 
/// This script is assigned to the Robo3 of the Agent prefab.</summary>
public class AgentController : MonoBehaviour
{

    #region Attribute declarations

    //========================================================================================================//
    //================================= GLOBAL OBJECTS =======================================================//
    //========================================================================================================//

    GameObject system;
    public Animator animator;
    Agent agentModel;

    //========================================================================================================//
    //================================= UI ELEMENTS ==========================================================//
    //========================================================================================================//

    GameObject agentCanvas;
    public GameObject speechBubble;
    GameObject speechBubbleContent;
    GameObject speechBubbleInvalidityImage;
    GameObject speechBubbleDirectionText;
    GameObject speechBubbleAgentName;
    public GameObject speechBubblePositionText;
    Texture2D cleaningTex;
    Texture2D itemPickTex;
    Texture2D directionTex;
    Sprite cleaningSprite;
    Sprite itemPickSprite;
    Sprite directionSprite;

    //========================================================================================================//
    //================================= HELPING FIELDS =======================================================//
    //========================================================================================================//

    public Vector3 currentpos;
    public Vector3 goalPosition;
    public float playBackSpeed;
    public float bufferAcceleration = 1f;
    public float sliderValue;
    public bool playingSequence;
    public bool buffering;
    public bool valid;
    public bool backwardBuffer;
    public bool finished = false;
    public bool batteryBlink = false;
    public string action;
    float currentActionLength = 0;
    string currentAnimationState = "";

    #endregion

    void Start()
    {
        #region Initilizations

        // initializing system object
        system = GameObject.FindWithTag("System");

        // initializing animator object
        animator = GetComponent<Animator>();

        // initializing helping fields
        currentpos = this.transform.position;
        goalPosition = currentpos;
        sliderValue = 0;
        valid = true;
        backwardBuffer = false;
        playingSequence = false;

        // initializing objects from scene
        agentCanvas = FindGameObjectInChildWithTag(gameObject, "AgentCanvas");
        speechBubble = FindGameObjectInChildWithTag(agentCanvas, "SpeechBubble");
        speechBubbleContent = FindGameObjectInChildWithTag(speechBubble, "SpeechBubbleContent");
        speechBubbleInvalidityImage = FindGameObjectInChildWithTag(speechBubble, "SpeechBubbleInvalid");
        speechBubbleInvalidityImage.SetActive(false);
        speechBubbleDirectionText = FindGameObjectInChildWithTag(speechBubble, "SpeechBubbleDirectionText");
        speechBubblePositionText = FindGameObjectInChildWithTag(speechBubble, "SpeechBubblePositiontext");
        speechBubbleAgentName = FindGameObjectInChildWithTag(speechBubble, "SpeechBubbleAgentNumber");
        SetAgentNameInSpeechBubble(this.transform.parent.gameObject.name);

        // loading sprite resources as textures (speech bubble images)
        cleaningTex = Resources.Load<Texture2D>("Sprites/cleaningNew");
        itemPickTex = Resources.Load<Texture2D>("Sprites/boxUp");
        directionTex = Resources.Load<Texture2D>("Sprites/direction");

        // creating sprites from textures
        cleaningSprite = Sprite.Create(cleaningTex, new Rect(0.0f, 0.0f, cleaningTex.width, cleaningTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        itemPickSprite = Sprite.Create(itemPickTex, new Rect(0.0f, 0.0f, itemPickTex.width, itemPickTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        directionSprite = Sprite.Create(directionTex, new Rect(0.0f, 0.0f, directionTex.width, directionTex.height), new Vector2(0.5f, 0.5f), 100.0f);

        // initially deactivating speech bubble
        speechBubble.SetActive(false);

        #endregion
    }

    #region Update

    /// <summary> Central Update method. Update position, playback speed. Setting the animator's "animSpeed" valuable to playback speed. 
    /// Setting buffering bool according buffering value of the stateMAnager. Calling movement method <c> MoveRobotTo(position)</c>.  </summary>

    void Update()
    {
        currentpos = this.transform.position;
        var stateManager = system.GetComponent<EnvironmentStateManager>();

        playBackSpeed = stateManager.playBackSpeed * bufferAcceleration;
        animator.SetFloat("animSpeed", playBackSpeed);
        buffering = stateManager.buffering;

        //sequence with playbutton is being played
        if (playingSequence && valid)
        {
            MoveRobotTo(goalPosition);
        }

        //buffering? no
        if (!playingSequence && currentpos != goalPosition && goalPosition != null && valid)
        {
            MoveRobotTo(goalPosition);
        }

        if (!playingSequence && !buffering)
        {
            ChangeAnimationState(AgentConstants.ANIMATION_IDLE);
            speechBubble.SetActive(false);
        }
        if (batteryBlink)
        {
            this.transform.GetComponent<BatteryBlinkPulse>().enabled = true;
        }
    }
    #endregion

    #region ActionUpdate

    /// <summary>This method is called to update the agent's action. This encompasses updating the display and its content 
    /// and in a switch statement calling the function <c>ChangeAnimationState(state)</c>. </summary>
    /// 
    public void UpdateAgentAction()
    {
        var stateManager = system.GetComponent<EnvironmentStateManager>();
        UpdateSpeechBubble(agentModel.action, agentModel.valid);
        speechBubblePositionText.GetComponent<Text>().text = "x:" + agentModel.x + " y:" + agentModel.y;
        action = agentModel.action;
        finished = false;

        if (!agentModel.valid)
        {
            valid = false;
            stateManager.AddToInValidCounter(1);
            ChangeAnimationState(AgentConstants.ANIMATION_INVALID);
        }
        if (agentModel.valid)
        {
            valid = true;
            stateManager.AddToValidCounter(1);
            switch (agentModel.action)
            {
                case AgentConstants.ACTION_CLEAN_UP:
                    StartCoroutine(BubblesAction());
                    ChangeAnimationState(AgentConstants.ANIMATION_CLEANING); break;
                case AgentConstants.ACTION_ITEM:
                    ChangeAnimationState(AgentConstants.ANIMATION_PICKUP); break;
                case AgentConstants.ACTION_NORTH:
                case AgentConstants.ACTION_NORTHEAST:
                case AgentConstants.ACTION_NORTHWEST:
                case AgentConstants.ACTION_EAST:
                case AgentConstants.ACTION_SOUTHEAST:
                case AgentConstants.ACTION_SOUTH:
                case AgentConstants.ACTION_SOUTHWEST:
                case AgentConstants.ACTION_WEST:
                    ChangeAnimationState(AgentConstants.ANIMATION_MOVING); break;
                default: ChangeAnimationState(AgentConstants.ANIMATION_IDLE); break;
            }
        }
        else
        {
            Debug.Log(" No validity data");
        }
    }

    /// <summary>This method returns the length of an action. It is later used to wait for this amount of time 
    /// before proceeding to the next step to let an animation finish. </summary>
    /// <param name="action">string value of the action name (in form of an <c>AgentConstants</c> value. </param>

    float GetActionLength(string action)
    {
        switch (action)
        {
            case AgentConstants.ANIMATION_CLEANING:
                return 2f / playBackSpeed;
            case AgentConstants.ANIMATION_PICKUP:
                return 2f / playBackSpeed;
            case AgentConstants.ANIMATION_MOVING:
                return 0.3f / playBackSpeed;
            case AgentConstants.ANIMATION_INVALID:
                return 1f / playBackSpeed;
            default: return 0.5f / playBackSpeed;
        }
    }
    #endregion

    #region Bubbles

    /// <summary>This iEnumerator is used to spawnCleaning bubbles ofter wating for a couple of frames. </summary>
    IEnumerator BubblesAction()
    {
        var stateManager = system.GetComponent<EnvironmentStateManager>();
        yield return StartCoroutine(WaitFor.Frames((int)(10 / playBackSpeed)));
        stateManager.SpawnCleaningBubbles(goalPosition);
    }
    #endregion

    #region Position and Moving

    /// <summary>This method is called every frame in update after verifying if the agent is moving.  
    /// It transforms the agents position by calling the Vector3.MoveTowards() function towards the 
    /// global variable goalPosition over time multiplied by speed. Speed is a multiple of 3 of playBackSpeed.
    /// The agent is rotated to face its goalPosition.
    /// If the buffer is buffering backwards, the agent is rotated by 180 degrees. </summary>
    /// <param name="position">the goalPosition </param>

    public void MoveRobotTo(Vector3 position)
    {
        var speed = 3f * playBackSpeed;
        this.transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);

        if (backwardBuffer)
        {
            this.transform.Rotate(new Vector3(0, 180, 0), Space.Self);
        }
        Vector3 difference = goalPosition - currentpos;
        var name = this.transform.GetChild(4);
        if (difference != Vector3.zero)
        {
            this.transform.forward = difference;
            name.transform.eulerAngles = Vector3.zero;
        }
    }
    /// <summary>This method resets the transform's position. It also starts a coroutine to wait for the current animation
    /// to finish. </summary>
    /// <param name="pos"> the new position. </param>
    public void SetRobotPosition(Vector3 pos)
    {
        this.transform.position = pos;
        StartCoroutine(WaitToFinishAnimation(currentAnimationState));
    }
    /// <summary>This method changes the animation state. If the desired state matches the current state 
    /// a coroutine is started to wait for the animation to finish. </summary>
    /// <param name="state"> the string state name (in form of AgentConstant) </param>
    void ChangeAnimationState(string state)
    {
        finished = false;
        if (currentAnimationState == state)
        {
            StartCoroutine(WaitToFinishAnimation(state));
        }
        animator.Play(state);
        currentAnimationState = state;
        StartCoroutine(WaitToFinishAnimation(state));
    }

    /// <summary>This IEnumerator waits for the length of an action before setting the global finished state to true. </summary>
    /// <param name="action"> the string action name (in form of AgentConstant) </param>
    IEnumerator WaitToFinishAnimation(string action)
    {
        currentActionLength = GetActionLength(action);
        yield return new WaitForSeconds(currentActionLength);
        //Debug.Log("Action finished...");
        finished = true;
    }
    #endregion

    #region List

    /// <summary>This method updates the content in the agent info scrollview. </summary>
    /// <param name="agentObject"> the corresponding GameObject to the ist item. </param>
    /// <param name="listItem"> the corresponding listItem GameObject. </param>
    /// <param name="x"> agent's x coordinate </param>
    /// <param name="y"> agent's y coordinate </param>  
    /// <param name="name"> agent's name </param>
    /// <param name="action"> agent's name </param>
    /// <param name="valid"> boolean indicating action's validity </param>
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
        string validityString = AgentConstants.INVALID;
        var color = Color.red;
        var textColor = Color.red;
        if (valid)
        {
            textColor = new Color32(0, 160, 20, 255);
            color = Color.green;
            validityString = AgentConstants.VALID;
        }
        contentValidityText.GetComponent<Text>().text = validityString;
        contentValidityText.GetComponent<Text>().color = textColor;

        var canvas = agentBody.transform.GetChild(4).gameObject;
        var nameTag = canvas.transform.GetChild(0).gameObject;
    }
    #endregion

    #region SpeechBubble

    /// <summary>This method updates the content in the agent over head display/speech-bubble.
    /// Depending on the agent action it calls the method <c>SetSpeechBubbleContent(...)</c></summary>
    /// <param name="action"> the string action name (in form of AgentConstant) </param>
    /// <param name="valid"> boolean indicating action's validity </param>
    public void UpdateSpeechBubble(string action, bool valid)
    {
        var rectTrans = speechBubble.GetComponent<RectTransform>();
        var camera = Camera.main;
        rectTrans.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
        Vector3 contentRotation = rectTrans.rotation.eulerAngles;

        switch (action)
        {
            case AgentConstants.ACTION_CLEAN_UP:
                SetSpeechBubbleContent(cleaningSprite, contentRotation, "", 0, false); break;

            case AgentConstants.ACTION_ITEM:
                SetSpeechBubbleContent(itemPickSprite, contentRotation, "", 0, false); break;

            case AgentConstants.ACTION_NORTH:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_NORTH, 0, true); break;

            case AgentConstants.ACTION_NORTHEAST:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_NORTHEAST, 315, true); break;

            case AgentConstants.ACTION_EAST:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_EAST, 270, true); break;

            case AgentConstants.ACTION_SOUTHEAST:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_SOUTHEAST, 225, true); break;

            case AgentConstants.ACTION_SOUTH:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_SOUTH, 180, true); break;

            case AgentConstants.ACTION_SOUTHWEST:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_SOUTHWEST, 135, true); break;

            case AgentConstants.ACTION_WEST:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_WEST, 90, true); break;

            case AgentConstants.ACTION_NORTHWEST:
                SetSpeechBubbleContent(directionSprite, contentRotation, AgentConstants.BUBBLE_NORTHWEST, 45, true); break;

            default: speechBubble.SetActive(false); break;
        }
        if (valid)
        {
            speechBubble.GetComponent<Image>().color = new Color32(60, 255, 140, 255);

        }
        else if (!valid)
        {
            speechBubble.GetComponent<Image>().color = new Color32(255, 60, 80, 255);
        }
        else
        {
            speechBubble.SetActive(false);
        }

    }

    /// <summary>This method sets the content in the agent over head display/speech-bubble. 
    /// Parameters are the sprite, contentRotation, text, rotation and the directionText. </summary>
    /// <param name="sprite"> the sprite to set the image to. </param>
    /// <param name="contentRotation"> rotation of the sprite. </param>
    /// <param name="text"> string text to display. </param>
    /// <param name="rotation"> rotation of the speech-bubble. </param>
    /// <param name="directionText"> boolean wheather to show the direction text or not. </param>
    void SetSpeechBubbleContent(Sprite sprite, Vector3 contentRotation, string text, float rotation, bool directionText)
    {
        speechBubbleContent.transform.GetComponent<Image>().sprite = sprite;
        speechBubbleDirectionText.GetComponent<TextMeshProUGUI>().text = text;
        speechBubbleContent.transform.rotation = Quaternion.Euler(new Vector3(contentRotation.x, contentRotation.y, contentRotation.z + rotation));
        speechBubbleDirectionText.SetActive(directionTex);
        speechBubble.SetActive(true);
    }
    /// <summary>This method sets name in the speech-bubble. </summary>
    /// <param name="name"> string name to set to. </param>
    public void SetAgentNameInSpeechBubble(string name)
    {
        speechBubbleAgentName.transform.GetComponent<Text>().text = GetAgentNumberFromNameAsString(name);

    }
    #endregion

    #region helper methods

    /// <summary>This method returns the agent number as a strring based on its name string. </summary>
    /// <param name="name"> string name to set to. </param>
    string GetAgentNumberFromNameAsString(string name)
    {
        string nameNew = Regex.Replace(name, "[^0-9]", "");
        return nameNew;
    }

    /// <summary>This method returns a certain child gameobject from a parent based on its tag. </summary>
    /// <param name="parent"> parent gameobject. </param>
    /// <param name="tag"> string tag of object. </param>
    public static GameObject FindGameObjectInChildWithTag(GameObject parent, string tag)
    {
        Transform t = parent.transform;

        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).gameObject.tag == tag)
            {
                return t.GetChild(i).gameObject;
            }

        }

        return null;
    }

    /// <summary>This method returns the recalculated position according to the unity coordinate system as a Vector3.
    /// This is used to transform the position values taken from the JSON. </summary>
    /// <param name="x"> original x value. </param>
    /// <param name="y"> original y value. </param>
    /// <param name="z"> original z value. </param>
    public Vector3 GetRecalculatedPosition(float x, float y, float z)
    {
        Vector3 center = system.GetComponent<EnvironmentStateManager>().environmentCenter;
        return new Vector3(x - center.x, y, z - center.z);
    }
    /// <summary>This method sets the global variable agentModel to the indicated agent. </summary>
    /// <param name="agent"> agent to set the model to. </param>
    public void SetAgentModel(Agent agent)
    {
        this.agentModel = agent;
    }

    /// <summary>This method returns the length of the current action. </summary>
    public float GetCurrentActionLength()
    {
        return this.currentActionLength;
    }
    #endregion

}
