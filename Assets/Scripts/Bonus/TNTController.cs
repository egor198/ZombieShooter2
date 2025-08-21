using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNTController : MonoBehaviour
{
    [SerializeField] private GameObject explosionCollider;
    [SerializeField] private GameObject barrel;
    [SerializeField] private GameObject effectExplosion;
    [SerializeField] private int health;

    public void Explosion()
    {
        health--;
        if(health <= 0)
        {
            explosionCollider.SetActive(true);
            barrel.SetActive(false);
            Destroy(gameObject, 0.5f);
            if (effectExplosion != null) Instantiate(effectExplosion, transform.position, Quaternion.identity);
        }
    }
}
