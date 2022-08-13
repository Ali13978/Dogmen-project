using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HumanController : MonoBehaviour
{
    Animator MyAnimator;
    [SerializeField] GameObject Dogs;
    bool IsSitting = false;

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
        MyAnimator.SetBool("IsSitting", false);
    }

    private void DestroyHuman()
    {
        Destroy(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2 && !IsSitting)
        {
            MyAnimator.SetBool("IsSitting", true);
            IsSitting = true;
        }
    }
}
