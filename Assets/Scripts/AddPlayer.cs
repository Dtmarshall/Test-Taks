using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPlayer : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent<Player>(out Player player))
        {
            player.gameManager.CreatePlayerWorld(transform.position);
            Destroy(gameObject);
        }
    }
}
