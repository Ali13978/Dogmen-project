using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    Animator MyAnimator;

    private void Awake()
    {
        MyAnimator = GetComponent<Animator>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void SetSittingFalse()
    {
        MyAnimator.SetBool("IsSitting", false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
