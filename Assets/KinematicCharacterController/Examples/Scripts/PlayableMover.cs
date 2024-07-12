using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Playables;

namespace KinematicCharacterController.Examples
{
    public class PlayableMover : MonoBehaviour, IMoverController
    {
        public PhysicsMover Mover;

        public float Speed = 1f; // Speed of the animation
        public float ForwardSpeed = 5f; // Speed at which the object will move forward
        public PlayableDirector Director;

        private Transform _transform;
        private Vector3 _startPosition;
        private Quaternion _startRotation; // Save starting rotation
        private float _elapsedTime = 0f; // Timer to track the elapsed time
        private float _moveTime = 20f; // Time in seconds after which the ship will stop moving
        private float _slowDownTime = 10f; // Time over which the animation slows down
        private float _originalSpeed; // Original animation speed
        private float _minSpeed = 0.1f; // Minimum animation speed

        private void Start()
        {
            _transform = this.transform;
            Director.timeUpdateMode = DirectorUpdateMode.Manual;

            Mover.MoverController = this;
            _startPosition = _transform.position; // Save the starting position
            _startRotation = _transform.rotation; // Save the starting rotation
            _originalSpeed = Speed; // Save the original animation speed
        }

        public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
        {
            // Update the timer
            _elapsedTime += deltaTime;

            // Remember pose before animation
            Vector3 positionBeforeAnim = _transform.position;
            Quaternion rotationBeforeAnim = _transform.rotation;

            // Calculate the new goal position based on elapsed time
            if (_elapsedTime <= _moveTime)
            {
                float movementFactor = _elapsedTime / _moveTime;
                goalPosition = _startPosition + (_startRotation * Vector3.forward * ForwardSpeed * movementFactor);
            }
            else
            {
                goalPosition = _startPosition + (_startRotation * Vector3.forward * ForwardSpeed);

                // Slow down the animation over time, but not below _minSpeed
                float slowDownFactor = Mathf.Clamp01((_elapsedTime - _moveTime) / _slowDownTime);
                Speed = Mathf.Lerp(_originalSpeed, _minSpeed, slowDownFactor);
            }

            // Update animation
            Director.time += deltaTime * Speed;
            if (Director.time >= Director.duration)
            {
                Director.time -= Director.duration; // Loop the animation
            }
            Director.Evaluate();

            // Set our platform's goal rotation to the animation's
            goalRotation = _transform.rotation;

            // Reset the actual transform pose to where it was before evaluating. 
            // This is so that the real movement can be handled by the physics mover; not the animation
            _transform.position = positionBeforeAnim;
            _transform.rotation = rotationBeforeAnim;
        }
    }
}






