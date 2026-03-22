using UnityEngine;
using Utilities;

public class StatBlock : MonoBehaviour
{
    [SerializeField] private float _acceleration;
    [SerializeField] private float _jumpForce;

    private float _bonusAcceleration;
    private float _penaltyAcceleration;

    public float Acceleration() => _acceleration + _bonusAcceleration + _penaltyAcceleration;
    public float JumpForce() => _jumpForce;

    private CountdownTimer accelerationBoostTimer;
    private CountdownTimer accelerationPenaltyTimer;

    public void BoostAcceleration(float bonusAcceleration, float duration)
    {
        if (bonusAcceleration < 0)
            Debug.LogWarning("Value supplied to LowerAcceleration(float, float) is non-positive, which will result in an decrease in acceleration.");
        accelerationBoostTimer = new CountdownTimer(duration);
        accelerationBoostTimer.OnTimerStart += () => { _bonusAcceleration = bonusAcceleration; Debug.Log("PowerUp Started. Acceleration is now " + Acceleration().ToString()); };
            accelerationBoostTimer.OnTimerStop += () => { _bonusAcceleration = 0; Debug.Log("PowerUp Over. Acceleration is now " + Acceleration().ToString()); };
        accelerationBoostTimer.Start();
    }

    public void LowerAcceleration(float penaltyAcceleration, float duration)
    {
        if (penaltyAcceleration > 0)
            Debug.LogWarning("Value supplied to LowerAcceleration(float, float) is non-negative, which will result in an increase in acceleration.");
        accelerationPenaltyTimer = new CountdownTimer(duration);
        accelerationPenaltyTimer.OnTimerStart += () => _penaltyAcceleration = penaltyAcceleration;
        accelerationPenaltyTimer.OnTimerStop -= () => _penaltyAcceleration = 0;
        accelerationPenaltyTimer.Start();
    }

    public void Update()
    {
        accelerationBoostTimer.Tick(Time.deltaTime);
        accelerationPenaltyTimer?.Tick(Time.deltaTime);
    }
}
