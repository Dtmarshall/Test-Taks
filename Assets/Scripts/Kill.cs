using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent<Player>(out Player player))
        {
            player.Death();
        }
    }
}
