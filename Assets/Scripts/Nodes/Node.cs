using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Node : MonoBehaviour
{
    public static int currentID = 0;
    public int ID;
    public List<Node> shortestPath = new List<Node>();
    public float distanceFromSource = float.MaxValue;
    public List<Node> neighbours = new List<Node>();

    public override bool Equals(object obj) => obj is Node node && ID == node.ID;
    public override int GetHashCode() => ID;

    private void Awake()
    {
        ID = Interlocked.Increment(ref currentID);
    }

    public float GetWeightOf(Transform neighbour)
    {
        return Vector3.Distance(transform.position, neighbour.position);
    }

    public void Reset()
    {
        shortestPath.Clear();
        distanceFromSource = float.MaxValue;
    }

}
