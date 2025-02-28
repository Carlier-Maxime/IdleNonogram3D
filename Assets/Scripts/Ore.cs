using System;
using UnityEngine;

public class Ore : MonoBehaviour
{
    [SerializeField]
    private GameObject cellPrefab;
    [SerializeField]
    private int width = 5;
    [SerializeField]
    private int height = 5;

    private void Start()
    {
        if (!cellPrefab.GetComponent<Cell>()) Debug.LogError("The Prefab used not content a component Cell.");
        var newPrefab = Instantiate(cellPrefab, transform.position, Quaternion.identity);
    }
}