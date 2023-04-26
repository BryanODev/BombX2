using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerController : MonoBehaviour
{
    PlayerInputs playerInput;
    public Camera playerCamera;

    ISelectable holdingObject;

    [Inject] IGameModeState gameModeState;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInputs();
        playerInput.Enable();
        SetUpPlayerInputs();
    }

    private void OnEnable()
    {
        playerInput.Enable();
        SetUpPlayerInputs();
    }

    public void SetUpPlayerInputs() 
    {
    }

    private void Update()
    {
        if (gameModeState.GameEnded || !gameModeState.GameStarted) { return; }

        if (playerInput.MainControls.TouchPress.WasPressedThisFrame()) 
        {
            Debug.Log("Player Pick Bomb");

            Vector2 screenPosition = playerInput.MainControls.TouchPosition.ReadValue<Vector2>();
            TrySelectingObject(screenPosition);
        }

        if (playerInput.MainControls.TouchPress.IsInProgress()) 
        {
            Vector3 holdingObjectPos = playerCamera.ScreenToWorldPoint(playerInput.MainControls.TouchPosition.ReadValue<Vector2>());
            holdingObjectPos.z = -5;

            if (holdingObject != null && holdingObject.IsSelectable)
            {
                holdingObject.SelectableRigidbody.velocity = (holdingObjectPos - holdingObject.SelectableTransform.position) * 10;
                //holdingObject.selectableTransform.position = holdingObjectPos;
            }
        }

        if (playerInput.MainControls.TouchPress.WasReleasedThisFrame()) 
        {
            Debug.Log("Player Drop Bomb");

            Vector3 holdingObjectPos = playerCamera.ScreenToWorldPoint(playerInput.MainControls.TouchPosition.ReadValue<Vector2>());
            holdingObjectPos.z = 0;

            if (holdingObject != null && holdingObject.IsSelectable)
            {
                holdingObject.SelectableRigidbody.velocity = (holdingObjectPos - holdingObject.SelectableTransform.position) * 10;
                //holdingObject.selectableTransform.position = holdingObjectPos;
                holdingObject.OnDiselect();
                holdingObject = null;
            }
        }
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void TrySelectingObject(Vector2 screenPosition)
    {
        Vector3 circlePos = playerCamera.ScreenToWorldPoint(screenPosition);
        circlePos.z = 0;

        Collider2D collider = Physics2D.OverlapCircle(circlePos, 0.25f);

        if (collider) 
        {
            ISelectable selectable = collider.GetComponent<ISelectable>();

            if (selectable != null && selectable.IsSelectable)
            {
                GrabObject(selectable);
            }
        }
    }

    public void GrabObject(ISelectable selectableObject) 
    {
        holdingObject = selectableObject;
        holdingObject.OnSelect();
    }
}
