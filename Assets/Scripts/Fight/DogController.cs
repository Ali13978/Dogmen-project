using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DogController : MonoBehaviour
{
    public Rigidbody rgd;
    Animator MyAnimator;

    private float Anim_speed;
    // Start is called before the first frame update
    void Start()
    {
        MyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rgd == null)
            return;

        float speed = rgd.velocity.magnitude;
        Anim_speed = Mathf.Lerp(Anim_speed , speed/5 ,Time.deltaTime*5);
        MyAnimator.SetFloat("speed", Anim_speed);
    }
}
