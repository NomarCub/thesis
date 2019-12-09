using System.Collections.Generic;
using UnityEngine;

public class JunctionT : Junction
{
    Node NodeA;
    Node NodeB;
    Node NodeC;
    Node NodeD;
    Node NodeE;
    Node NodeF;
    List<(Node, Node)> rightTurns;
    List<(Node, Node)> leftTurns;
    List<(Node, Node)> straights;
    List<((Node, Node), (Node, Node))> crossing;

    void Start()
    {
        var nodes = gameObject.GetComponentsInChildren<Node>();

        NodeA = nodes[0];
        NodeB = nodes[1];
        NodeC = nodes[2];
        NodeD = nodes[3];
        NodeE = nodes[4];
        NodeF = nodes[5];

        rightTurns = new List<(Node, Node)> {
            (NodeA, NodeB),
            (NodeE, NodeF)
        };

        leftTurns = new List<(Node, Node)> {
            (NodeA, NodeD),
            (NodeC, NodeF)
        };

        straights = new List<(Node, Node)> {
            (NodeC, NodeD),
            (NodeE, NodeB)
        };


        // TODO: Node pairs would be enough
        crossing = new List<((Node, Node), (Node, Node))>{
            ((NodeA, NodeD), (NodeE, NodeB)),
            ((NodeA, NodeD), (NodeC, NodeF)),
            ((NodeC, NodeF), (NodeE, NodeB))
        };
    }

    bool Intersect((Node, Node) path, (Node, Node) otherPath)
    {
        if (path.Item1 == otherPath.Item1)
            return false;
        if (path.Item2 == otherPath.Item2)
            return true;
        if (crossing.Contains((path, otherPath)) || crossing.Contains((otherPath, path)))
            return true;
        else return false;
    }

    protected override void EvaluateCars()
    {
        cantGo.ForEach(car =>
        {
            if (rightTurns.Contains(car.Item2) &&
                    canGo.TrueForAll(otherCar => !Intersect(car.Item2, otherCar.Item2)))
                canGo.Add(car);
        });
        cantGo.ForEach(car =>
        {
            if (straights.Contains(car.Item2) && canGo.TrueForAll(otherCar => !Intersect(car.Item2, otherCar.Item2)))
                canGo.Add(car);
        });
        cantGo.ForEach(car =>
        {
            if (leftTurns.Contains(car.Item2) && canGo.TrueForAll(otherCar => !Intersect(car.Item2, otherCar.Item2)))
                canGo.Add(car);
        });

        canGo.ForEach(it =>
        {
            it.Item1.forbiddenTraffic = false;
            cantGo.Remove(it);
        });
        cantGo.ForEach(it => it.Item1.forbiddenTraffic = true);
    }
}