using UnityEngine;

public class ActionAttack : IAction
{
    private bool mIsActive = false;
    private GameBody mOpponent;
    private float mDamage;
    private float mHitRate;
    private float mAttackTimer;
    private float mRange;
    private Vector2 mPosition;

    public ActionAttack(GameBody opponent, Vector2 position, float range, float damage, float hitRate)
    {
        mOpponent = opponent;
        mDamage = damage;
        mHitRate = hitRate;
        mAttackTimer = mHitRate;
        mRange = range;
        mPosition = position;
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
        float distance = Vector2.Distance(mOpponent.transform.position, mPosition);
        if (distance <= mRange) {
            if (mAttackTimer >= mHitRate)
            {
                mAttackTimer = 0;
                float reducedHealth = mOpponent.GetHealth() - mDamage;
                mOpponent.SetHealth(reducedHealth);
            }

            mAttackTimer += Time.fixedDeltaTime;
        }
        
        if (mIsActive && (mOpponent.GetHealth() <= 0 || distance > mRange))
        {
            mIsActive = false;
        }
    }
}
