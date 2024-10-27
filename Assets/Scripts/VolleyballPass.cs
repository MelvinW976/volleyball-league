using UnityEngine;

public class VolleyballPass : MonoBehaviour
{
    public float passForce = 8f;            // Strength of the pass
    public Rigidbody ballRb;                // Reference to the ball's Rigidbody

    private bool canPass = false;           // Flag to check if the ball is in range

    [System.Obsolete]
    void Update()
    {
        // Check for pass input
        if (canPass && Input.GetKeyDown(KeyCode.K))
        {
            PerformPass();
        }
    }

    [System.Obsolete]
    private void PerformPass()
    {
        // Apply an upward and forward force to the ball
        // Vector3 passDirection = (transform.forward + Vector3.up).normalized;
        Vector3 passDirection =  Vector3.up.normalized;
        ballRb.velocity = Vector3.zero; // Reset the ball's velocity
        ballRb.angularVelocity = Vector3.zero; // Reset the ball's rotation
        ballRb.AddForce(passDirection * passForce, ForceMode.Impulse);

        Debug.Log("Passed the ball!");

        canPass = false; // Prevent multiple passes until the ball re-enters the trigger
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the ball
        if (other.attachedRigidbody == ballRb)
        {
            canPass = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When the ball exits the trigger, disallow passing
        if (other.attachedRigidbody == ballRb)
        {
            canPass = false;
        }
    }
}
