using System.Collections.Generic;
using UnityEngine;

public class Dijkstra : MonoBehaviour
{
    public static Dijkstra Instance { get; private set; }

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

    public List<Node> CalculateShortestPath(Graph graph, Node source, Node destination)
    {
        List<Node> visitedNodes = new List<Node>();
        List<Node> unvisitedNodes = new List<Node>
        {
            source
        };

        graph.Reset();
        source.distanceFromSource = 0;

        while (unvisitedNodes.Count > 0)
        {
            Node currentNode = GetLowestDistanceNode(unvisitedNodes);

            foreach (Node neighbour in currentNode.neighbours)
            {
                float edgeWeight = currentNode.GetWeightOf(neighbour.transform);

                if (!visitedNodes.Contains(neighbour))
                {
                    CalculateMinimumDistance(neighbour, edgeWeight, currentNode);
                    unvisitedNodes.Add(neighbour);
                }
            }

            unvisitedNodes.Remove(currentNode);
            visitedNodes.Add(currentNode);
        }

        List<Node> path = new List<Node>(destination.shortestPath)
        {
            destination
        };

        return path;
    }

    private void CalculateMinimumDistance(Node neighbour, float edgeWeight, Node currentNode)
    {
        float sourceDistance = currentNode.distanceFromSource;

        if (sourceDistance + edgeWeight < neighbour.distanceFromSource)
        {
            neighbour.distanceFromSource = sourceDistance + edgeWeight;
            List<Node> shortestPath = new List<Node>(currentNode.shortestPath)
            {
                currentNode
            };
            neighbour.shortestPath = shortestPath;
        }
    }

    private Node GetLowestDistanceNode(List<Node> unvisitedNodes)
    {
        Node lowestDistanceNode = null;
        float lowestDistance = float.MaxValue;

        foreach (Node node in unvisitedNodes)
        {
            float nodeDistanceFromSource = node.distanceFromSource;
            if (nodeDistanceFromSource < lowestDistance)
            {
                lowestDistance = nodeDistanceFromSource;
                lowestDistanceNode = node;
            }
        }

        return lowestDistanceNode;
    }
}