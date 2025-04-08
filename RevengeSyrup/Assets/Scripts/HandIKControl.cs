using UnityEngine;

public class HandIKControl : MonoBehaviour
{
    public Animator handAnimator; // Reference to the hand's Animator
    public Transform gunTransform; // The gun the hand is holding

    private void OnAnimatorIK(int layerIndex)
    {
        if (handAnimator == null || gunTransform == null)
            return;

        // Set the target position for the hand
        handAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        handAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        handAnimator.SetIKPosition(AvatarIKGoal.LeftHand, gunTransform.position);
        handAnimator.SetIKRotation(AvatarIKGoal.LeftHand, gunTransform.rotation);
    }
}
