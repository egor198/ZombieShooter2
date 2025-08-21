using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [System.NonSerialized] public bool isOccupied = false;
    [System.NonSerialized] public EnemyController occupiedBy;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
        isOccupied = false;
        occupiedBy = null;
    }

    // Атомарное занятие точки
    public bool TryOccupy(EnemyController occupier)
    {
        if (isOccupied) return false;

        isOccupied = true;
        occupiedBy = occupier;
        return true;
    }
}

