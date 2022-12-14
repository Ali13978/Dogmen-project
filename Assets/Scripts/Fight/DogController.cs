using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Animator))]
public class DogController : MonoBehaviourPunCallbacks
{
    public Rigidbody rgd;
    public Animator MyAnimator;
    bool IsAttacking = false;
    [SerializeField] public GameObject HitBox;
    [SerializeField] public int Damage = 10;

    [SerializeField] GameObject GameOverPannel;

    [HideInInspector] public PhotonView pv;
    
    private float Anim_speed;
    private void Awake()
    {
        if (photonView.IsMine)
        {
            pv = GetComponent<PhotonView>();
        }
    }

    private void GameCompleted()
    {
        if (!PhotonNetwork.IsMasterClient && pv.IsMine)
        {
            PhotonNetwork.SetMasterClient(pv.Owner);
        }
        PhotonNetwork.LoadLevel(3);
    }

    [PunRPC]
    void SyncValues(GameObject _gameOver)
    {
        _gameOver.SetActive(true);
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
        if (Input.GetMouseButtonDown(0) && !IsAttacking)
        {
            MyAnimator.SetBool("BasicAttack", true);
            IsAttacking = true;
        }

        if(Input.GetMouseButtonDown(1) && !IsAttacking)
        {
            MyAnimator.SetBool("JumpAttack", true);
            IsAttacking = true;
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            MyAnimator.SetBool("LightAttack-L", true);
            IsAttacking = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            MyAnimator.SetBool("LightAttack-R", true);
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
        MyAnimator.SetBool("LightAttack-L", false);
        MyAnimator.SetBool("LightAttack-R", false);
        MyAnimator.SetBool("JumpAttack", false);
    }

    private void TurnOffHitAnim()
    {
        GetComponentInParent<AppearSelectedDog>().IsHitted = false;
        MyAnimator.SetBool("LightHit", false);
        MyAnimator.SetBool("HeavyHit", false);
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
