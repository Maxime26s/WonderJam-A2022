using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private float jumpHeight = 1f;
    [SerializeField]
    private float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;

    private bool isNoClipping = false;

    private int health = 3;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    private void ToggleNoClip()
    {
        isNoClipping = !isNoClipping;
        controller.enabled = !isNoClipping;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.PlayerGetNoClipInput())
            ToggleNoClip();

        if (!isNoClipping)
        {
            MoveUpdate();
        }
        else
        {
            NoclipUpdate();
        }

        SwapWeapon();
    }

    private void NoclipUpdate()
    {
        playerVelocity = Vector3.zero;

        Vector2 movement = InputManager.Instance.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0, movement.y);

        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.Normalize();
        controller.transform.position += move * Time.deltaTime * speed * 1.5f;
    }

    private void MoveUpdate()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = InputManager.Instance.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0, movement.y);

        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        move.Normalize();
        controller.Move(move * Time.deltaTime * speed);

        if (InputManager.Instance.PlayerJumpedThisFrame() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void SwapWeapon() {
        if (InputManager.Instance.PlayerGetSelectWrench() == true) {
            Debug.Log("Wrench Selected");
            return;
        }
        if (InputManager.Instance.PlayerGetSelectShotgun() == true) {
            Debug.Log("Shotgun Selected");
            return;
        }
        if (InputManager.Instance.PlayerGetSelectBomb() == true) {
            Debug.Log("Bomb Selected");
            return;
        }
        if (InputManager.Instance.PlayerGetScrollUpWeapon() > 0) {
            Debug.Log("Scrolled up");
            return;
        }
        if (InputManager.Instance.PlayerGetScrollDownWeapon() > 0) {
            Debug.Log("Scrolled down");
            return;
        }

    }

    public void TakeDamage()
    {
        health--;
    }
}
