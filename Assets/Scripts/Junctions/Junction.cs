using System.Collections.Generic;
using UnityEngine;

public abstract class Junction : MonoBehaviour
{
    public const float speedLimit = 3;
    protected List<(CarController, (Node, Node))> canGo = new List<(CarController, (Node, Node))>();
    protected List<(CarController, (Node, Node))> cantGo = new List<(CarController, (Node, Node))>();
    public void Enter(CarController car, (Node, Node) path)
    {
        var sensor = car.sensor.transform.localScale;
        car.sensor.transform.localScale = new Vector3(sensor.x / 2, sensor.y, sensor.z);
        cantGo.Add((car, path));
        EvaluateCars();
    }
    public void Exit(CarController car, (Node, Node) path)
    {
        var sensor = car.sensor.transform.localScale;
        car.sensor.transform.localScale = new Vector3(sensor.x * 2, sensor.y, sensor.z);
        canGo.Remove((car, path));
        EvaluateCars();
    }
    protected abstract void EvaluateCars();
}
