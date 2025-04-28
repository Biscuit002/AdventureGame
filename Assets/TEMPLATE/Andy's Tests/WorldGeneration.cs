using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public GameObject grassPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(-8.9f, 5, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateTerrain();
        }
    }
    private void GenerateTerrain()
    {
        transform.position = new Vector3(-8.9f, 5, 0);
        bool isGrassPlaced = false;
        for (int i = 0; i < 11; i ++)
        {
            int randomValue = Random.Range(0, 4);
            if (randomValue == 0 && !isGrassPlaced)
            {
                Instantiate(grassPrefab, transform.position, Quaternion.identity);
                isGrassPlaced = true;
            }
            else if (randomValue > 0)
            {
                isGrassPlaced = false;
            }
            transform.position -= new Vector3(0, 1f, 0);
        }
    }
}
