/*
 * POLYGON DOG ANIMATION SCRIPT
 * DESCRIPTION: This script demonstrates the range of animations and Prefabs included in 
 * Polygon Dog which can be customized for the users preference. Please attach this to the
 * Dog.Prefab asset then customize the keys for the particular animations.
 * 
 * PLEASE NOTE: This script is intended for demonstration purposes and user customization or 
 * third party animation plugins will be required for further animation options.
 * 
 */
using System.Collections;
using UnityEngine;
public class POLYGON_DogAnimationController : MonoBehaviour
{
    public enum DogTypeList
    {
        Coyote,
        Dalmatian,
        DalmatianCollar,
        Doberman,
        DobermanCollar,
        Fox,
        GermanShepherd,
        GermanShepherdCollar,
        GoldenRetriever,
        GoldenRetrieverCollar,
        DogGreyhound,
        GreyhoundCollar,
        HellHound,
        Husky,
        HuskyCollar,
        Labrador,
        LabradorCollar,
        Pointer,
        PointerCollar,
        Ridgeback,
        RidgebackCollar,
        Robot,
        Scifi,
        Shiba,
        Shiba_Collar,
        Wolf,
        ZombieDoberman,
        ZombieGermanShepherd
    };
    public bool DisplayUI = true; // Toggle displaying UI
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
    static private string[] dogLabels;
    static private string[] DogNewTypes; // Dog Types
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
    private Transform getDogName;
    private GUIStyle guiStyle = new GUIStyle();
    [Header("Particle FX")]
    public ParticleSystem poopFX;
    public ParticleSystem dirtFX;
    public ParticleSystem peeFX;
    public ParticleSystem waterFX;
    private Vector3 newSpawn = new Vector3();
    public Transform fxTransform;
    public Transform fxTail;
    void Start() // On start store dogKeyCodes
    {
        dogAnim = GetComponent<Animator>(); // Get the animation component
        currentSpeed = 0.0f;
        DogNewTypes = new string[]
        {
        "Coyote",
        "Dalmatian",
        "DalmatianCollar",
        "Doberman",
        "DobermanCollar",
        "Fox",
        "GermanShepherd",
        "GermanShepherdCollar",
        "GoldenRetriever",
        "GoldenRetrieverCollar",
        "DogGreyhound",
        "GreyhoundCollar",
        "HellHound",
        "Husky",
        "HuskyCollar",
        "Labrador",
        "LabradorCollar",
        "Pointer",
        "PointerCollar",
        "Ridgeback",
        "RidgebackCollar",
        "Robot",
        "Scifi",
        "Shiba",
        "Shiba_Collar",
        "Wolf",
        "ZombieDoberman",
        "ZombieGermanShepherd"
        };
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
        dogLabels = new string[] // Labels for UI
        {
        "Attack: ",
        "Second Attack: ",
        "Forward: ",
        "Backward: ",
        "Left: ",
        "Right: ",
        "Action: ",
        "Jump: ",
        "Run: ",
        "Sit: ",
        "Sleep: ",
        "Exit Playmode: ",
        "Death: ",
        "Reset: ",
        "Action 1: ",
        "Action 2: ",
        "Action 3: ",
        "Action 4: ",
        "Action 5: ",
        "Action 6: ",
        "Action 7: ",
        "Action 8: ",
        "Action 9: ",
        "Action 10: ",
        "Action 11: ",
        "Action 12: ",
        "Action 13: "
        };
        guiStyle.fontSize = 18;
        guiStyle.normal.textColor = Color.black;
        children = GetComponentsInChildren<Transform>();
        for (int x = 0; x < children.Length; x++) // Search for dog names
        {
            if (children[x].name.Contains("Dogs"))
            {
                getDogName = children[x];
            }
        }
        newSpawn = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    }
    IEnumerator DogActions(int actionType) // Dog action coroutine
    {
        dogActionEnabled = true; // Enable the dog animation flag
        dogAnim.SetInteger("ActionType_int", actionType); // Enable Animation
        yield return new WaitForSeconds(countDown); // Countdown
        dogAnim.SetInteger("ActionType_int", 0); // Disable animation
        dogActionEnabled = false; // Disable the dog animation flag
    }
    void OnGUI()
    {
        if(DisplayUI) // Display Dog keycodes on UI
        {
        GUI.backgroundColor = Color.yellow;
        for (int i = 0; i < dogKeyCodes.Length; i++)
        {
        GUI.Label(new Rect(10, 10 + (i * 30), 400, 30), dogLabels[i] + " " + dogKeyCodes[i].ToString(), guiStyle);
        }
        } 
    }
   void Update()
    {
        bool attackMode = Input.GetKey(dogKeyCodes[0]); // Get the current keycodes assigned by user
        bool secondAttack = Input.GetKey(dogKeyCodes[1]);
        bool walkPressed = Input.GetKey(dogKeyCodes[2]);
        bool turnBack = Input.GetKey(dogKeyCodes[3]);
        bool leftTurn = Input.GetKey(dogKeyCodes[4]);
        bool rightTurn = Input.GetKey(dogKeyCodes[5]);
        bool randActionPressed = Input.GetKeyDown(dogKeyCodes[6]);
        bool jumpPressed = Input.GetKeyDown(dogKeyCodes[7]);
        bool runPressed = Input.GetKey(dogKeyCodes[8]);
        bool sitPressed = Input.GetKeyDown(dogKeyCodes[9]);
        bool sleepPressed = Input.GetKeyDown(dogKeyCodes[10]);
        bool exitPressed = Input.GetKeyDown(dogKeyCodes[11]);
        bool deathPressed = Input.GetKeyDown(dogKeyCodes[12]);
        bool resetPressed = Input.GetKeyDown(dogKeyCodes[13]);
        bool a1Pressed = Input.GetKey(dogKeyCodes[14]);
        bool a2Pressed = Input.GetKey(dogKeyCodes[15]);
        bool a3Pressed = Input.GetKey(dogKeyCodes[16]);
        bool a4Pressed = Input.GetKey(dogKeyCodes[17]);
        bool a5Pressed = Input.GetKey(dogKeyCodes[18]);
        bool a6Pressed = Input.GetKey(dogKeyCodes[19]);
        bool a7Pressed = Input.GetKey(dogKeyCodes[20]);
        bool a8Pressed = Input.GetKey(dogKeyCodes[21]);
        bool a9Pressed = Input.GetKey(dogKeyCodes[22]);
        bool a10Pressed = Input.GetKey(dogKeyCodes[23]);
        bool a11Pressed = Input.GetKey(dogKeyCodes[24]);
        bool a12Pressed = Input.GetKey(dogKeyCodes[25]);
        bool a13Pressed = Input.GetKey(dogKeyCodes[26]);
        if (attackMode)
        {
            dogAnim.SetBool("AttackReady_b", true);
        }
        else
        {
            dogAnim.SetBool("AttackReady_b", false);
        }
        if (secondAttack)
        {
            dogAnim.SetInteger("AttackType_int", 2);
        }
        else
        {
            dogAnim.SetInteger("AttackType_int", 0);
        }
        if (randActionPressed)
        {
            float currentSpeed = a1Pressed ? 1 : maxWalk;
            dogAnim.SetInteger("ActionType_int", Random.Range(0, 13));
        }
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
        if (randActionPressed)
        {
            StartCoroutine(DogActions(Random.Range(1, 13)));
        }
        if (jumpPressed)
        {
            dogAnim.SetTrigger("Jump_tr");
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
        if (sleepPressed) // Sleep
        {
            if (Sleep_b == false)
            {
                Sleep_b = true;
            }
            else if (Sleep_b == true)
            {
                Sleep_b = false;
            }
            dogAnim.SetBool("Sleep_b", Sleep_b); // Set sleep animation
        }
        if (exitPressed)
        {

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBPLAYER
                Application.OpenURL(webplayerQuitURL);
        #else
            Application.Quit();
        #endif
        }
        if (deathPressed)
        {
            dogAnim.SetBool("Death_b", true);  // Kill the dog 
        }
        if (resetPressed)
        {
            dogAnim.Rebind();
            dogAnim.Update(0f);
        }
        if (a1Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(1));
        }
        if (a2Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(2));
        }
        if (a3Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(3));
        }
        if (a4Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(4));
            if(!Sit_b)
            {
            ParticleSystem go = Instantiate(dirtFX, new Vector3(this.transform.position.x, fxTransform.transform.position.y, fxTransform.transform.position.z), this.transform.rotation);
            go.transform.SetParent(fxTransform);
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, go.transform.localPosition.z + 0.3f); 
            }           
        }
        if (a5Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(5));
        }
        if (a6Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(6));
        }
        if (a7Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(7));
        }
        if (a8Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(8));
             if(!Sit_b)
            {
            ParticleSystem go = Instantiate(peeFX, new Vector3(this.transform.position.x, fxTransform.transform.position.y + 0.5f, fxTransform.transform.position.z - 0f), this.transform.rotation);
            go.transform.SetParent(fxTransform);
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, go.transform.localPosition.z - 0.2f);
            go.transform.localRotation = Quaternion.Euler(0, -45, 0);
            }  
        }
        if (a9Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(9));
             if(!Sit_b)
            {
            ParticleSystem go = Instantiate(poopFX, new Vector3(this.transform.position.x, fxTransform.transform.position.y + 0.5f, fxTransform.transform.position.z - 0f), this.transform.rotation);
            go.transform.SetParent(fxTransform);
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, go.transform.localPosition.z - 0.35f);
            }
        }
        if (a10Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(10));
             if(!Sit_b)
            {
            ParticleSystem go = Instantiate(waterFX, new Vector3(this.transform.position.x, fxTransform.transform.position.y + 0.5f, fxTransform.transform.position.z - 0f), this.transform.rotation);
            go.transform.SetParent(fxTransform);
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y - 0.0f, go.transform.localPosition.z);
            go.gameObject.transform.GetChild(0).transform.position = new Vector3(fxTail.transform.position.x, fxTail.transform.position.y, fxTail.transform.position.z);
            }
        }
        if (a11Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(11));
        }
        if (a12Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(12));
        }
        if (a13Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(13));
        }
        dogAnim.SetTrigger("Blink_tr"); // Blink will continue unless asleep or dead
        dogAnim.SetFloat("Movement_f", w_movement); // Set movement speed for all required parameters
    }
}