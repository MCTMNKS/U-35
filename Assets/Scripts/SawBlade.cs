using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SawBladeMovement : MonoBehaviour
{
    public float time = 1f;
    public float length = 15f;
    public string axis = "x";
    public float maxRotationSpeed = 100f; // Maximum speed of rotation in degrees per second
    public float startDelay = 0f; // Start delay in seconds

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Tweener tweener;

    void Start()
    {
        startPosition = transform.position;

        switch (axis.ToLower())
        {
            case "x":
                endPosition = startPosition + new Vector3(length, 0, 0);
                break;
            case "y":
                endPosition = startPosition + new Vector3(0, length, 0);
                break;
            case "z":
                endPosition = startPosition + new Vector3(0, 0, length);
                break;
            default:
                Debug.LogError("Invalid axis specified! Please enter x, y, or z.");
                break;
        }

        Invoke("MoveSawBlade", startDelay); // Invoke the MoveSawBlade method after startDelay seconds
    }

    void Update()
    {
        // Rotate the saw blade around the Z-axis at a speed proportional to the current velocity of the movement
        if (tweener != null)
        {
            float rotationSpeed = maxRotationSpeed * tweener.fullPosition / length;
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }

    void MoveSawBlade()
    {
        tweener = transform.DOMove(endPosition, time).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            Vector3 temp = startPosition;
            startPosition = endPosition;
            endPosition = temp;
            MoveSawBlade();
        });
    }
}
