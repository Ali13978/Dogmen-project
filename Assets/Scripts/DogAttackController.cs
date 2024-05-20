using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAttackController : MonoBehaviour
{
    [Header("Action Keys")]
    public KeyCode Attack = KeyCode.Mouse0; // Mouse Left-Click Attack
    public KeyCode secondAttack = KeyCode.Mouse1; // Mouse Right-Click Attack
    static private KeyCode[] dogKeyCodes; // Keycode array for assigned keys
    Animator dogAnim;// Animator for the assigned dog
    void Start() // On start store dogKeyCodes
    {
        dogAnim = GetComponent<Animator>(); // Get the animation component
        dogKeyCodes = new KeyCode[]
        {
        Attack,
        secondAttack,
        };

    }

    void Update()
    {
        bool attackMode = Input.GetKey(dogKeyCodes[0]); // Get the current keycodes assigned by user
        bool secondAttack = Input.GetKey(dogKeyCodes[1]);

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

    }
}
