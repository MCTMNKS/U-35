using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class NPC : MonoBehaviour, Iinteractable
{
    [SerializeField] private SpriteRenderer _interactSprite;

    private Transform _playerTransform;
    protected bool isInteracting = false; // Add this line



    private const float INTERACT_DISTANCE = 5f;

    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("ExampleCharacter").transform;
    }

    private void Update()
    {
        // If the player presses the E key and is within the interaction distance, interact with the NPC
        if (Keyboard.current.eKey.wasPressedThisFrame && isWithinInteractDistance())
        {
            Interact();
        }

        // If the player is interacting and moves out of the interaction distance, stop interacting with the NPC
        if (isInteracting && !isWithinInteractDistance())
        {
            StopInteract();
        }



        if (_interactSprite.gameObject.activeSelf && !isWithinInteractDistance())
        {
            //turn off the sprite
            _interactSprite.gameObject.SetActive(false);
        }
        else if (!_interactSprite.gameObject.activeSelf && isWithinInteractDistance())
        {
            //turn on the sprite
            _interactSprite.gameObject.SetActive(true);
        }


    }

    public abstract void Interact();

    public virtual void StopInteract()
    {
        // This method can be overridden in derived classes
    }

    private bool isWithinInteractDistance()
    {
        if (Vector2.Distance(_playerTransform.position, transform.position) < INTERACT_DISTANCE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
