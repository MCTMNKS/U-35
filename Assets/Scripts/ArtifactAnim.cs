using UnityEngine;
using DG.Tweening;

public class ArtifactMove : MonoBehaviour
{
    public float rotationSpeed = 50f; // Speed of rotation
    public float moveDistance = 3f; // Distance to move up and down
    public float moveDuration = 1f; // Duration of one move up or down
    private Sequence sequence; // Reference to the sequence

    void Start()
    {
        // Start the up and down movement
        MoveUpAndDown();
    }

    void Update()
    {
        // Rotate the object
        if (transform != null)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    void MoveUpAndDown()
    {
        // Check if the transform is not null
        if (transform != null)
        {
            // Create a sequence
            sequence = DOTween.Sequence();

            // Add move up to the sequence
            sequence.Append(transform.DOMoveY(transform.position.y + moveDistance, moveDuration).SetEase(Ease.OutQuad));

            // Add move down to the sequence
            sequence.Append(transform.DOMoveY(transform.position.y, moveDuration).SetEase(Ease.InQuad));

            // Loop the sequence indefinitely
            sequence.SetLoops(-1);
        }
    }

    void OnDestroy()
    {
        // Stop all tweens on this GameObject
        if (sequence != null)
        {
            sequence.Kill();
        }
    }
}

