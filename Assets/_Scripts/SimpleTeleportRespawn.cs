using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTeleportRespawn : MonoBehaviour
{

    public Transform spawnPoint;

    private void Awake()
    {
        if (spawnPoint == null)
        {
            GameObject newSpawn = new GameObject("SpawnPoint");
            spawnPoint = Instantiate(newSpawn, Vector3.zero, Quaternion.identity).transform;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        other.attachedRigidbody.gameObject.transform.position = spawnPoint.position;
    }
}
