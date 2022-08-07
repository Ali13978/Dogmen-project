using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    [SerializeField] GameObject Dog;
    Animator MyAnimator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void leaveDog()
    {
        Dog.transform.parent = transform.parent;
        Dog.GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Animator>().SetBool("IsDogLeft", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
