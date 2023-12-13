using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Walking,
    Jumping
}
public class State : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] private CapsuleCollider leftFootCollider;
    [SerializeField] private CapsuleCollider rightFootCollider;

    [Header("State")]
    [SerializeField] public bool isGrounded;

    void Start()
    {
        
    }

    void Update()
    {
        if (!IsOwner)
            return;

        isGrounded = CheckGrounded();
    }

    public bool CheckGrounded()
    {
        bool leftFootGrounded = Physics.OverlapCapsule(leftFootCollider.transform.position + leftFootCollider.center,
                                                       leftFootCollider.transform.position - leftFootCollider.center,
                                                       leftFootCollider.radius, LayerMask.GetMask("Floor")).Length > 0;

        bool rightFootGrounded = Physics.OverlapCapsule(rightFootCollider.transform.position + rightFootCollider.center,
                                                        rightFootCollider.transform.position - rightFootCollider.center,
                                                        rightFootCollider.radius, LayerMask.GetMask("Floor")).Length > 0;

        // Set isGrounded to true if either foot is grounded
        return leftFootGrounded || rightFootGrounded;
    }
}
