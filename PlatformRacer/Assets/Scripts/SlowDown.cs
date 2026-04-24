using UnityEngine;
[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObjects/PowerUpEffects/SlowDown")]
public class SlowDown : PowerUpEffect
{
    public float penaltyAcceleration;
    public float duration;
    public override void Apply(GameObject target)
    {
        StatBlock statBlock = target.GetComponent<StatBlock>();
        if (statBlock != null)
        {
            statBlock.LowerAcceleration(-penaltyAcceleration, duration);
        }
    }
}