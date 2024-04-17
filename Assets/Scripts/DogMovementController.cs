using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogMovementController : MonoBehaviour
{
    private GameObject DogObject;
    Transform[] children;
    [Header("Action Keys")]
    public KeyCode Attack = KeyCode.Mouse0; // Mouse Left-Click Attack
    public KeyCode secondAttack = KeyCode.Mouse1; // Mouse Right-Click Attack
    public KeyCode forward = KeyCode.W; // Move forward
    public KeyCode backward = KeyCode.S; // Move backward
    public KeyCode left = KeyCode.A; // Move left
    public KeyCode right = KeyCode.D; // Move left
    public KeyCode action = KeyCode.F; // action
    public KeyCode jump = KeyCode.Space; // Jump
    public KeyCode run = KeyCode.LeftShift; // Run
    public KeyCode sit = KeyCode.LeftControl; // Sit
    public KeyCode sleep = KeyCode.Q; // Sleep
    public KeyCode ExitPlaymode = KeyCode.Escape; // Escape playmode
    public KeyCode Death = KeyCode.X; // Death 
    public KeyCode Reset = KeyCode.R; // Reset dog
    [Header("Custom Actions")]
    public KeyCode action1 = KeyCode.Alpha1;
    public KeyCode action2 = KeyCode.Alpha2;
    public KeyCode action3 = KeyCode.Alpha3;
    public KeyCode action4 = KeyCode.Alpha4;
    public KeyCode action5 = KeyCode.Alpha5;
    public KeyCode action6 = KeyCode.Alpha6;
    public KeyCode action7 = KeyCode.Alpha7;
    public KeyCode action8 = KeyCode.Alpha8;
    public KeyCode action9 = KeyCode.Alpha9;
    public KeyCode action10 = KeyCode.Alpha0;
    public KeyCode action11 = KeyCode.F1;
    public KeyCode action12 = KeyCode.F2;
    public KeyCode action13 = KeyCode.F3;
    static private KeyCode[] dogKeyCodes; // Keycode array for assigned keys
    Animator dogAnim;// Animator for the assigned dog
    bool dogActionEnabled;
    public float timeRemaining = 1.0f;
    private int countDown = 1;
    bool Movement_f;
    bool death_b = false;
    bool Sleep_b = false;
    bool Sit_b = false;
    private float w_movement = 0.0f; // Run value
    public float acceleration = 1.0f;
    public float decelleration = 1.0f;
    private float maxWalk = 0.5f;
    private float maxRun = 1.0f;
    private float currentSpeed;
    void Start() // On start store dogKeyCodes
    {
        dogAnim = GetComponent<Animator>(); // Get the animation component
        currentSpeed = 0.0f;

        dogKeyCodes = new KeyCode[]
        {
        Attack,
        secondAttack,
        forward,
        backward,
        left,
        right,
        action,
        jump,
        run,
        sit,
        sleep,
        ExitPlaymode,
        Death,
        Reset,
        action1,
        action2,
        action3,
        action4,
        action5,
        action6,
        action7,
        action8,
        action9,
        action10,
        action11,
        action12,
        action13
        };

    }

    void Update()
    {
        bool walkPressed = Input.GetKey(dogKeyCodes[2]);
        bool turnBack = Input.GetKey(dogKeyCodes[3]);
        bool leftTurn = Input.GetKey(dogKeyCodes[4]);
        bool rightTurn = Input.GetKey(dogKeyCodes[5]);
        bool runPressed = Input.GetKey(dogKeyCodes[8]);
        bool sitPressed = Input.GetKeyDown(dogKeyCodes[9]);

        if (runPressed)
        {
            currentSpeed = maxRun;
        }
        if (!runPressed)
        {
            currentSpeed = maxWalk;
        }
        if (walkPressed && (w_movement < currentSpeed)) // If walking
        {
            w_movement += Time.deltaTime * acceleration;
        }
        if (walkPressed && !runPressed && w_movement > currentSpeed) // Slow down
        {
            w_movement -= Time.deltaTime * decelleration;

        }
        if (!walkPressed && w_movement > 0.0f) // If no longer walking
        {
            w_movement -= Time.deltaTime * decelleration;
        }
        if (leftTurn)
        {
            if (w_movement > 0.25 && w_movement < 0.75)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * -45, Space.Self);
            }
            if (w_movement > 0.75)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * -65, Space.Self);
            }
            if (w_movement < 0.25)
            {
                dogAnim.SetInteger("TurnAngle_int", -90);
            }
        }
        else if (rightTurn)
        {
            if (w_movement > 0.25 && w_movement < 0.75)
            {
                transform.Rotate(-Vector3.down * Time.deltaTime * 45, Space.Self);
            }
            if (w_movement > 0.75)
            {
                transform.Rotate(-Vector3.down * Time.deltaTime * 65, Space.Self);
            }
            if (w_movement < 0.25)
            {
                dogAnim.SetInteger("TurnAngle_int", 90);
            }
        }
        else if (turnBack)
        {
            dogAnim.SetInteger("TurnAngle_int", 180);
        }
        else
        {
            dogAnim.SetInteger("TurnAngle_int", 0);
        }
        if (sitPressed) // Sit
        {
            if (Sit_b == false)
            {
                Sit_b = true;
            }
            else if (Sit_b == true)
            {
                Sit_b = false;
            }
            dogAnim.SetBool("Sit_b", Sit_b); // Set sit animation
        }
        dogAnim.SetTrigger("Blink_tr");
        dogAnim.SetFloat("Movement_f", w_movement); // Set movement speed for all required parameters
    }
}
