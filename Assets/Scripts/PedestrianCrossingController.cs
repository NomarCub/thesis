using UnityEngine;

public class PedestrianCrossingController : MonoBehaviour
{
    public int pedestrianCounter { get; private set; } = 0;
    public int carCounter { get; private set; } = 0;
    public bool dangerous { get; private set; } = false;

    private void SetDangerous(GameObject other)
    {
        DistanceSensor distanceSensor = other.GetComponent<DistanceSensor>();
        float distance = Vector3.Distance(transform.position, distanceSensor.car.transform.position);
        float velocity = distanceSensor.car.GetComponent<Rigidbody>().velocity.magnitude;

        if (distance < 10f && velocity > 10f)
        {
            dangerous = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case Strings.car:
                carCounter++;
                break;
            case Strings.pedestrian:
                pedestrianCounter++;
                break;
            case Strings.distanceSensor:
                SetDangerous(other.gameObject);
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case Strings.car:
                carCounter--;
                break;
            case Strings.pedestrian:
                pedestrianCounter--;
                break;
            case Strings.distanceSensor:
                dangerous = false;
                break;
            default:
                break;
        }
    }
}
