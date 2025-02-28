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
        var startPosition = transform.position - new Vector3(width / 2f, height / 2f, 0);
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var spawnPosition = startPosition + new Vector3(x, y, 0);
                Instantiate(cellPrefab, spawnPosition, Quaternion.identity, transform);
            }
        }
    }
}