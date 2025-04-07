using UnityEngine;

public class AutoAttachGun : MonoBehaviour
{
    private void Start()
    {
        // Simply make sure the gun is in the right hand at the start.
        Transform rightHandTransform = GameObject.Find("RightHand Controller").transform;

        if (rightHandTransform != null)
        {
            // Parent the gun to the right hand to follow it
            transform.SetParent(rightHandTransform);

            // Set the gun's local position and rotation relative to the hand
            transform.localPosition = Vector3.zero;  // Adjust as needed for positioning
            transform.localRotation = Quaternion.identity; // Adjust as needed
        }
        else
        {
            Debug.LogWarning("Could not find the Right Hand Controller!");
        }
    }
}
