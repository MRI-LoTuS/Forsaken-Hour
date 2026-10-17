using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float crouchSpeed = 3f;


    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -19.62f;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private LayerMask ceilingMask = ~0;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float groundCheckDistance = 0.3f;

    [Header("Reference")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference sprintAction;

    private CharacterController _characterController;
    private Vector2 _moveInput;
    private Vector3 _verticalVelocity;
    private bool _jumpRequested;
    private bool _crouchHeld;
    private bool _isCrouching;
    private bool _sprintHeld;

    private float _standHeight;
    private float _feetOffset;   // Jarak dari pivot ke dasar kapsul
    private float _cameraStandY;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();

        _standHeight = _characterController.height;
        _feetOffset = _characterController.center.y - _standHeight / 2f;

        if (cameraRoot != null)
        {
            _cameraStandY = cameraRoot.localPosition.y;
        }
    }
    private void OnEnable()
    {
        // Mengaktifkan action dan mendaftarkan callback
        moveAction.action.Enable();
        moveAction.action.performed += OnMovePerformed;
        moveAction.action.canceled += OnMoveCanceled;

        jumpAction.action.Enable();
        jumpAction.action.performed += OnJumpPerformed;

        crouchAction.action.Enable();
        crouchAction.action.performed += OnCrouchPerformed;
        crouchAction.action.canceled += OnCrouchCanceled;

        sprintAction.action.Enable();
        sprintAction.action.performed += OnSprintPerformed;
        sprintAction.action.canceled += OnSprintCanceled;

    }

    private void OnDisable()
    {
        // Menonaktifkan action dan menghapus callback untuk mencegah memory leak
        moveAction.action.performed -= OnMovePerformed;
        moveAction.action.canceled -= OnMoveCanceled;
        moveAction.action.Disable();

        jumpAction.action.performed -= OnJumpPerformed;
        jumpAction.action.Disable();

        crouchAction.action.performed -= OnCrouchPerformed;
        crouchAction.action.canceled -= OnCrouchCanceled;
        crouchAction.action.Disable();

        sprintAction.action.performed -= OnSprintPerformed;
        sprintAction.action.canceled -= OnSprintCanceled;
        sprintAction.action.Disable();
    }

    // Fungsi callback saat tombol movement ditekan
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    // Fungsi callback saat tombol movement dilepas
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        _jumpRequested = true;
    }

    // Fungsi callback saat tombol crouch ditahan / dilepas
    private void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        _crouchHeld = true;
    }

    private void OnCrouchCanceled(InputAction.CallbackContext context)
    {
        _crouchHeld = false;
    }
    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        _sprintHeld = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        _sprintHeld = false;
    }


    void Update()
    {
        HandleCrouch();
        HandleMovement();
    }

    private void HandleCrouch()
    {
        if (_crouchHeld)
        {
            _isCrouching = true;
        }
        else if (_isCrouching && CanStandUp())
        {
            _isCrouching = false;
        }

        float targetHeight = _isCrouching ? crouchHeight : _standHeight;
        float newHeight = Mathf.Lerp(_characterController.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        if (Mathf.Abs(newHeight - targetHeight) < 0.001f) { newHeight = targetHeight; }

        _characterController.height = newHeight;
        Vector3 center = _characterController.center;
        center.y = _feetOffset + newHeight / 2f;
        _characterController.center = center;

        if (cameraRoot != null) 
        {
            Vector3 camPos = cameraRoot.localPosition;
            camPos.y = _cameraStandY - (_standHeight - newHeight);
            cameraRoot.localPosition = camPos;
        }
    }

    private bool CanStandUp()
    {
        float distance = _standHeight - _characterController.height;
        if (distance <= 0f) return true;

        // Cek ke atas dari kepala player saat ini
        float radius = _characterController.radius;
        Vector3 origin = transform.position + Vector3.up * (_characterController.center.y + _characterController.height / 2f - radius);

        return !Physics.SphereCast(origin, radius * 0.95f, Vector3.up, out _, distance, ceilingMask, QueryTriggerInteraction.Ignore);
    }

    private float GetCurrentSpeed()
    {
        if (_isCrouching) return crouchSpeed;
        if (_sprintHeld && _moveInput.y > 0f) return runSpeed;
        return walkSpeed;
    }

    private void HandleMovement()
    {
        //Debug.Log("Grounded: " + _characterController.isGrounded);

        //if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f))
        //    Debug.Log("Di bawah player: " + hit.collider.name + " | jarak: " + hit.distance);
        //else
        //    Debug.Log("TIDAK ADA collider di bawah player!");

        bool isGrounded = _characterController.isGrounded;
         
        if (isGrounded && _verticalVelocity.y < 0) 
        {
            _verticalVelocity.y = -2;
        }

        Vector3 move = Vector3.zero;


        if (cameraTransform != null) 
        {
            // Mengambil arah berdasarkan rotasi kamera
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            move = forward * _moveInput.y + right * _moveInput.x;
        }




        float currentSpeed = GetCurrentSpeed();

        if (_jumpRequested && isGrounded && !_isCrouching)
            _verticalVelocity.y = MathF.Sqrt(jumpHeight * -2f * gravity);
        _jumpRequested = false;

        _verticalVelocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = move * currentSpeed + _verticalVelocity;

        _characterController.Move(finalMove * Time.deltaTime);
    }

    //private void OnJumpPerformed(InputAction.CallbackContext context)
    //{
    //    Debug.Log("Jump ditekan");
    //    _jumpRequested = true;
    //}
}