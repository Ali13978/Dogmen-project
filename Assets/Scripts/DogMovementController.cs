using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogMovementController : MonoBehaviour
{
    private GameObject DogObject;
    Transform[] children;
    [Header("Action Keys")]
    public KeyCode forward = KeyCode.W; // Move forward
    public KeyCode backward = KeyCode.S; // Move backward
    public KeyCode left = KeyCode.A; // Move left
    public KeyCode right = KeyCode.D; // Move left
    public KeyCode run = KeyCode.LeftShift; // Run
    public KeyCode sit = KeyCode.LeftControl; // Sit
    static private KeyCode[] dogKeyCodes; // Keycode array for assigned keys
    Animator dogAnim;// Animator for the assigned dog
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
        forward,
        backward,
        left,
        right,
        run,
        sit
        };

    }

    void Update()
    {
        bool walkPressed = Input.GetKey(dogKeyCodes[0]);
        bool turnBack = Input.GetKey(dogKeyCodes[1]);
        bool leftTurn = Input.GetKey(dogKeyCodes[2]);
        bool rightTurn = Input.GetKey(dogKeyCodes[3]);
        bool runPressed = Input.GetKey(dogKeyCodes[4]);
        bool sitPressed = Input.GetKeyDown(dogKeyCodes[5]);

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
