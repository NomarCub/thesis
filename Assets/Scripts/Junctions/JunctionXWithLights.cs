using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Phase { North, NorthToEast, East, EastToNorth }
static class PhaseExtension
{
    public static Phase Next(this Phase phase)
    {
        switch (phase)
        {
            case Phase.North:
                return Phase.NorthToEast;
            case Phase.NorthToEast:
                return Phase.East;
            case Phase.East:
                return Phase.EastToNorth;
            case Phase.EastToNorth:
                return Phase.North;
            default:
                return Phase.North;
        }
    }
}
public class JunctionXWithLights : JunctionX
{
    Phase Phase;

    readonly float mainPhaseTime = 10;
    readonly float inBetWeenPhaseTime = 3;
    float phaseTime = 10;
    float currentTime = 0;

    List<Node> north;
    List<Node> east;

    List<TrafficLight> northLights;
    List<TrafficLight> eastLights;
    protected override void Start()
    {
        base.Start();

        var lights = gameObject.GetComponentsInChildren<TrafficLight>();
        var lightA = lights[0];
        var lightC = lights[1];
        var lightE = lights[2];
        var lightG = lights[3];

        north = new List<Node>(){
            NodeA, NodeE
        };
        east = new List<Node>(){
            NodeC, NodeG
        };
        northLights = new List<TrafficLight>(){
            lightA, lightE
        };
        eastLights = new List<TrafficLight>(){
            lightC, lightG
        };

        ChangePhase();
    }

    bool CanGo((CarController car, (Node, Node)) car)
    {
        return (((Phase == Phase.North && north.Contains(car.Item2.Item1)) ||
                (Phase == Phase.East && east.Contains(car.Item2.Item1))) &&
                canGo.TrueForAll(otherCar => !Intersect(otherCar.Item2, car.Item2)));

    }
    protected override void EvaluateCars()
    {
        cantGo.ForEach(car =>
        {
            if (rightTurns.Contains(car.Item2) && CanGo(car))
                canGo.Add(car);
        });
        cantGo.ForEach(car =>
        {
            if (straights.Contains(car.Item2) && CanGo(car))
                canGo.Add(car);
        });
        cantGo.ForEach(car =>
        {
            if (leftTurns.Contains(car.Item2) && CanGo(car))
                canGo.Add(car);
        });

        canGo.ForEach(it =>
        {
            it.Item1.forbiddenTraffic = false;
            cantGo.Remove(it);
        });
        cantGo.ForEach(it => it.Item1.forbiddenTraffic = true);
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > phaseTime)
        {
            if (Phase == Phase.North || Phase == Phase.East)
                phaseTime = inBetWeenPhaseTime;
            else phaseTime = mainPhaseTime;
            currentTime = 0;

            ChangePhase();
        }
    }

    void ChangePhase()
    {
        Phase = Phase.Next();
        switch (Phase)
        {
            case Phase.North:
                northLights.ForEach(ligth =>
                {
                    ligth.green = true;
                    ligth.red = false;
                    ligth.yellow = false;
                });
                eastLights.ForEach(ligth =>
                {
                    ligth.green = false;
                    ligth.red = true;
                    ligth.yellow = false;
                });
                break;
            case Phase.NorthToEast:
                northLights.ForEach(ligth =>
                    {
                        ligth.green = false;
                        ligth.red = false;
                        ligth.yellow = true;
                    });
                eastLights.ForEach(ligth =>
                {
                    ligth.green = false;
                    ligth.red = true;
                    ligth.yellow = true;
                });
                break;
            case Phase.East:
                northLights.ForEach(ligth =>
                    {
                        ligth.green = false;
                        ligth.red = true;
                        ligth.yellow = false;
                    });
                eastLights.ForEach(ligth =>
                {
                    ligth.green = true;
                    ligth.red = false;
                    ligth.yellow = false;
                });
                break;
            case Phase.EastToNorth:
                northLights.ForEach(ligth =>
                    {
                        ligth.green = false;
                        ligth.red = true;
                        ligth.yellow = true;
                    });
                eastLights.ForEach(ligth =>
                {
                    ligth.green = false;
                    ligth.red = false;
                    ligth.yellow = true;
                });
                break;
            default:
                break;
        }

        EvaluateCars();
    }

}
