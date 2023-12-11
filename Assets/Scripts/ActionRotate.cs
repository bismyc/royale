using UnityEngine;

public class ActionRotate : IAction
{
    private bool mIsActive = false;
    private Rigidbody2D mRigidBody;
    private float mTargetRotation;
    private float mRotationDelta;
    private float mRotationSpeed;

    public ActionRotate(Rigidbody2D rigidbody, float targetRotation, float rotSpeed)
    {
        mRigidBody = rigidbody;
        mTargetRotation = targetRotation;
        mRotationDelta = mTargetRotation - mRigidBody.rotation;
        mRotationSpeed = rotSpeed;
        mIsActive = true;
    }

    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    public bool IsActive()
    {
        return mIsActive;
    }

    public void Update()
    {
        mRigidBody.MoveRotation(mRigidBody.rotation + mRotationDelta * mRotationSpeed * Time.fixedDeltaTime);
        if (mIsActive && Mathf.Abs(mRigidBody.rotation - mTargetRotation) <= 1)
        {
            mIsActive = false;
        }
    }
}
