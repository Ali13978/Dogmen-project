using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Animator))]
public class DogController : MonoBehaviourPunCallbacks
{
    public Rigidbody rgd;
    Animator MyAnimator;
    bool IsAttacking = false;

    private float Anim_speed;
    private void Awake()
    {
        if (photonView.IsMine)
        {
            MyAnimator = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            UpdateAnimation();
            Attack();
            StopMoving();
        }
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float speed = rgd.velocity.magnitude;
            Anim_speed = Mathf.Lerp(Anim_speed, speed / 5, Time.deltaTime * 5);
            MyAnimator.SetFloat("speed", Anim_speed);
            MyAnimator.SetBool("BasicAttack", true);
            IsAttacking = true;
        }
    }

    private void StopMoving()
    {
        if (IsAttacking)
        {
            rgd.velocity = new Vector3(0, 0, 0);
        }
    }

    private void EndAttack()
    {
        IsAttacking = false;
        MyAnimator.SetBool("BasicAttack", false);
    }

    private void UpdateAnimation()
    {
        if (rgd == null)
        { return; }
        float speed = rgd.velocity.magnitude;
        Anim_speed = Mathf.Lerp(Anim_speed, speed / 5, Time.deltaTime * 5);
        MyAnimator.SetFloat("speed", Anim_speed);
    }
}
