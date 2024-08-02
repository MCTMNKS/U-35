using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XEntity.InventoryItemSystem;
using KinematicCharacterController;
using System;

namespace KinematicCharacterController.Examples
{
    public enum CharacterState
    {
        Default,
    }

    public enum OrientationMethod
    {
        TowardsCamera,
        TowardsMovement,
    }

    public struct PlayerCharacterInputs
    {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool JumpDown;
        public bool CrouchDown;
        public bool CrouchUp;
        public bool Sprint; // Sprinting mechanic added
    }

    public struct AICharacterInputs
    {
        public Vector3 MoveVector;
        public Vector3 LookVector;
    }

    public enum BonusOrientationMethod
    {
        None,
        TowardsGravity,
        TowardsGroundSlopeAndGravity,
    }

    public class ExampleCharacterController : MonoBehaviour, ICharacterController
    {
        public KinematicCharacterMotor Motor;

        [Header("Stable Movement")]
        public float MaxStableMoveSpeed = 10f;
        public float StableMovementSharpness = 15f;
        public float OrientationSharpness = 10f;
        public OrientationMethod OrientationMethod = OrientationMethod.TowardsCamera;

        [Header("Air Movement")]
        public float MaxAirMoveSpeed = 15f;
        public float AirAccelerationSpeed = 15f;
        public float Drag = 0.1f;

        [Header("Jumping")]
        public bool AllowJumpingWhenSliding = false;
        public float JumpUpSpeed = 10f;
        public float JumpScalableForwardSpeed = 10f;
        public float JumpPreGroundingGraceTime = 0f;
        public float JumpPostGroundingGraceTime = 0f;
        // Double jump variables (Furkan)
        private bool _canDoubleJump = false;
        private bool _doubleJumpUsed = false;
        private bool _doubleJumpEnabled = false; // Double jump will be activated when the player collects the ring

        [Header("Dash")] // Added for dash mechanic (Furkan)
        public float DashSpeed = 50f; // The speed at which the character will dash
        public float DashDistance = 5f; // The distance the character will dash
        private bool _isDashing = false; // Whether the character is currently dashing
        private float _dashTimer = 3f; // A timer for the dash duration
        private bool _dashEnabled = false; // Dash will be activated when the player collects the ring
        private float _lastDashTime = -2f; // Initialize to a value more than 3 seconds ago (dash cooldown)
        private float _dashCooldown = 2f; // The cooldown duration in seconds (dash cooldown)

        [Header("Wall Interaction")]
        public bool isTouchingWall = false;
        public bool isWallStickActive = false;
        public float wallStickTimer = 0f;
        public float wallStickDuration = 5f; // The character will stick to the wall for 5 seconds
        public float climbingSpeed = 5f; // The speed at which the character climbs the wall
        public float jumpForce = 5f; // Adjust this value as needed

        [Header("Misc")]
        public List<Collider> IgnoredColliders = new List<Collider>();
        public BonusOrientationMethod BonusOrientationMethod = BonusOrientationMethod.None;
        public float BonusOrientationSharpness = 10f;
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        public Transform MeshRoot;
        public Transform CameraFollowPoint;
        public float CrouchedCapsuleHeight = 1f;

        //audio
        public AudioClip[] FootstepAudioClips;
        public AudioSource audioSource;
        private float _timeSinceLastFootstep;
        public float FootstepSoundInterval = 0.5f; // Time interval between footstep sounds



        public CharacterState CurrentCharacterState { get; private set; }

        private Collider[] _probedColliders = new Collider[8];
        private RaycastHit[] _probedHits = new RaycastHit[8];
        private Vector3 _moveInputVector;
        private Vector3 _lookInputVector;
        private bool _jumpRequested = false;
        private bool _jumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0f;
        private Vector3 _internalVelocityAdd = Vector3.zero;
        private bool _shouldBeCrouching = false;
        private bool _isCrouching = false;
        private bool _isSprinting = false; // For storing sprinting state
        private Animator animator; // For animator state
        private int _animIDJump;
        private int _animIDFreeFall;
        public ParticleSystem crouchParticleEffect;
        private Vector3 originalCharacterScale;
        private Vector3 originalCapsuleDimensions;
        // Added this variable to store the dash direction
        private Vector3 _dashDirection;
        // for squashing and stretching
        private Vector3 originalScale;
        private bool isReturningToOriginalScale;
        private bool isInAir;

        private bool _cursorShouldBeLocked = true; // for esc key toggle



        // For scaling the mesh when crouching
        private IEnumerator ScaleOverTime(Transform target, Vector3 targetScale, float duration)
        {
            Vector3 initialScale = target.localScale;
            float time = 0;

            while (time < duration)
            {
                target.localScale = Vector3.Lerp(initialScale, targetScale, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            target.localScale = targetScale;

            // Calculate the ratio of the new scale to the original scale
            Vector3 scaleRatio = new Vector3(
                target.localScale.x / originalCharacterScale.x,
                target.localScale.y / originalCharacterScale.y,
                target.localScale.z / originalCharacterScale.z
            );

            // Adjust the capsule dimensions based on the scale ratio
            Vector3 newCapsuleDimensions = new Vector3(
                originalCapsuleDimensions.x * scaleRatio.x,
                originalCapsuleDimensions.y * scaleRatio.y,
                originalCapsuleDimensions.z * scaleRatio.z
            );

            Motor.SetCapsuleDimensions(newCapsuleDimensions.x, newCapsuleDimensions.y, newCapsuleDimensions.z);
        }


        private Vector3 lastInnerNormal = Vector3.zero;
        private Vector3 lastOuterNormal = Vector3.zero;
        private void Start()
        {
            // Store the original scale of the character and the capsule dimensions
            originalCharacterScale = MeshRoot.localScale;
            originalCapsuleDimensions = new Vector3(0.4f, 3f, 1.5f);
            _animIDJump = Animator.StringToHash("IsJumping");
            _animIDFreeFall = Animator.StringToHash("IsFreeFalling");
            originalScale = MeshRoot.localScale; // Store the original scale
            audioSource = GetComponent<AudioSource>();

            // Subscribe to the onBlueConsumed event
            ItemManager.Instance.onBlueConsumed.AddListener(OnBlueConsumed);

        }
        private void OnBlueConsumed()
        {
            // Change the jump up speed to 20
            JumpUpSpeed = 20f;
            JumpScalableForwardSpeed = 20f;
        }
        private void OnDestroy()
        {
            // Unsubscribe from the onBlueConsumed event when the object is destroyed
            ItemManager.Instance.onBlueConsumed.RemoveListener(OnBlueConsumed);
        }
        private void Awake()
        {
            // Handle initial state
            TransitionToState(CharacterState.Default);

            // Assign the characterController to the motor
            Motor.CharacterController = this;

            animator = GetComponent<Animator>(); // Handle Animator
        }

        // Animator states and interpolation (smooth transition between states)
        private void Update()
        {
            // Toggle cursor lock state when the Escape key is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _cursorShouldBeLocked = !_cursorShouldBeLocked;
            }

            // Apply cursor lock state and visibility
            if (_cursorShouldBeLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            // Cast a ray from the character's position in the direction they are facing
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
            {
                // If the ray hits a wall, set isTouchingWall to true
                if (hit.collider.CompareTag("Wall"))
                {
                    isTouchingWall = true;
                }
                else
                {
                    isTouchingWall = false;
                }
            }
            else
            {
                isTouchingWall = false;
            }
        
        // Calculate the target speed as a percentage of the max speed
        float targetSpeedPercent = _moveInputVector.magnitude / MaxStableMoveSpeed;

            // Increase targetSpeedPercent when sprinting and character is moving
            if (_isSprinting && _moveInputVector.sqrMagnitude > 0)
            {
                targetSpeedPercent = 10f; // Or any value greater than the threshold for running
            }

            // Get the current speed percent from the animator
            float currentSpeedPercent = animator.GetFloat("Speed");

            // Define a speed at which the character changes its speed
            float speedIncreaseRate = 10f; // Adjust this value as needed
            float speedDecreaseRate = 5f; // Adjust this value as needed

            // If targetSpeedPercent is less than a small threshold, set it directly to zero
            if (targetSpeedPercent < 0.1f)
            {
                animator.SetFloat("Speed", 0f);
            }
            else
            {
                // If targetSpeedPercent is less than current speed, use speedDecreaseRate, else use speedIncreaseRate
                float speedChangeRate = targetSpeedPercent < currentSpeedPercent ? speedDecreaseRate : speedIncreaseRate;

                // Smoothly interpolate between the current and target speed percent
                float smoothedSpeedPercent = Mathf.Lerp(currentSpeedPercent, targetSpeedPercent, Time.deltaTime * speedChangeRate);

                // Set the "Speed" parameter in the Animator
                animator.SetFloat("Speed", smoothedSpeedPercent);
            }
        }

        private IEnumerator PlayParticleEffect(ParticleSystem particleEffect, float duration)
        {
            particleEffect.Play();

            yield return new WaitForSeconds(duration);

            particleEffect.Stop();
        }


        void HandleWallStick()
        {
            if (isTouchingWall && Input.GetKeyDown(KeyCode.F))
            {
                isWallStickActive = true;
                wallStickTimer = wallStickDuration;
            }

            if (isWallStickActive)
            {
                // Allow the character to move upwards
                if (Input.GetKey(KeyCode.W))
                {
                    _moveInputVector.y = 1;
                }
                else
                {
                    _moveInputVector.y = 0;
                }

                // Cast a ray from the character's position upwards
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.up, out hit, 1f))
                {
                    // If the ray hits the wall, stop the upward movement
                    if (hit.collider.CompareTag("Wall"))
                    {
                        isWallStickActive = false;
                        _internalVelocityAdd = Vector3.zero; // Reset the internal velocity add
                    }
                }

                // If the space key is pressed, exit the wall sticking state and prepare to jump
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isWallStickActive = false;
                    _jumpRequested = true;
                }

                // Decrease the timer and deactivate wall stick if time is up
                wallStickTimer -= Time.deltaTime;
                if (wallStickTimer <= 0)
                {
                    isWallStickActive = false;
                    _internalVelocityAdd = Vector3.zero; // Reset the internal velocity add
                }
            }
        }


        void OnCollisionEnter(Collision collision)
        {
            // Check if the character has collided with the top of the wall
            if (collision.gameObject.CompareTag("Wall"))
            {
                isWallStickActive = false;
            }
        }






            /// <summary>
            /// Handles movement state transitions and enter/exit callbacks
            /// </summary>
            public void TransitionToState(CharacterState newState)
        {
            CharacterState tmpInitialState = CurrentCharacterState;
            OnStateExit(tmpInitialState, newState);
            CurrentCharacterState = newState;
            OnStateEnter(newState, tmpInitialState);
        }

        /// <summary>
        /// Event when entering a state
        /// </summary>
        public void OnStateEnter(CharacterState state, CharacterState fromState)
        {
            switch (state)
            {
                case CharacterState.Default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// Event when exiting a state
        /// </summary>
        public void OnStateExit(CharacterState state, CharacterState toState)
        {
            switch (state)
            {
                case CharacterState.Default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// This is called every frame by ExamplePlayer in order to tell the character what its inputs are
        /// </summary>
        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            // Handle dash input
            if (_dashEnabled && Input.GetKeyDown(KeyCode.R) && !_isDashing && Time.time - _lastDashTime > _dashCooldown)
            {
                _isDashing = true;
                _dashTimer = 0f;
                _lastDashTime = Time.time; // Update the time of the last dash

                // Store the dash direction
                _dashDirection = _moveInputVector;
                // Trigger the dash animation
                animator.SetTrigger("Dash");
            }
            // Clamp input
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

            // Calculate camera direction and rotation on the character plane
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        // Move and look inputs
                        _moveInputVector = cameraPlanarRotation * moveInputVector;

                        switch (OrientationMethod)
                        {
                            case OrientationMethod.TowardsCamera:
                                _lookInputVector = cameraPlanarDirection;
                                break;
                            case OrientationMethod.TowardsMovement:
                                _lookInputVector = _moveInputVector.normalized;
                                break;
                        }

                        //Sprinting input
                        _isSprinting = inputs.Sprint;

                        // Jumping input
                        if (inputs.JumpDown)
                        {
                            _timeSinceJumpRequested = 0f;
                            _jumpRequested = true;
                        }

                        // Crouching input
                        if (inputs.CrouchDown)
                        {
                            _shouldBeCrouching = !_shouldBeCrouching; // Toggle the crouching state

                            if (_shouldBeCrouching && !_isCrouching)
                            {
                                _isCrouching = true;
                                StartCoroutine(ScaleOverTime(MeshRoot, new Vector3(0.4f, 0.4f, 0.4f), 0.25f)); // Scale down over 1 second
                                StartCoroutine(PlayParticleEffect(crouchParticleEffect, 0.5f)); // Play particle effect for 1 second
                            }
                            else if (!_shouldBeCrouching && _isCrouching)
                            {
                                _isCrouching = false;
                                StartCoroutine(ScaleOverTime(MeshRoot, new Vector3(0.8f, 0.8f, 0.8f), 0.25f)); // Scale up over 1 second
                            }
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// This is called every frame by the AI script in order to tell the character what its inputs are
        /// </summary>
        public void SetInputs(ref AICharacterInputs inputs)
        {
            _moveInputVector = inputs.MoveVector;
            _lookInputVector = inputs.LookVector;
        }

        private Quaternion _tmpTransientRot;

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called before the character begins its movement update
        /// </summary>
        public void BeforeCharacterUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its rotation should be right now. 
        /// This is the ONLY place where you should set the character's rotation
        /// </summary>
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        if (_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
                        {
                            // Smoothly interpolate from current to target look direction
                            Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                            // Set the current rotation (which will be used by the KinematicCharacterMotor)
                            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                        }

                        Vector3 currentUp = (currentRotation * Vector3.up);
                        if (BonusOrientationMethod == BonusOrientationMethod.TowardsGravity)
                        {
                            // Rotate from current up to invert gravity
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                        }
                        else if (BonusOrientationMethod == BonusOrientationMethod.TowardsGroundSlopeAndGravity)
                        {
                            if (Motor.GroundingStatus.IsStableOnGround)
                            {
                                Vector3 initialCharacterBottomHemiCenter = Motor.TransientPosition + (currentUp * Motor.Capsule.radius);

                                Vector3 smoothedGroundNormal = Vector3.Slerp(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

                                // Move the position to create a rotation around the bottom hemi center instead of around the pivot
                                Motor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * Motor.Capsule.radius));
                            }
                            else
                            {
                                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                            }
                        }
                        else
                        {
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its velocity should be right now. 
        /// This is the ONLY place where you can set the character's velocity
        /// </summary>
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        // Stretch based on vertical speed only when in the air
                        if (isInAir && !isReturningToOriginalScale && !_isCrouching)
                        {
                            float stretchFactor = 0.8f + Mathf.Clamp(Mathf.Abs(currentVelocity.y) / 5f, 0f, 0.05f);
                            MeshRoot.localScale = new Vector3(0.7f / stretchFactor, stretchFactor, 0.8f / stretchFactor);
                        }
                    
                    if (_isDashing)
                        {
                            // Dash towards the stored dash direction
                            currentVelocity = _dashDirection * DashSpeed;

                            // Increment the dash timer
                            _dashTimer += deltaTime;

                            // Calculate the distance traveled
                            float dashDistanceTraveled = DashSpeed * _dashTimer;

                            // End the dash if the distance has been exceeded
                            if (dashDistanceTraveled >= DashDistance)
                            {
                                _isDashing = false;

                                // Reset the dash animation trigger
                                animator.ResetTrigger("Dash");
                            }
                        }
                        else
                        
                            // Handle wall stick
                            HandleWallStick();

                        // If the character is sticking to the wall, don't apply other forces
                        if (isWallStickActive)
                        {
                            // Allow the character to move upwards
                            if (Input.GetKey(KeyCode.W))
                            {
                                currentVelocity = Motor.CharacterUp * climbingSpeed; // Adjust the climbing speed here
                            }
                            else
                            {
                                currentVelocity = Vector3.zero;
                            }

                            // If a jump is requested, add a force away from the wall and upwards
                            if (_jumpRequested)
                            {
                                currentVelocity += -transform.forward * jumpForce; // Adjust the jump force as needed
                                currentVelocity += Motor.CharacterUp * jumpForce; // Add an upward force
                                _jumpRequested = false;
                            }

                            return;
                        }



                        // Ground movement
                        if (Motor.GroundingStatus.IsStableOnGround)
                        {
                            float currentVelocityMagnitude = currentVelocity.magnitude;

                            Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

                            // Reorient velocity on slope
                            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

                            // Calculate target velocity
                            Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                            Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

                            // Increase speed when sprinting
                            if (_isSprinting)
                            {
                                float sprintMultiplier = 3f; // Adjust this value as needed
                                targetMovementVelocity *= sprintMultiplier;
                            }

                            // If crouching, halve the speed
                            if (_isCrouching)
                            {
                                targetMovementVelocity *= 0.5f;
                            }

                            // Smooth movement Velocity
                            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
                        }
                        // Air movement
                        else
                        {
                            // Add move input
                            if (_moveInputVector.sqrMagnitude > 0f)
                            {
                                Vector3 addedVelocity = _moveInputVector * AirAccelerationSpeed * deltaTime;

                                Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

                                // Limit air velocity from inputs
                                if (currentVelocityOnInputsPlane.magnitude < MaxAirMoveSpeed)
                                {
                                    // clamp addedVel to make total vel not exceed max vel on inputs plane
                                    Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, MaxAirMoveSpeed);
                                    addedVelocity = newTotal - currentVelocityOnInputsPlane;
                                }
                                else
                                {
                                    // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                                    if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                                    {
                                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                                    }
                                }

                                // Prevent air-climbing sloped walls
                                if (Motor.GroundingStatus.FoundAnyGround)
                                {
                                    if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                                    {
                                        Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                                    }
                                }

                                // Apply added velocity
                                currentVelocity += addedVelocity;
                            }

                            // Gravity
                            currentVelocity += Gravity * deltaTime;

                            // Drag
                            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                        }


                        // Handle jumping
                        _jumpedThisFrame = false;
                        _timeSinceJumpRequested += deltaTime;
                        if (_jumpRequested)
                        {
                            // See if we actually are allowed to jump
                            if (!_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
                            {
                                // Calculate jump direction before ungrounding
                                Vector3 jumpDirection = Motor.CharacterUp;
                                if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                                {
                                    jumpDirection = Motor.GroundingStatus.GroundNormal;
                                }

                                // Makes the character skip ground probing/snapping on its next update. 
                                Motor.ForceUnground();

                                // Add to the return velocity and reset jump state
                                currentVelocity += (jumpDirection * JumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                currentVelocity += (_moveInputVector * JumpScalableForwardSpeed);
                                _jumpRequested = false;
                                _jumpConsumed = true;
                                _jumpedThisFrame = true;

                                // Allow double jump since we jumped from the ground
                                _canDoubleJump = true;
                                _doubleJumpUsed = false;
                            }
                            else if (_doubleJumpEnabled && _canDoubleJump && !_doubleJumpUsed) // Check _doubleJumpEnabled here
                            {
                                // Double jump
                                currentVelocity += (Motor.CharacterUp * JumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                _jumpRequested = false;
                                _doubleJumpUsed = true;
                                _jumpedThisFrame = true;
                            }
                        }
                        else
                        {
                            // If we are not jumping, stop the jump animation
                            animator.SetBool(_animIDJump, false);
                        }
                        // Handle free falling
                        if (!Motor.GroundingStatus.IsStableOnGround)
                        {
                            // If we are not grounded, start the free fall animation
                            animator.SetBool(_animIDFreeFall, true);
                        }
                        else
                        {
                            // If we are grounded, stop the free fall animation
                            animator.SetBool(_animIDFreeFall, false);
                        }
                    
                    // Take into account additive velocity
                    if (_internalVelocityAdd.sqrMagnitude > 0f)
                        {
                            currentVelocity += _internalVelocityAdd;
                            _internalVelocityAdd = Vector3.zero;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called after the character has finished its movement update
        /// </summary>

        public void EnableDoubleJump()
        {
            _doubleJumpEnabled = true;
        }
        // Call this function when the player collects the ring
        public void EnableDash()
        {
            _dashEnabled = true;
        }
        // Call this function as well when the player collects the ring

        public void AfterCharacterUpdate(float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        // Handle jump-related values
                        {
                            // Handle jumping pre-ground grace period
                            if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                            {
                                _jumpRequested = false;
                            }

                            if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                            {
                                // If we're on a ground surface, reset jumping values
                                if (!_jumpedThisFrame)
                                {
                                    _jumpConsumed = false;
                                }
                                _timeSinceLastAbleToJump = 0f;
                            }
                            else
                            {
                                // Keep track of time since we were last able to jump (for grace period)
                                _timeSinceLastAbleToJump += deltaTime;
                            }
                        }

                        // Handle uncrouching
                        if (_isCrouching && !_shouldBeCrouching)
                        {
                            // Do an overlap test with the character's standing height to see if there are any obstructions
                            Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                            if (Motor.CharacterOverlap(
                                Motor.TransientPosition,
                                Motor.TransientRotation,
                                _probedColliders,
                                Motor.CollidableLayers,
                                QueryTriggerInteraction.Ignore) > 0)
                            {
                                // If obstructions, just stick to crouching dimensions
                                Motor.SetCapsuleDimensions(0.5f, CrouchedCapsuleHeight, CrouchedCapsuleHeight * 0.5f);
                            }
                            else
                            {
                                // If no obstructions, uncrouch
                                MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                                _isCrouching = false;
                            }
                        }
                        break;
                    }
            }
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            // Handle landing and leaving ground
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLeaveStableGround();
            }
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            if (IgnoredColliders.Count == 0)
            {
                return true;
            }

            if (IgnoredColliders.Contains(coll))
            {
                return false;
            }

            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void AddVelocity(Vector3 velocity)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        _internalVelocityAdd += velocity;
                        break;
                    }
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        protected void OnLanded()
        {
            // Disable double jump since we're on the ground
            _canDoubleJump = false;
            _doubleJumpUsed = false;

            // Apply squash effect only if not crouching
            if (!_isCrouching)
            {
                MeshRoot.localScale = new Vector3(1.14f, 0.75f, 1.14f);

                // Reset scale after a short delay
                StartCoroutine(ResetScale());
            }

            // Play footstep sound
            if (FootstepAudioClips.Length > 0)
            {
                var clip = FootstepAudioClips[UnityEngine.Random.Range(0, FootstepAudioClips.Length)];
                audioSource.PlayOneShot(clip);
            }

            // Update state
            isInAir = false;
        }


        protected void OnLeaveStableGround()
        {
            // Update state
            isInAir = true;
        }
        private IEnumerator ResetScale()
        {
            isReturningToOriginalScale = true;

            yield return new WaitForSeconds(0.1f);

            float transitionDuration = 0.2f;
            Vector3 initialScale = MeshRoot.localScale;
            float time = 0f;

            while (time < transitionDuration)
            {
                time += Time.deltaTime;
                MeshRoot.localScale = Vector3.Lerp(initialScale, originalScale, time / transitionDuration);
                yield return null;
            }

            MeshRoot.localScale = originalScale;
            isReturningToOriginalScale = false;
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }
}