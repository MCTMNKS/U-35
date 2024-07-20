using System.Collections.Generic;
using UnityEngine;

public class ActionRecorder : MonoBehaviour
{
    public struct PlayerAction
    {
        public Vector3 position;
        public Quaternion rotation;
        public AnimatorStateInfo animationState;  // Add this line
    }

    public Queue<PlayerAction> actions = new Queue<PlayerAction>();
    private Vector3 lastRecordedPosition;
    private Animator animator;  // Add this line

    void Start()
    {
        lastRecordedPosition = transform.position;
        animator = GetComponent<Animator>();  // Add this line
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, lastRecordedPosition) > 0.001f) // Small threshold to detect significant movement
        {
            PlayerAction action = new PlayerAction();
            action.position = transform.position;
            action.rotation = transform.rotation;
            action.animationState = animator.GetCurrentAnimatorStateInfo(0);  // Add this line
            actions.Enqueue(action);

            lastRecordedPosition = transform.position;
        }
    }
}







