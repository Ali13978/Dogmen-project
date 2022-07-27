using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainDog : MonoBehaviour
{
    [Header("Require for Tire")]
    [SerializeField] float RotatingSpeed = 30;
    [SerializeField] bool IsTrainingwithTyre = false;
    Rigidbody MyRigidBody;

    [Header("Required For TreadMill")]
    [SerializeField] Slider TreadMillSlider;
    [SerializeField] public float SliderMaxValue;
    [SerializeField] float Decreasingamount;
    [SerializeField] float ValueAdded;
    [HideInInspector]public float CurrentValue;

    // Start is called before the first frame update
    void Awake()
    {
        if (IsTrainingwithTyre)
        {
            MyRigidBody = GetComponent<Rigidbody>();
        }
    }

    private void Start()
    {
        if (!IsTrainingwithTyre)
        {
            TreadMillSlider.maxValue = SliderMaxValue;
            TreadMillSlider.value = SliderMaxValue;
            CurrentValue = SliderMaxValue;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (IsTrainingwithTyre)
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                float Speed = RotatingSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
                MyRigidBody.angularVelocity += new Vector3(0, 0, Speed);
            }
        }
        else
        {
            if(Input.GetButtonDown("Jump"))
            {
                CurrentValue += ValueAdded * Time.deltaTime;
                if(CurrentValue > SliderMaxValue)
                {
                    CurrentValue = SliderMaxValue;
                }
                TreadMillSlider.value = CurrentValue; 
            }
        }
    }

    private void LateUpdate()
    {
        if(!IsTrainingwithTyre)
        {
            CurrentValue -= Decreasingamount * Time.deltaTime;
            TreadMillSlider.value = CurrentValue;
            if(CurrentValue <= 0)
            {
                CurrentValue = 0;
                GetComponentInChildren<Animator>().SetBool("IsSlipped", true);
            }
        }
    }
    
}
