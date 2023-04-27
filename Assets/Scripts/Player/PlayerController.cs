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

    public float itemDragSpeed = 10;
    public Vector2 lastTouchPosition = Vector2.zero;
    public Vector3 lastTouchPositionInWorld = Vector2.zero;

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
        HandlePlayerInput();
    }

    public void HandlePlayerInput()
    {
        if (gameModeState.GameEnded || !gameModeState.GameStarted) { return; }

        lastTouchPosition = playerInput.MainControls.TouchPosition.ReadValue<Vector2>();
        lastTouchPositionInWorld = playerCamera.ScreenToWorldPoint(lastTouchPosition);
        lastTouchPositionInWorld.z = 0;

        if (playerInput.MainControls.TouchPress.WasPressedThisFrame())
        {
            Debug.Log("Player Pick Bomb");

            TrySelectingObject();
        }

        if (holdingObject != null)
        {
            //if (!holdingObject.IsSelectable) 
            //{
            //    DeselectObject();
            //}

            DragObject();
        }

        if (playerInput.MainControls.TouchPress.WasReleasedThisFrame())
        {
            DeselectObject();
        }
    }

    private void TrySelectingObject()
    {
        Collider2D collider = Physics2D.OverlapCircle(lastTouchPositionInWorld, 0.25f);

        if (collider)
        {
            ISelectable selectable = collider.GetComponent<ISelectable>();

            if (selectable != null && selectable.IsSelectable)
            {
                SelectObject(selectable);
            }
        }
    }

    public void SelectObject(ISelectable selectableObject)
    {
        Debug.Log("Grab " + selectableObject.SelectableTransform.name);
        holdingObject = selectableObject;
        holdingObject.OnSelect();
    }

    void DragObject() 
    {
        if (holdingObject != null)
        {
            holdingObject.SelectableRigidbody.velocity = (lastTouchPositionInWorld - holdingObject.SelectableTransform.position) * itemDragSpeed;
            //holdingObject.selectableTransform.position = holdingObjectPos;
        }
    }

    void DeselectObject() 
    {
        if(holdingObject == null) { return; }

        holdingObject.OnDiselect();
        holdingObject = null;
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }


}
