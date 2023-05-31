using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public GameObject deathParticles;
    public GameManager gameManager;

    public Animator animator;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void Death()
    {
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        gameManager.RemovePlayer(this);
        Destroy(gameObject);
    }
}
