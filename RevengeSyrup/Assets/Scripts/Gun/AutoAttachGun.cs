using UnityEngine;

public class AutoAttachGun : MonoBehaviour
{
    private void Start()
    {
        // Change the name from "RightHand Controller" to "Right Controller"
        Transform rightHandTransform = GameObject.Find("Right Controller")?.transform;

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
            Debug.LogWarning("Could not find the Right Controller! Make sure it is in the scene and correctly named.");
        }
    }
}
