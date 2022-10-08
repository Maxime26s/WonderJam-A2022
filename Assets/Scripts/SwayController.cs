using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayController : MonoBehaviour
{

    [SerializeField]
    private float aimSwayFactor = 25f;
    [SerializeField]
    private float movementSwayFactor = 20f;
    [SerializeField]
    private float aimSwayClampY = 10f;
    [SerializeField]
    private float aimSwayClampX = 10f;
    [SerializeField]
    private float movementSwayClampY = 10f;
    [SerializeField]
    private float movementSwayClampX = 10f;
    [SerializeField]
    private float aimSwaySmoothTime = 2f;
    [SerializeField]
    private float aimSwaySmoothing = 0f;
    [SerializeField]
    private bool invertX = false;
    [SerializeField]
    private bool invertY = false;

    private Vector3 targetWeaponRotation;
    private Vector3 targetWeaponRotationVelocity;
    private Vector3 newWeaponRotation;
    private Vector3 newWeaponRotationVelocity;
    private float inputY;
    private float inputX;
    private float horizontalMovement;
    private float verticalMovement;

    private Vector3 targetWeaponMovementRotation;
    private Vector3 targetWeaponMovementRotationVelocity;
    private Vector3 newWeaponMovementRotation;
    private Vector3 newWeaponMovementRotationVelocity;

    void Update()
    {
        AimingSway();
    }

    void AimingSway() {

        inputX = -Input.GetAxis("Mouse Y");
        inputY = Input.GetAxis("Mouse X");
        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");


        if (invertX)
            horizontalMovement = -horizontalMovement;
        if (invertY)
            verticalMovement = -verticalMovement;

        targetWeaponRotation.y += inputY * aimSwayFactor * Time.deltaTime;
        targetWeaponRotation.x += inputX * aimSwayFactor * Time.deltaTime;

        targetWeaponRotation.y = Mathf.Clamp(targetWeaponRotation.y, -aimSwayClampY, aimSwayClampY);
        targetWeaponRotation.x = Mathf.Clamp(targetWeaponRotation.x, -aimSwayClampX, aimSwayClampX);
        targetWeaponRotation.z = -targetWeaponRotation.y * 2f;

        targetWeaponRotation = Vector3.SmoothDamp(targetWeaponRotation, Vector3.zero, ref targetWeaponRotationVelocity, aimSwaySmoothTime);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, targetWeaponRotation, ref newWeaponRotationVelocity, aimSwaySmoothing);

        targetWeaponMovementRotation.z = movementSwayFactor * horizontalMovement;
        targetWeaponMovementRotation.x = movementSwayFactor * verticalMovement;

        targetWeaponMovementRotation.z = Mathf.Clamp(targetWeaponMovementRotation.z, -movementSwayClampX, movementSwayClampX);
        targetWeaponMovementRotation.x = Mathf.Clamp(targetWeaponMovementRotation.x, -movementSwayClampY, movementSwayClampY);

        targetWeaponMovementRotation = Vector3.SmoothDamp(targetWeaponMovementRotation, Vector3.zero, ref targetWeaponMovementRotationVelocity, aimSwaySmoothTime);
        newWeaponMovementRotation = Vector3.SmoothDamp(newWeaponMovementRotation, targetWeaponMovementRotation, ref newWeaponMovementRotationVelocity, aimSwaySmoothTime);

        transform.localRotation = Quaternion.Euler(newWeaponRotation + newWeaponMovementRotation);
    }
}
