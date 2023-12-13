using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class Controls : NetworkBehaviour
{
    [Header("Variables")]
    [SerializeField, ReadOnly] private Rigidbody rb;
    [SerializeField, ReadOnly] private State state;
    [SerializeField] private GameObject camHolder;

    [Header("Functionality Option")]
    [SerializeField] private bool enableMovement = true;
    [SerializeField] private bool enableJump = true;

    [Header("Camera Parameters")]
    [SerializeField, Range(0.01f, 10.00f)] private float sensitivity = 0.1f;
    [SerializeField] private float lowerLookBoundary = -90f;
    [SerializeField] private float upperLookBoundary = 90f;
    [SerializeField, ReadOnly] private float lookRotation;
    private Vector2 lookInputVector;

    [Header("Movement Parameters")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float maxForce = 1f;
    private Vector2 moveInputVector;


    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        state = GetComponent<State>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        MoveHandler();
    }

    private void LateUpdate()
    {
        if (!IsOwner)
            return;

        LookHandler();
    }

    #region Look
    public void Look(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;
        lookInputVector = context.ReadValue<Vector2>();
    }
    private void LookHandler()
    {
        transform.Rotate(Vector3.up * lookInputVector.x * sensitivity);
        lookRotation += -lookInputVector.y * sensitivity;
        lookRotation = Mathf.Clamp(lookRotation, lowerLookBoundary, upperLookBoundary);
        camHolder.transform.eulerAngles = new Vector3(lookRotation, camHolder.transform.eulerAngles.y, camHolder.transform.eulerAngles.z);
    }
    public override void OnNetworkSpawn()
    {
        camHolder.SetActive(IsOwner);
        base.OnNetworkSpawn();
    }
    #endregion

    #region Move
    public void Movement(InputAction.CallbackContext context)
    {
        if (!enableMovement || !IsOwner)
            return;

        moveInputVector = context.ReadValue<Vector2>();
    }
    private void MoveHandler()
    {
        if (!state.isGrounded)
            return;

        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = new Vector3(moveInputVector.x, 0, moveInputVector.y);
        targetVelocity *= speed;
        targetVelocity = transform.TransformDirection(targetVelocity);
        Vector3 velocityChange = targetVelocity - currentVelocity;
        velocityChange = new Vector3(velocityChange.x, 0f, velocityChange.z);
        Vector3.ClampMagnitude(velocityChange, maxForce);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
    #endregion

    #region Jump
    public void Jump(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;

        if (enableJump && context.performed && state.isGrounded)
        {
            Debug.Log("Jump: " + context.phase);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }
    #endregion
}
