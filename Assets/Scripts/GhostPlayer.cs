using System.Collections;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    public ActionRecorder actionRecorder;  // Reference to the ActionRecorder on the player
    public float delay = 3f;  // The delay before the ghost starts following
    private Animator animator;  // Add this line

    IEnumerator Start()
    {
        animator = GetComponent<Animator>();  // Add this line

        // Wait for the delay time
        yield return new WaitForSeconds(delay);

        // Start dequeuing actions
        while (true)
        {
            if (actionRecorder.actions.Count > 0)
            {
                ActionRecorder.PlayerAction action = actionRecorder.actions.Dequeue();
                transform.position = action.position;
                transform.rotation = action.rotation;
                animator.Play(action.animationState.fullPathHash, 0, action.animationState.normalizedTime);  // Add this line
            }

            yield return null;  // Wait until the next frame
        }
    }
}



