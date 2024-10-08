using UnityEngine;

public class HumanHealthCollectable : CollectableBase
{
    public int HealAmount = 10;

    public override void Collect(Collider other)
    {
        Human human = other.GetComponent<Human>();
        if (human)
        {
            if (human.CurrentHealth >= human.MaxHealth) return;
            human.OnHeal(HealAmount);
            Destroy(gameObject);
        }
    }
}