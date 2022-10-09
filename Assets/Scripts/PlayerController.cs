using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour {
    [SerializeField]
    GameObject wrench;
    [SerializeField]
    GameObject shotgun;
    [SerializeField]
    GameObject bomb;

    GameManager gameManager;


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

    public int health = 3;

    public UnityEvent playerJumping = new UnityEvent();
    public UnityEvent playerTakeDamage = new UnityEvent();

    // Start is called before the first frame update
    void Start() {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        gameManager = GameManager.Instance;
    }

    private void ToggleNoClip() {
        isNoClipping = !isNoClipping;
        controller.enabled = !isNoClipping;
    }

    // Update is called once per frame
    void Update() {
        if (InputManager.Instance.PlayerGetNoClipInput())
            ToggleNoClip();

        if (!isNoClipping) {
            MoveUpdate();
        } else {
            NoclipUpdate();
        }

        SwapWeapon();
    }

    private void NoclipUpdate() {
        playerVelocity = Vector3.zero;

        Vector2 movement = InputManager.Instance.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0, movement.y);

        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.Normalize();
        controller.transform.position += move * Time.deltaTime * speed * 1.5f;
    }

    private void MoveUpdate() {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }

        Vector2 movement = InputManager.Instance.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0, movement.y);

        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        move.Normalize();
        controller.Move(move * Time.deltaTime * speed);

        if (InputManager.Instance.PlayerJumpedThisFrame() && groundedPlayer) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            playerJumping.Invoke();
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private bool CheckWeaponsOnCooldown() {
        return (wrench.GetComponent<Wrench>().onCooldown || shotgun.GetComponent<Shotgun>().onCooldown);
    }


    private void SwapWeapon() {
        if (InputManager.Instance.PlayerGetSelectWrench() == true) {
            EquipWrench();
            Debug.Log("Wrench Selected");
            return;
        }
        if (InputManager.Instance.PlayerGetSelectShotgun() == true) {
            EquipShotgun();
            Debug.Log("Shotgun Selected");
            return;
        }
        if (InputManager.Instance.PlayerGetSelectBomb() == true) {
            EquipBomb();
            Debug.Log("Bomb Selected");
            return;
        }
        if (InputManager.Instance.PlayerGetScrollUpWeapon() > 0 || InputManager.Instance.PlayerGetNextWeapon()) {
            if (gameManager.wrenchEnabled)
                EquipShotgun();
            else if (gameManager.shotgunEnabled) {
                if (!EquipBomb())
                    EquipWrench();
            } else if (gameManager.bombEnabled)
                EquipWrench();
            Debug.Log("Next Weapon");
            return;
        }
        if (InputManager.Instance.PlayerGetScrollDownWeapon() > 0) {
            if (gameManager.wrenchEnabled) {
                if (!EquipBomb())
                    EquipShotgun();
            } else if (gameManager.shotgunEnabled)
                EquipWrench();
            else if (gameManager.bombEnabled)
                EquipShotgun();
            Debug.Log("Previous Weapon");
            return;
        }
    }


    private void EquipWrench() {
        //if (!CheckWeaponsOnCooldown()) {
        wrench.SetActive(true);
        gameManager.wrenchEnabled = true;

        shotgun.SetActive(false);
        gameManager.shotgunEnabled = false;

        if (bomb && !bomb.GetComponent<Bomb>().IsThrown) {
            bomb.SetActive(false);
            gameManager.bombEnabled = false;
        }

        wrench.GetComponent<Wrench>().wrenchAnimation.Play("TakeOutWrench");
        //}
    }

    private void EquipShotgun() {
        //if (!CheckWeaponsOnCooldown()) {
        wrench.SetActive(false);
        gameManager.wrenchEnabled = false;

        shotgun.SetActive(true);
        gameManager.shotgunEnabled = true;

        if (bomb && !bomb.GetComponent<Bomb>().IsThrown) {
            bomb.SetActive(false);
            gameManager.bombEnabled = false;
        }
        shotgun.GetComponent<Shotgun>().shotgunAnimation.Play("TakeOutShotgun");
        //}
    }

    private bool EquipBomb() {
        if (bomb && !bomb.GetComponent<Bomb>().IsThrown) {
            wrench.SetActive(false);
            gameManager.wrenchEnabled = false;

            shotgun.SetActive(false);
            gameManager.shotgunEnabled = false;

            bomb.SetActive(true);
            gameManager.bombEnabled = true;
            bomb.GetComponent<Bomb>().bombAnimation.Play("TakeOutBomb");
            return true;
        }
        return false;
    }

    public void TakeDamage() {
        health--;
        playerTakeDamage.Invoke();
    }
}
