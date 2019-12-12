using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class CarController : MonoBehaviour
{
    private const float rayHeight = 0.8f;
    private const float laneDistance = 5f;

    public const float FastestCarVelocity = 14f;
    public const float SlowestCarVelocity = 4f;
    public static int currentID = 0;
    public int ID;

    public override bool Equals(object obj) => obj is CarController controller && ID == controller.ID;
    public override int GetHashCode() => ID;

    private void Awake()
    {
        ID = Interlocked.Increment(ref currentID);
    }

    public float maxVelocity = 4.33f;
    private float _speedlimit = 4.33f;
    public float speedLimit
    {
        get => _speedlimit;
        set => _speedlimit = value < maxVelocity ? value : maxVelocity;
    }
    [SerializeField]
    private float velocity;

    public Node destination = null;
    public Node source = null;
    private List<Node> shortestPath;
    private int currentNodeIndex = 0;

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

    public DistanceSensor sensor;
    public Transform sensorTransform;
    private Rigidbody rigidB;
    private float currentSteeringAngle;
    public UnityEvent OnDestroy;

    private void Start()
    {
        rigidB = GetComponent<Rigidbody>();
        sensorTransform = transform.Find(Strings.distanceSensor);
        sensor = gameObject.GetComponentInChildren<DistanceSensor>();
        shortestPath = Dijkstra.Instance.CalculateShortestPath(Graph.Instance, source, destination);
        maxVelocity = ID % 3 == 0 ? FastestCarVelocity : SlowestCarVelocity;
        speedLimit = maxVelocity;
    }

    private void FixedUpdate()
    {
        velocity = rigidB.velocity.magnitude;
        Steer();
        if (cantGo())
            Brake();
        else
            Accelerate();

        UpdateWheelTransform(frontLeftWheelCollider, frontLeftTransform);
        UpdateWheelTransform(frontRightWheelCollider, frontRightTransform);
        UpdateWheelTransform(rearLeftWheelCollider, rearLeftTransform);
        UpdateWheelTransform(rearRightWheelCollider, rearRightTransform);

        RotateSensor();
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

    private void Accelerate()
    {
        frontLeftWheelCollider.motorTorque = motorTorque;
        frontRightWheelCollider.motorTorque = motorTorque;
        rearRightWheelCollider.motorTorque = motorTorque;
        rearLeftWheelCollider.motorTorque = motorTorque;

        frontLeftWheelCollider.brakeTorque = 0;
        frontRightWheelCollider.brakeTorque = 0;
        rearLeftWheelCollider.brakeTorque = 0;
        rearRightWheelCollider.brakeTorque = 0;

        rigidB.drag = 0.0f;
    }

    private void Steer()
    {
        Node currentNode = shortestPath[currentNodeIndex];
        if (currentNodeIndex < shortestPath.Count - 1)
        {
            float distanceFromCurrentNode = Vector3.Distance(transform.position, currentNode.transform.position);
            if (!isInJunction
                && PhysicsCalc.SlowDownDistance(rigidB.velocity.magnitude, Junction.speedLimit) > distanceFromCurrentNode
                && currentNode.gameObject.tag == Strings.junctionInTag)
            {
                isInJunction = true;
                speedLimit = Junction.speedLimit;
                overtakeInfo.state = OvertakeInfo.State.None;
            }
            if (distanceFromCurrentNode < 3)
            {
                if (currentNode.gameObject.tag == Strings.junctionInTag)
                    currentNode.gameObject.GetComponentInParent<Junction>().Enter(this, (currentNode, shortestPath[currentNodeIndex + 1]));
                if (currentNode.gameObject.tag == Strings.junctionOutTag)
                {
                    speedLimit = maxVelocity;
                    isInJunction = false;
                    currentNode.gameObject.GetComponentInParent<Junction>().Exit(this, (shortestPath[currentNodeIndex - 1], currentNode));
                }
                currentNodeIndex++;
            }
        }

        if (overtakeInfo.state == OvertakeInfo.State.None)
            steerTowards(currentNode.transform.position);
        else
            Overtake();
    }

    void steerTowards(Vector3 position)
    {
        Vector3 localVector = transform.InverseTransformPoint(position).normalized;

        currentSteeringAngle = maxSteeringAngle * localVector.x;
        frontLeftWheelCollider.steerAngle = currentSteeringAngle;
        frontRightWheelCollider.steerAngle = currentSteeringAngle;
    }

    internal bool TryOvertake(CarController otherCar)
    {
        if (maxVelocity < otherCar.maxVelocity + 4
                || otherCar.velocity < SlowestCarVelocity * 0.5
                || isInJunction
                || cantGo()
                || overtakeInfo.state != OvertakeInfo.State.None
                || overtakeInfo.lastTried + 0.5f > Time.fixedTime)
            return false;

        Node currentNode = shortestPath[currentNodeIndex];
        Node previousNode = shortestPath[currentNodeIndex - 1];
        float distanceToJunction = Vector3.Distance(transform.position, currentNode.transform.position);
        float minOvertakeDist = PhysicsCalc.OvertakeDistance(maxVelocity, otherCar.maxVelocity);
        if (distanceToJunction < minOvertakeDist)
            return false;

        var rightLane = previousNode.transform.position;
        rightLane.y = rayHeight;
        var forwardDir = currentNode.transform.position - rightLane;
        forwardDir.y = 0;
        forwardDir.Normalize();
        var leftDir = new Vector3(-forwardDir.z, 0, forwardDir.x);
        var rightDir = -leftDir;
        var leftLane = rightLane + leftDir * laneDistance;

        var rayFrom = gameObject.transform.position + leftDir * laneDistance;
        rayFrom.y = rayHeight;
        RaycastHit hit;
        float rayDist = minOvertakeDist + FastestCarVelocity * (minOvertakeDist / maxVelocity);
        Debug.DrawRay(rayFrom - leftDir * 0.1f, forwardDir * rayDist, Color.red, 0.5f, false);
        if (Physics.Raycast(rayFrom, forwardDir, out hit, rayDist))
        {
            var hitTag = hit.collider.gameObject.tag;
            if (hitTag == Strings.car || hitTag == Strings.pedestrianCrossing)
                return false;
        }

        Debug.DrawRay(rightLane + rightDir * 0.1f, forwardDir * 30, Color.white, 1.5f, false);
        Debug.DrawRay(leftLane + leftDir * 0.1f, forwardDir * 30, Color.black, 1.5f, false);

        Debug.DrawRay(rayFrom, forwardDir * rayDist, Color.green, 1f, false);

        overtakeInfo = new OvertakeInfo()
        {
            car = this,
            otherCar = otherCar,
            forwardDir = forwardDir,
            rightDir = rightDir,
            leftDir = leftDir,
            rightLane = rightLane,
            leftLane = leftLane,
            lastTried = Time.fixedTime,
            state = OvertakeInfo.State.KeepLeft
        };

        return true;
    }

    public class OvertakeInfo
    {
        public enum State { None, KeepRight, KeepLeft };
        private State _state = State.None;
        public State state
        {
            get => _state;
            set
            {
                Debug.Log($"car {car?.ID} {value}");
                _state = value;
            }
        }
        public CarController car;
        public CarController otherCar;

        public float lastTried = 0;
        public Vector3 rightLane;
        public Vector3 leftLane;
        public Vector3 forwardDir;
        public Vector3 leftDir;
        public Vector3 rightDir;
    }

    public OvertakeInfo overtakeInfo = new OvertakeInfo();

    void Overtake()
    {
        switch (overtakeInfo.state)
        {
            case OvertakeInfo.State.KeepLeft:
                {
                    var dest = PhysicsCalc.ProjectPointOnLine(
                        transform.position,
                        overtakeInfo.forwardDir,
                        overtakeInfo.leftLane)
                            + overtakeInfo.forwardDir * 6;
                    Debug.DrawRay(transform.position, dest - transform.position, Color.blue, 0.5f);
                    steerTowards(dest);
                    if (overtakeInfo.otherCar == null || PhysicsCalc.IsBehind(
                            transform.position,
                            overtakeInfo.forwardDir,
                            overtakeInfo.otherCar.transform.position + overtakeInfo.forwardDir * 2))
                        overtakeInfo.state = OvertakeInfo.State.KeepRight;
                }
                break;
            case OvertakeInfo.State.KeepRight:
                {
                    var dest = PhysicsCalc.ProjectPointOnLine(
                        transform.position,
                        overtakeInfo.forwardDir,
                        overtakeInfo.rightLane)
                            + overtakeInfo.forwardDir * 12;
                    Debug.DrawRay(transform.position, dest - transform.position, Color.blue, 0.5f);
                    steerTowards(dest);

                    if (overtakeInfo.otherCar == null || PhysicsCalc.IsToRight(
                            transform.position,
                            overtakeInfo.forwardDir,
                            overtakeInfo.leftLane + overtakeInfo.rightDir * laneDistance * 0.8f))
                        overtakeInfo.state = OvertakeInfo.State.None;
                }
                break;
        }
    }
    private void RotateSensor()
    {
        sensorTransform.localRotation = Quaternion.Euler(new Vector3(0f, currentSteeringAngle, 0f));
    }

    public void Destroy()
    {
        Destroy(gameObject);
        OnDestroy.Invoke();
        OnDestroy.RemoveAllListeners();
    }
}