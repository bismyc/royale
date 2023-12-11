using UnityEngine;

public class ActionMove : IAction
{
    private bool mIsActive = false;
    private Rigidbody2D mRigidBody;
    private Vector2 mTargetPosition;
    private Vector2 mDirection;
    private float mSpeed;
    private float mRange;

    public ActionMove(Rigidbody2D rigidbody, float range, float speed, Vector2 targetPosition)
    {
        mRigidBody = rigidbody;
        mTargetPosition = targetPosition;
        mSpeed = speed;
        mRange = range;
        mDirection = targetPosition - rigidbody.position;
        mDirection.Normalize();
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

        float distance = Vector2.Distance(mRigidBody.position, mTargetPosition);
        if (mIsActive && distance <= mRange)
        {
            mIsActive = false;
        } else if (mIsActive)
        {
            mRigidBody.MovePosition((Vector2)mRigidBody.position + (mDirection * mSpeed * Time.fixedDeltaTime));
        }
    }
}
