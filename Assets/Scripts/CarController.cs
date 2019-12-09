using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class CarController : MonoBehaviour
{
    public static int noteID = 0;
    public int ID;

    public override bool Equals(object obj) => obj is CarController controller && ID == controller.ID;
    public override int GetHashCode() => ID;

    private void Awake()
    {
        ID = Interlocked.Increment(ref noteID);
    }


    public float speedLimit
    {
        get => _speedlimit;
        set => _speedlimit = value < maxVelocity ? value : maxVelocity;
    }

    public float maxVelocity = 4.33f;
    public UnityEvent OnDestroy;
    public Node destination = null;
    public Node source = null;
    [SerializeField]
    private bool isInJunction = false;
    public bool isParking = false;
    public bool forbiddenTraffic = false;
    public bool hasObstacle = false;

    [SerializeField]
    private float maxSteeringAngle = 38f;
    [SerializeField]
    private float motorTorque = 200f;
    [SerializeField]
    private float brakeTorque = 1000f;
    [SerializeField]
    private float velocity;

    [SerializeField]
    private WheelCollider frontLeftWheelCollider;
    [SerializeField]
    private WheelCollider frontRightWheelCollider;
    [SerializeField]
    private WheelCollider rearLeftWheelCollider;
    [SerializeField]
    private WheelCollider rearRightWheelCollider;

    [SerializeField]
    private Transform frontLeftTransform;
    [SerializeField]
    private Transform frontRightTransform;
    [SerializeField]
    private Transform rearLeftTransform;
    [SerializeField]
    private Transform rearRightTransform;

    public Transform sensor;
    private List<Node> shortestPath;
    private Rigidbody rigidB;
    private int currentIndex = 0;
    private float currentSteeringAngle;
    private float _speedlimit = 4.33f;

    private void Start()
    {
        rigidB = GetComponent<Rigidbody>();
        sensor = transform.Find(Strings.distanceSensor);
        shortestPath = Dijkstra.Instance.CalculateShortestPath(Graph.Instance, source, destination);
        speedLimit = maxVelocity;
    }

    private void FixedUpdate()
    {
        Steer();
        Accelerate();
        Brake();

        UpdateWheelTransform(frontLeftWheelCollider, frontLeftTransform);
        UpdateWheelTransform(frontRightWheelCollider, frontRightTransform);
        UpdateWheelTransform(rearLeftWheelCollider, rearLeftTransform);
        UpdateWheelTransform(rearRightWheelCollider, rearRightTransform);

        Sensor();
    }

    private void UpdateWheelTransform(WheelCollider wheelCollider, Transform transform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion quat);
        transform.position = pos;
        transform.rotation = quat;
    }

    bool cantGo() => velocity > speedLimit || isParking || forbiddenTraffic || hasObstacle;
    private void Brake()
    {
        if (cantGo())
        {
            frontLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;
            rearRightWheelCollider.motorTorque = 0;
            rearLeftWheelCollider.motorTorque = 0;

            frontLeftWheelCollider.brakeTorque = brakeTorque;
            frontRightWheelCollider.brakeTorque = brakeTorque;
            rearLeftWheelCollider.brakeTorque = brakeTorque;
            rearRightWheelCollider.brakeTorque = brakeTorque;

            rigidB.drag = 0.4f;
        }
    }

    private void Accelerate()
    {
        velocity = rigidB.velocity.magnitude;
        rigidB.drag = 0.0f;

        if (cantGo())
        {
            frontLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;
            rearRightWheelCollider.motorTorque = 0;
            rearLeftWheelCollider.motorTorque = 0;
        }
        else
        {
            frontLeftWheelCollider.brakeTorque = 0;
            frontRightWheelCollider.brakeTorque = 0;
            rearLeftWheelCollider.brakeTorque = 0;
            rearRightWheelCollider.brakeTorque = 0;

            frontLeftWheelCollider.motorTorque = motorTorque;
            frontRightWheelCollider.motorTorque = motorTorque;
            rearRightWheelCollider.motorTorque = motorTorque;
            rearLeftWheelCollider.motorTorque = motorTorque;
        }
    }

    float SlowDownDistance(float to)
    {
        float velocity = rigidB.velocity.magnitude;
        if (velocity < to)
            return 0;

        float deltaV = velocity - to;
        const float brakeDeceleration = 4.8f;
        float deltaT = deltaV / brakeDeceleration;
        return to * deltaT + deltaT * deltaV / 2;
    }

    private void Steer()
    {
        if (currentIndex < shortestPath.Count - 1)
        {
            Node currentNode = shortestPath[currentIndex];
            float distanceFromCurrentNode = Vector3.Distance(transform.position, currentNode.transform.position);
            if (!isInJunction
                && SlowDownDistance(Junction.speedLimit) > distanceFromCurrentNode
                && currentNode.gameObject.tag == Strings.junctionInTag)
            {
                isInJunction = true;
                speedLimit = Junction.speedLimit;
            }
            if (distanceFromCurrentNode < 3)
            {
                if (currentNode.gameObject.tag == Strings.junctionInTag)
                    currentNode.gameObject.GetComponentInParent<Junction>().Enter(this, (currentNode, shortestPath[currentIndex + 1]));
                if (currentNode.gameObject.tag == Strings.junctionOutTag)
                {
                    speedLimit = maxVelocity;
                    isInJunction = false;
                    currentNode.gameObject.GetComponentInParent<Junction>().Exit(this, (shortestPath[currentIndex - 1], currentNode));
                }
                currentIndex++;
            }
        }

        Vector3 localVector = transform.InverseTransformPoint(shortestPath[currentIndex].transform.position);
        localVector = localVector.normalized;

        currentSteeringAngle = maxSteeringAngle * localVector.x;
        frontLeftWheelCollider.steerAngle = currentSteeringAngle;
        frontRightWheelCollider.steerAngle = currentSteeringAngle;
    }

    private void Sensor()
    {
        sensor.localRotation = Quaternion.Euler(new Vector3(0f, currentSteeringAngle, 0f));
    }

    public void Destroy()
    {
        Destroy(gameObject);
        OnDestroy.Invoke();
        OnDestroy.RemoveAllListeners();
    }
}
