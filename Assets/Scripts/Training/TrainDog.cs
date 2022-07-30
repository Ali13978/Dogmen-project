using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainDog : MonoBehaviour
{
    [Header("Required for both")]
    [SerializeField] List<GameObject> AllDogs;
    int SelectedDog;

    [Header("Require for Tire")]
    [SerializeField] float RotatingSpeed = 30;
    [SerializeField] bool IsTrainingwithTyre = false;
    Vector3 Wheelpos;
    Quaternion WheelRot;
    Rigidbody MyRigidBody;
    [HideInInspector] public bool AttachedWithTire = false;

    [Header("Required For TreadMill")]
    [SerializeField] Slider TreadMillSlider;
    [SerializeField] public float SliderMaxValue;
    [SerializeField] float Decreasingamount;
    [SerializeField] float ValueAdded;
    [HideInInspector]public float CurrentValue;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (GameObject i in AllDogs)
        {
            i.SetActive(false);
        }
        for (int i = 0; i <= AllDogs.Count; i++)
        {
            if (i == PlayerPrefs.GetInt("SelectedDog"))
            {
                SelectedDog = i;
                AllDogs[SelectedDog].SetActive(true);
            }
        }
        if (IsTrainingwithTyre)
        {
            MyRigidBody = GetComponent<Rigidbody>();
            Wheelpos = transform.position;
            WheelRot = transform.rotation;
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
            if(Input.GetButtonDown("Jump"))
            {
                AllDogs[SelectedDog].GetComponent<Animator>().SetBool("JumpPressed", true);
            }
            if (Input.GetAxis("Horizontal") != 0 && AttachedWithTire)
            {
                float Speed = RotatingSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
                MyRigidBody.angularVelocity += new Vector3(Speed, Speed, Speed);
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

    public void TurnOnTyreOn()
    {
        if (IsTrainingwithTyre)
        {
            AllDogs[SelectedDog].GetComponent<Animator>().SetBool("ForTyre", true);
        }
    }

    public void ResetWheelTransform()
    {
        if(IsTrainingwithTyre)
        {
            transform.position = Wheelpos;
            transform.rotation = WheelRot;
            MyRigidBody.angularVelocity = Vector3.zero;
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
