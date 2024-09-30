using UnityEngine;

public class PlayerHealthCollectable : CollectableBase
{
    public int HealAmount = 10;
    public GameObject PickupEffect;

    public override void Collect(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            if(player.CurrentHealth >= player.MaxHealth) return;
            player.AddHealth(HealAmount);
            if (PickupEffect)
            {
                Instantiate(PickupEffect, transform.position, Quaternion.identity, null);
            }
            Destroy(gameObject);
        }
    }
}