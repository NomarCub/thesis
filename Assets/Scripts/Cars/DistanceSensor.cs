using UnityEngine;

public class DistanceSensor : MonoBehaviour
{
    public GameObject car;
    public float length = 10f;
    public float carVelocity = 0f;

    public GameObject otherCar = null;
    private GameObject crossing = null;
    private GameObject barrier = null;

    private Rigidbody carRigidbody;
    private CarController carController;

    private void Start()
    {
        transform.localScale = new Vector3(3, 2, 1.5f);
        carRigidbody = car.GetComponent<Rigidbody>();
        carController = car.GetComponent<CarController>();
    }

    private void Update()
    {
        bool carIsInBrakingDistance = false;
        bool crossingIsInBrakingDistance = false;
        bool barrierIsInBrakingDistance = false;

        if (otherCar != null)
        {
            carIsInBrakingDistance = carRigidbody.velocity.magnitude > otherCar.GetComponent<Rigidbody>().velocity.magnitude
                || Vector3.Distance(car.transform.position, otherCar.transform.position) < 4;
            var otherController = otherCar.GetComponent<CarController>();
            if (carController.maxVelocity > otherController.maxVelocity + 4) //TODO
            {
                if (carController.TryOvertake(otherController))
                {
                    otherCar = null;
                    return;
                }
            }
        }

        if (crossing != null)
        {
            crossingIsInBrakingDistance = crossing.GetComponent<PedestrianCrossingController>().pedestrianCounter > 0;
        }

        barrierIsInBrakingDistance = barrier != null;

        carController.hasObstacle = carIsInBrakingDistance || crossingIsInBrakingDistance || barrierIsInBrakingDistance;

        CalculateSensorLength();
    }

    private void CalculateSensorLength()
    {
        carVelocity = carRigidbody.velocity.magnitude;

        float normalized = carVelocity / CarController.maxCarVelocity;

        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, length * normalized + 1.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case Strings.car:
                otherCar = other.gameObject;
                break;
            case Strings.pedestrianCrossing:
                crossing = other.gameObject;
                break;
            case Strings.barrier:
                barrier = other.gameObject;
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case Strings.car:
                otherCar = null;
                carController.hasObstacle = false;
                break;
            case Strings.pedestrianCrossing:
                crossing = null;
                carController.hasObstacle = false;
                break;
            case Strings.barrier:
                barrier = null;
                carController.hasObstacle = false;
                break;
            default:
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }
}
