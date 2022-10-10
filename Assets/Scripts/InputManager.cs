using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; set; }

    private PlayerControls playerControls;

    public void Awake()
    {
        playerControls = new PlayerControls();

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return playerControls.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return playerControls.Player.Look.ReadValue<Vector2>() * 0.2f;
    }

    public bool PlayerJumpedThisFrame()
    {
        return playerControls.Player.Jump.triggered;
    }

    public bool PlayerGetFireInput()
    {
        return playerControls.Player.Fire.triggered;
    }
    
    public bool PlayerGetNoClipInput()
    {
        return playerControls.Player.NoClip.triggered;
    }

    public bool PlayerGetSelectWrench() {
        return playerControls.Player.SelectWrench.triggered;
    }

    public bool PlayerGetSelectShotgun() {
        return playerControls.Player.SelectShotgun.triggered;
    }

    public bool PlayerGetSelectBomb() {
        return playerControls.Player.SelectBomb.triggered;
    }

    public float PlayerGetScrollUpWeapon() {
        return playerControls.Player.ScrollUpWeapon.ReadValue<float>();
    }

    public float PlayerGetScrollDownWeapon() {
        return playerControls.Player.ScrollDownWeapon.ReadValue<float>();
    }

    public bool PlayerGetNextWeapon() {
        return playerControls.Player.NextWeapon.triggered;
    }

    public bool PlayerPause()
    {
        return playerControls.Player.Pause.triggered;
    }
}
