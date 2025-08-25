using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllBoxActive : MonoBehaviour
{
    [SerializeField] private bool isPoint;
    [SerializeField] private SpawnPoint point;

    public void OnEnable()
    {
        if (isPoint)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            point.spawnedEnemies.Clear();
        }
        else
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
