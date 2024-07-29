using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class ExampleCharacterCamera : MonoBehaviour
    {
        [Header("Framing")]
        public Camera Camera;
        public Vector2 FollowPointFraming = new Vector2(0f, 0f);
        public float FollowingSharpness = 10000f;

        [Header("Distance")]
        public float DefaultDistance = 6f;
        public float MinDistance = 0f;
        public float MaxDistance = 10f;
        public float DistanceMovementSpeed = 5f;
        public float DistanceMovementSharpness = 10f;

        [Header("Rotation")]
        public bool InvertX = false;
        public bool InvertY = false;
        [Range(-90f, 90f)]
        public float DefaultVerticalAngle = 20f;
        [Range(-90f, 90f)]
        public float MinVerticalAngle = -90f;
        [Range(-90f, 90f)]
        public float MaxVerticalAngle = 90f;
        public float RotationSpeed = 1f;
        public float RotationSharpness = 10000f;
        public bool RotateWithPhysicsMover = false;

        [Header("Obstruction")]
        public float ObstructionCheckRadius = 0.2f;
        public LayerMask ObstructionLayers = -1;
        public float ObstructionSharpness = 10000f;
        public List<Collider> IgnoredColliders = new List<Collider>();

        [Header("Screen Shake")]
        public float shakeDuration = 0.4f;
        public float shakeMagnitude = 0.1f;
        public float dampingSpeed = 1.0f;
        Vector3 initialPosition;
        bool isShaking = false;
        float shakeTimer = 0f;
        float shakeCooldown = 2f;
        Vector3 shakeOffset = Vector3.zero;

        // New field for the dash AudioSource
        public AudioSource audioSource;
        public AudioClip dashSound; // The sound clip for the dash

        public Transform Transform { get; private set; }
        public Transform FollowTransform { get; private set; }

        public Vector3 PlanarDirection { get; set; }
        public float TargetDistance { get; set; }

        private bool _distanceIsObstructed;
        private float _currentDistance;
        private float _targetVerticalAngle;
        private RaycastHit _obstructionHit;
        private int _obstructionCount;
        private RaycastHit[] _obstructions = new RaycastHit[MaxObstructions];
        private float _obstructionTime;
        private Vector3 _currentFollowPosition;
        private const int MaxObstructions = 32;

        // New variable to track if the dash power is acquired
        private bool hasDashPower = false;

        void OnValidate()
        {
            DefaultDistance = Mathf.Clamp(DefaultDistance, MinDistance, MaxDistance);
            DefaultVerticalAngle = Mathf.Clamp(DefaultVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
        }

        void Awake()
        {
            Transform = this.transform;
            _currentDistance = DefaultDistance;
            TargetDistance = _currentDistance;
            _targetVerticalAngle = 0f;
            PlanarDirection = Vector3.forward;
            initialPosition = Transform.localPosition;

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        
    }

        void Update()
        {
            // Update the shake timer
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
            }

            // Trigger screen shake only if dash power is acquired
            if (Input.GetKeyDown(KeyCode.R) && !isShaking && shakeTimer <= 0 && hasDashPower)
            {
                StartCoroutine(Shake());
                shakeTimer = shakeCooldown;  // Reset the shake timer
            }
        }

        IEnumerator Shake()
        {
            isShaking = true;
            float elapsed = 0.0f;

            // Play sound if available and not playing
            if (audioSource != null && dashSound != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(dashSound);
            }

            while (elapsed < shakeDuration)
            {
                float x = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
                float y = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

                shakeOffset = new Vector3(x, y, 0);

                elapsed += Time.deltaTime;

                yield return null;
            }

            shakeOffset = Vector3.zero;
            isShaking = false;
        }

        // Public method to enable dash power
        public void EnableDashPower()
        {
            hasDashPower = true;
        }

        public void SetFollowTransform(Transform t)
        {
            FollowTransform = t;
            PlanarDirection = FollowTransform.forward;
            _currentFollowPosition = FollowTransform.position;
        }

        public void UpdateWithInput(float deltaTime, float zoomInput, Vector3 rotationInput)
        {
            if (FollowTransform)
            {
                if (InvertX)
                {
                    rotationInput.x *= -1f;
                }
                if (InvertY)
                {
                    rotationInput.y *= -1f;
                }

                Quaternion rotationFromInput = Quaternion.Euler(FollowTransform.up * (rotationInput.x * RotationSpeed));
                PlanarDirection = rotationFromInput * PlanarDirection;
                PlanarDirection = Vector3.Cross(FollowTransform.up, Vector3.Cross(PlanarDirection, FollowTransform.up));
                Quaternion planarRot = Quaternion.LookRotation(PlanarDirection, FollowTransform.up);

                _targetVerticalAngle -= (rotationInput.y * RotationSpeed);
                _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
                Quaternion verticalRot = Quaternion.Euler(_targetVerticalAngle, 0, 0);
                Quaternion targetRotation = Quaternion.Slerp(Transform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-RotationSharpness * deltaTime));

                Transform.rotation = targetRotation;

                if (_distanceIsObstructed && Mathf.Abs(zoomInput) > 0f)
                {
                    TargetDistance = _currentDistance;
                }
                TargetDistance += zoomInput * DistanceMovementSpeed;
                TargetDistance = Mathf.Clamp(TargetDistance, MinDistance, MaxDistance);

                _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, FollowTransform.position, 1f - Mathf.Exp(-FollowingSharpness * deltaTime));

                RaycastHit closestHit = new RaycastHit();
                closestHit.distance = Mathf.Infinity;
                _obstructionCount = Physics.SphereCastNonAlloc(_currentFollowPosition, ObstructionCheckRadius, -Transform.forward, _obstructions, TargetDistance, ObstructionLayers, QueryTriggerInteraction.Ignore);
                for (int i = 0; i < _obstructionCount; i++)
                {
                    bool isIgnored = false;
                    for (int j = 0; j < IgnoredColliders.Count; j++)
                    {
                        if (IgnoredColliders[j] == _obstructions[i].collider)
                        {
                            isIgnored = true;
                            break;
                        }
                    }
                    for (int j = 0; j < IgnoredColliders.Count; j++)
                    {
                        if (IgnoredColliders[j] == _obstructions[i].collider)
                        {
                            isIgnored = true;
                            break;
                        }
                    }

                    if (!isIgnored && _obstructions[i].distance < closestHit.distance && _obstructions[i].distance > 0)
                    {
                        closestHit = _obstructions[i];
                    }
                }

                if (closestHit.distance < Mathf.Infinity)
                {
                    _distanceIsObstructed = true;
                    _currentDistance = Mathf.Lerp(_currentDistance, closestHit.distance, 1 - Mathf.Exp(-ObstructionSharpness * deltaTime));
                }
                else
                {
                    _distanceIsObstructed = false;
                    _currentDistance = Mathf.Lerp(_currentDistance, TargetDistance, 1 - Mathf.Exp(-DistanceMovementSharpness * deltaTime));
                }

                Vector3 targetPosition = _currentFollowPosition - ((targetRotation * Vector3.forward) * _currentDistance);

                targetPosition += Transform.right * FollowPointFraming.x;
                targetPosition += Transform.up * FollowPointFraming.y;

                Transform.position = targetPosition + shakeOffset;
            }
        }
    }
}
