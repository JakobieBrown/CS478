using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObjects/PowerUpEffects/SpeedBoost")]
public class SpeedBoost : PowerUpEffect
{
    public float bonusAcceleration;
    public float duration;
    public override void Apply(GameObject target)
    {
        StatBlock statBlock = target.GetComponent<StatBlock>();
        if (statBlock != null)
        {
            statBlock.BoostAcceleration(bonusAcceleration, duration);
        }
    }
}
