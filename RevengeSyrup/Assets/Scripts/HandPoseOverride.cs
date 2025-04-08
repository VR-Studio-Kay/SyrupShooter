using UnityEngine;

public class HandPoseOverride : MonoBehaviour
{
    public Animator handAnimator;    // Reference to the hand's Animator (if any)
    public Transform gunTransform;   // Reference to the gun's transform
    public Transform handTransform;  // Reference to the hand's transform (the one holding the gun)

    // Set these offsets to adjust how the hand holds the gun
    public Vector3 gunHoldPositionOffset = new Vector3(0, 0, 0);
    public Vector3 gunHoldRotationOffset = new Vector3(0, 0, 0);

    void Start()
    {
        // Disable any automatic hand animation (if needed)
        if (handAnimator != null)
        {
            // Disable the Animator so it doesn't animate the hand
            handAnimator.enabled = false;
        }

        // Ensure the hand is positioned to hold the gun at start
        SetHandToGunPose();
    }

    void SetHandToGunPose()
    {
        // Position the hand to hold the gun in the correct pose
        if (handTransform != null && gunTransform != null)
        {
            handTransform.position = gunTransform.position + gunHoldPositionOffset;
            handTransform.rotation = gunTransform.rotation * Quaternion.Euler(gunHoldRotationOffset);
        }
    }

    // Optionally, you can update the hand pose dynamically if needed
    void Update()
    {
        SetHandToGunPose();
    }
}
