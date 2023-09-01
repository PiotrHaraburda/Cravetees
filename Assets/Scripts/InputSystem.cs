using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    public float rotationSpeed;
    public CharacterController characterController;
    
    private Animator animator;
    private InputActions input;
    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 finalMovementVector;
    private bool movementPressed;
    private bool runPressed;
    private bool jumpPressed;
    
    float gravity = -9.8f;
    float groundedGravity = -0.1f;
    private bool isJumping;
    private float initialJumpVelocity;
    private float maxJumpHeight=3f;
    private float maxJumpTime=1f;
    
    
    

    void Awake()
    {
        input = new InputActions();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        input.Player.Movement.started += onMovementInput;
        input.Player.Movement.performed += onMovementInput;
        input.Player.Movement.canceled += onMovementInput;
        
        input.Player.Run.started += ctx => runPressed = ctx.ReadValueAsButton();
        input.Player.Run.canceled += ctx => runPressed = ctx.ReadValueAsButton();
        
        input.Player.Jump.started += ctx => jumpPressed=ctx.ReadValueAsButton();
        input.Player.Jump.canceled += ctx => jumpPressed=ctx.ReadValueAsButton();

        setupJumpVariables();
    }

    void setupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);

        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void onMovementInput(InputAction.CallbackContext ctx)
    {
        currentMovementInput = ctx.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        movementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    public void Update()
    {
        handleRotation();
        handleMovement();
        finalMovementVector = ConvertToCameraLook(currentMovement);
        characterController.Move( finalMovementVector* Time.deltaTime);
        handleGravity();
        handleJump();
        if (isJumping)
        {
            finalMovementVector.y = 0;
            if (runPressed)
            {
                finalMovementVector *= 3;
                characterController.Move( finalMovementVector* Time.deltaTime);
            }
            else
            {
                finalMovementVector *= 2;
                characterController.Move( finalMovementVector* Time.deltaTime);
            }
        }
    }

    void handleJump()
    {
        if (!isJumping && characterController.isGrounded && jumpPressed)
        {
            animator.SetBool("isJumping",true);
            isJumping = true;
            handleJumpSpeed();
            currentMovement.y = initialJumpVelocity*0.5f;
        }
        else if(isJumping&&characterController.isGrounded && !jumpPressed)
        {
            isJumping = false;
        }
    }

    void handleJumpSpeed()
    {
        
    }
    
    Vector3 ConvertToCameraLook(Vector3 vectorToRotate)
    {
        float currentY = vectorToRotate.y;
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        
        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        Vector3 cameraForwardZMultiplied = vectorToRotate.z * cameraForward;
        Vector3 cameraRightXMultiplied = vectorToRotate.x * cameraRight;

        Vector3 convertedVector = cameraForwardZMultiplied + cameraRightXMultiplied;
        convertedVector.y = currentY;
        return convertedVector;
    }

    public void handleGravity()
    {
        if (characterController.isGrounded)
        {
            animator.SetBool("isJumping",false);
            currentMovement.y = groundedGravity;
        }
        else
        {
            float prevYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity*Time.deltaTime);
            float nextYVelocity = (prevYVelocity + newYVelocity) * 0.5f;
            currentMovement.y = nextYVelocity;
        }
    }

    void handleMovement()
    {
        bool isRunning = animator.GetBool("isRunning");
        bool isWalking = animator.GetBool("isWalking");

        if (movementPressed && !isWalking)
        {
            animator.SetBool("isWalking",true);
        }
        else if (!movementPressed && isWalking)
        {
            animator.SetBool("isWalking",false);
        }
        
        if ((movementPressed && runPressed)&&!isRunning)
        {
            animator.SetBool("isRunning",true);
        }
        else if ((!movementPressed || !runPressed)&&isRunning)
        {
            animator.SetBool("isRunning",false);
        }
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = finalMovementVector.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = finalMovementVector.z;

        Quaternion currentRotation = transform.rotation;

        if (movementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation=Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed*Time.deltaTime);
        }
    }

    void OnEnable()
    {
        input.Player.Enable();
    }
    
    void OnDisable()
    {
        input.Player.Disable();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
