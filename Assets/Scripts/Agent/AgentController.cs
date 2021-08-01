using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentController : MonoBehaviour
{

    Vector3 currentpos;
    Vector3 goalPosition;
    Vector3 endGoalPosition;
    Vector3 lastReachedGoalPosition;
    int lastReachedGoalPositionIndex;

    [SerializeField] float playBackSpeed;

    [SerializeField] float speed;

    [SerializeField] List<Vector3> positions = new List<Vector3>();

    GameObject system;
    Text positionText;

    public int currentArrayPosOnSlider = 0;

    bool isCleaning = false;
    bool collision;

    bool stopMoving = false;
    Slider slider;

    Button playButton;
    Button pauseButton;


    public bool playingSequence;

    float sequenceTimer;

    int playButtonCounter;

    float distance;

    public Canvas canvas;
    public GameObject fieldOfViewPlane;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        currentpos = this.transform.position;
        endGoalPosition = currentpos;
        lastReachedGoalPosition = currentpos;
        lastReachedGoalPositionIndex = 0;
        initializePositions();

        system = GameObject.FindWithTag("System");

        playButton = system.GetComponent<UIGlobals>().playButton;
        pauseButton = system.GetComponent<UIGlobals>().pauseButton;

        slider = system.GetComponent<UIGlobals>().slider;

        playButton.onClick.AddListener(PlaySequence);
        pauseButton.onClick.AddListener(PauseSequence);
        playingSequence = false;
        sequenceTimer = 0f;
        playButtonCounter = 0;
        distance = 0f;

        slider.maxValue = positions.Count - 1;
        slider.onValueChanged.AddListener(delegate { UpdateGoalPositionAccordingToSlider(); });

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        collision = this.GetComponent<AgentCollision>().GetCollision();
        currentpos = this.transform.position;

        if (!stopMoving)
        {
            if (playingSequence)
            {
                HandleSequencePlayUpdate();
            }

            if (currentpos != goalPosition && goalPosition != null && !isCleaning && !collision)
            {
                MoveRobotTo(goalPosition);
                //positionText zeigt aktuell die zwischenstopps an, lieber endGoalPosition?
                positionText.text = "Moving to x: " + goalPosition.x + " y: " + goalPosition.y + " z: " + goalPosition.z;
            }

            if (currentpos == goalPosition && goalPosition != endGoalPosition)
            {
                Debug.Log("---------------------Reached Endgoal!");
                float diff = slider.value - lastReachedGoalPositionIndex;
                if (diff > 0)
                {
                    var index = positions.IndexOf(goalPosition) + 1;
                    goalPosition = positions[index];
                }
                else
                {
                    var index = positions.IndexOf(goalPosition) - 1;
                    goalPosition = positions[index];
                }
            }
            if (currentpos == endGoalPosition)
            {
                lastReachedGoalPosition = endGoalPosition;
                lastReachedGoalPositionIndex = positions.IndexOf(endGoalPosition);
            }

            if (collision)
            {
                BounceOff();
            }
        }

        if (isCleaning)
        {
            stopMoving = true;
            //clean --> call in different script
        }


    }

    public void UpdateGoalPositionAccordingToSlider()
    {
        //TODO does not work yet when slider moved to fast
        stopMoving = false;
        Debug.Log("-------------------Slider adjusted....." + slider.value);
        float diff = slider.value - lastReachedGoalPositionIndex;
        Debug.Log("-------------------Slider difference....." + slider.value);
        if (playingSequence)
        {
            speed = playBackSpeed;
        }
        else
        {
            speed = speed * Mathf.Abs(diff);
        }

        currentArrayPosOnSlider = (int)slider.value;
        endGoalPosition = positions[currentArrayPosOnSlider];

        if (diff > 1)
        {
            goalPosition = positions[lastReachedGoalPositionIndex + 1];
        }
        if (diff == 1 || diff == -1)
        {
            goalPosition = endGoalPosition;
        }
        //TODO not yet working; check if end of positions list reached
        if (diff < 0)
        {
            goalPosition = positions[lastReachedGoalPositionIndex - 1];
        }
        distance = Vector3.Distance(currentpos, goalPosition);

    }

    public void HandleSequencePlayUpdate()
    {
        if (slider.value == 0f)
        {
            slider.value = 1f;
        }
        else if (currentpos == endGoalPosition && slider.value < positions.Count)
        {
            slider.value += 1f;
        }
        else if (currentpos == endGoalPosition && ((int)slider.value) == (positions.Count - 1))
        {
            Debug.Log("-----------Playback slider value finished list: " + slider.value);
            slider.value = 0f;
        }
        Debug.Log("-----------Playback slider value: " + slider.value);
        Debug.Log("-----------Position Count: " + (positions.Count - 1));

        /*else
        {
            Debug.Log("------------Sequence timer: " + sequenceTimer);
            if ((int)sequenceTimer == (slider.value + 1) || (int)sequenceTimer == 0)
            {
                slider.value += sequenceTimer;
                Debug.Log("------------Slider value set in Sequence: " + slider.value);

            }
        }
        sequenceTimer += playBackSpeed;*/
    }

    public void MoveRobotTo(Vector3 position)
    {
        animator.SetBool("moving", true);
        animator.SetFloat("speed", speed);

        this.transform.position = Vector3.MoveTowards(transform.position, goalPosition, Time.deltaTime * speed);
        Vector3 difference = goalPosition - currentpos;
        this.transform.forward = difference;
        fieldOfViewPlane.transform.position = Vector3.MoveTowards(transform.position, goalPosition, Time.deltaTime * speed);


        /*
        Debug.Log("-------------------Difference....." + difference.x + " " + difference.y + " " + difference.z);
        Debug.Log("-------------------speed: " + speed);

        this.transform.position = currentpos + difference * speed;
        this.transform.forward = difference;
        //canvas.GetComponent<RectTransform>().forward = Vector3.zero;
        fieldOfViewPlane.transform.position = currentpos + difference * speed; */
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
            var index = positions.IndexOf(goalPosition) + 1;
            goalPosition = positions[index];
        }
    }

    void initializePositions()
    {
        positions.Add(new Vector3(0, 0, 0));
        positions.Add(new Vector3(-5, 0, 0));
        positions.Add(new Vector3(-4, 0, 10));
        positions.Add(new Vector3(15, 0, 0));
        positions.Add(new Vector3(8, 0, -2));
    }

    void PlaySequence()
    {
        playingSequence = true;
        stopMoving = false;
    }

    void PauseSequence()
    {
        playingSequence = false;
        stopMoving = true;
    }

    public void setPositionText(Text text)
    {
        this.positionText = text;
    }

}
