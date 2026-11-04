using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int poolSize = 10;

    private List<GameObject> objectPoolList = new List<GameObject>();

    void Start()
    {
        CreatePool();
    }

    // Creates the initial pool by instantiating poolSize objects
    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    // Instantiates a new inactive object and adds it to the pool list
    private GameObject CreateNewObject()
    {
        GameObject newObj = Instantiate(objectPrefab);
        newObj.SetActive(false);
        objectPoolList.Add(newObj);
        return newObj;
    }

    // Returns an inactive object from the pool, creating a new one if all are in use
    public GameObject GetGameObjectFromPool()
    {
        GameObject theObject = objectPoolList.Find(obj => !obj.activeInHierarchy);

        // If every object is in use, grow the pool
        if (theObject == null)
        {
            theObject = CreateNewObject();
        }

        theObject.SetActive(true);
        return theObject;
    }
}