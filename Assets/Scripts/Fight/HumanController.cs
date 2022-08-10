using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    Animator MyAnimator;
    [SerializeField] GameObject Dogs;

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
        Dogs.transform.parent = transform.parent;
        Destroy(gameObject);
        MyAnimator.SetBool("IsSitting", false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
