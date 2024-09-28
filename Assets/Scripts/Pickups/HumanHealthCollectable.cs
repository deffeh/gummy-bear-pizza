using UnityEngine;

public class HumanHealthCollectable : CollectableBase
{
    public int HealAmount = 10;

    public override void Collect(Collider other)
    {
        Human human = other.GetComponent<Human>();
        if (human)
        {
            // human.AddHealth(HealAmount);
            Debug.Log("Human collected health pickup");
            Destroy(gameObject);
        }
    }
}