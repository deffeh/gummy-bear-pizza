using UnityEngine;

public class HealthPickup : CollectableBase
{
    public int HealAmount = 10;

    public override void Collect(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (!player)
        {
            player.AddHealth(HealAmount);
            Debug.Log("Player collected health pickup");
        }
    }
}