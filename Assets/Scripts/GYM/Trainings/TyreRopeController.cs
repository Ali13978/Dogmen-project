using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyreRopeController : MonoBehaviour
{
    #region Singleton
    public static TyreRopeController instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField] GameObject tyreRopeCam;
    [SerializeField] GameObject tyreRopeDog;
    [SerializeField] TreadmillDog tyreRopeDogController;
    [SerializeField] Rigidbody tyreRopeRB;
    [SerializeField] private float swingForce = 10f;
    private bool tyreRopeEnabled = false;

    public void EnableTyreRope(DogSO dog)
    {
        tyreRopeDog.SetActive(true);
        tyreRopeCam.SetActive(true);
        tyreRopeDogController.InitDog(dog);
        tyreRopeEnabled = true;
        tyreRopeDogController.EnableWalkAnim();

    }

    public void DisableTyreRope()
    {
        tyreRopeDog.SetActive(false);
        tyreRopeCam.SetActive(false);
        tyreRopeEnabled = false;
        tyreRopeDogController.EnableIdleAnim();
    }
    void Update()
    {
        if (!tyreRopeEnabled)
            return;

        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 swingForceVector = new Vector3(-1 * horizontalInput * swingForce, 2f, 1f);
        tyreRopeRB.AddForce(swingForceVector, ForceMode.Force);
    }
}
