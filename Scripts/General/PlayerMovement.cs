using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    private float desiredMoveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float gravityMultiplier = 1f;
    private bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    private bool isCrouching;
    private bool crouchSoundCooldown;
    public float crouchSoundCooldownTime = 0.5f;
    private bool landingSoundCooldown;
    public float landingSoundCooldownTime = 0.5f;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;
    [SerializeField] private bool wasGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 45f;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    public float stepHeight = 0.5f;
    public float stepSmooth = 0.1f;

    [Header("View Bobbing")]
    public float bobbingAmount = 0.05f;
    public float bobbingSpeed = 14f;
    private float defaultYPos;
    private float bobbingTimer;

    [Header("Audio")]
    public AudioClip footstepSound;
    public AudioClip jumpSound;
    public AudioClip landingSound;
    public AudioClip crouchSound;
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource oneShotAudioSource;
    private float currentAudioTime;
    public float maxVolume = 1.0f;
    public float minVolume = 0.5f;
    public float maxPitch = 1.5f;
    public float minPitch = 0.8f;

    [Header("Moving Platform Handling")]
    private Transform platform; // Platform the player is standing on
    private Vector3 platformLastPosition; // Last position of the platform
    private Vector3 platformVelocity; // Velocity of the platform

    [Header("Landing Customization")]
    public float fallDurationThreshold = 1f;  // Time after which landing sound should play
    [SerializeField] private float fallDuration;
    [SerializeField] private bool jumpTriggered;

    public Transform orientation;
    public Transform cameraTransform;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;
    public Vector3 velocity;

    [SerializeField] private CharacterController controller;

    public MovementState state;
    public enum MovementState
    {
        freeze,
        unlimited,
        walking,
        sprinting,
        crouching,
        air
    }

    public bool freeze;
    public bool unlimited;
    public bool restricted;
    public bool ontutorial;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        readyToJump = true;

        startYScale = transform.localScale.y;

        defaultYPos = cameraTransform.localPosition.y;
    }

    private void Update()
    {
        grounded = controller.isGrounded;

        MyInput();
        SpeedControl();
        StateHandler();
        HandleViewBobbing();

        if (grounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            HandleLandingSound();
        }
        else
        {
            velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

            // Increment fall duration when in the air
            fallDuration += Time.deltaTime;
        }

        HandleFootsteps();

        // If the player is standing on a moving platform, calculate its velocity
        if (platform != null)
        {
            platformVelocity = (platform.position - platformLastPosition) / Time.deltaTime;
            platformLastPosition = platform.position;
        }
        else
        {
            platformVelocity = Vector3.zero;
        }

        // Update `wasGrounded` after the landing logic has executed
        wasGrounded = grounded;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            jumpTriggered = true;  // Set jumpTriggered to true after jumping
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey) && grounded)
        {
            if (isCrouching)
            {
                if (CanStandUp())
                {
                    transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                    isCrouching = false;

                    if (!crouchSoundCooldown)
                    {
                        oneShotAudioSource.PlayOneShot(crouchSound);
                        StartCoroutine(CrouchSoundCooldown());
                    }
                }
            }
            else
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                isCrouching = true;

                if (!crouchSoundCooldown)
                {
                    oneShotAudioSource.PlayOneShot(crouchSound);
                    StartCoroutine(CrouchSoundCooldown());
                }
            }
        }
    }

    private bool CanStandUp()
    {
        float rayLength = startYScale - crouchYScale + 1f;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.up, out hit, rayLength))
        {
            return false;
        }

        return true;
    }

    private void StateHandler()
    {
        if (freeze)
        {
            state = MovementState.freeze;
            desiredMoveSpeed = 0f;
        }
        else if (unlimited)
        {
            state = MovementState.unlimited;
            desiredMoveSpeed = 999f;
        }
        else if (isCrouching)
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        if (restricted) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Add platform velocity if standing on a moving platform
        Vector3 platformMotion = platform != null ? platformVelocity : Vector3.zero;

        controller.Move((moveDirection.normalized * moveSpeed + velocity + platformMotion) * Time.deltaTime);
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (controller.velocity.magnitude > moveSpeed)
                velocity = velocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(velocity.x, 0f, velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                velocity = new Vector3(limitedVel.x, velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
        oneShotAudioSource.PlayOneShot(jumpSound);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, controller.height / 2 + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private void HandleViewBobbing()
    {
        if (grounded && (horizontalInput != 0 || verticalInput != 0))
        {
            bobbingTimer += Time.deltaTime * bobbingSpeed;
            float bobbingOffset = Mathf.Sin(bobbingTimer) * bobbingAmount;
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, defaultYPos + bobbingOffset, cameraTransform.localPosition.z);
        }
        else
        {
            bobbingTimer = 0;
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, Mathf.Lerp(cameraTransform.localPosition.y, defaultYPos, Time.deltaTime * bobbingSpeed), cameraTransform.localPosition.z);
        }
    }

    private void HandleFootsteps()
    {
        float currentSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;

        if (grounded && !ontutorial && currentSpeed > 0)
        {
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.pitch = Random.Range(minPitch, maxPitch);
                footstepAudioSource.volume = Random.Range(minVolume, maxVolume);
                footstepAudioSource.PlayOneShot(footstepSound);
            }
        }
        else
        {
            footstepAudioSource.Stop();
        }
    }

    private void HandleLandingSound()
    {
        // If the player has landed and has been falling for longer than the threshold
        if (!wasGrounded && grounded && fallDuration >= fallDurationThreshold && !jumpTriggered)
        {
            if (!landingSoundCooldown)
            {
                oneShotAudioSource.PlayOneShot(landingSound);
                StartCoroutine(LandingSoundCooldown());
            }
        }

        // Reset fall duration if grounded
        if (grounded)
        {
            fallDuration = 0f;
            jumpTriggered = false; // Reset jump state on landing
        }
    }

    IEnumerator CrouchSoundCooldown()
    {
        crouchSoundCooldown = true;
        yield return new WaitForSeconds(crouchSoundCooldownTime);
        crouchSoundCooldown = false;
    }

    IEnumerator LandingSoundCooldown()
    {
        landingSoundCooldown = true;
        yield return new WaitForSeconds(landingSoundCooldownTime);
        landingSoundCooldown = false;
    }

    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    if (hit.gameObject.CompareTag("MovingPlatform"))
    //    {
    //        platform = hit.transform;
    //        platformLastPosition = platform.position;
    //    }
    //    else
    //    {
    //        platform = null;
    //    }
    //}

public IEnumerator DisableMovementTemporarily(float delay)
    {
        restricted = true; // Prevent movement
        yield return new WaitForSeconds(delay);
        restricted = false; // Allow movement again
    }
}
