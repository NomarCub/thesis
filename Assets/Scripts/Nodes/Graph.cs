using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public static Graph Instance { get; private set; }
    public List<Node> Nodes { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Nodes = new List<Node>(GetComponentsInChildren<Node>());
    }

    public void Reset()
    {
        foreach (Node node in Nodes)
        {
            node.Reset();
        }
    }
}
