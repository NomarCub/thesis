using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private List<Node> sourcesForCars;
    [SerializeField]
    private List<Node> destinationsForCars;
    [SerializeField]
    private GameObject carPrefab;
    [SerializeField]
    private GameObject pedestrianPrefab;
    [SerializeField]
    private List<Transform> sourcesForPedestrians;
    [SerializeField]
    private float spawnTimeForCars = 1f;
    [SerializeField]
    private float spawnTimeForPedestrians = 1f;
    [SerializeField]
    private int carCounter = 0;
    [SerializeField]
    private int pedestrianCounter = 0;
    [SerializeField]
    private int maxCarsOnScreen = 10;
    [SerializeField]
    private int maxPedestriansOnScreen = 10;
    private float currentSpawnTimeForCars = 0f;
    private float currentSpawnTimeForPedestrians = 0f;

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

    private void Update()
    {
        currentSpawnTimeForCars += Time.deltaTime;
        currentSpawnTimeForPedestrians += Time.deltaTime;

        if (currentSpawnTimeForCars > spawnTimeForCars && carCounter < maxCarsOnScreen)
        {
            SpawnCar();
            currentSpawnTimeForCars = 0f;
        }

        if (currentSpawnTimeForPedestrians > spawnTimeForPedestrians && pedestrianCounter < maxPedestriansOnScreen)
        {
            SpawnPedestrian();
            currentSpawnTimeForPedestrians = 0f;
        }
    }

    private void SpawnCar()
    {
        carPrefab.SetActive(false);

        Node randomSource = sourcesForCars[Random.Range(0, sourcesForCars.Count)];
        Node randomDestination = destinationsForCars[Random.Range(0, destinationsForCars.Count)];

        while (randomSource.GetComponent<Source>().busy)
        {
            randomSource = sourcesForCars[Random.Range(0, sourcesForCars.Count)];
        }

        if (randomDestination.gameObject.tag == Strings.destinationForParking)
        {
            destinationsForCars.Remove(randomDestination);
        }

        GameObject car = Instantiate(carPrefab, randomSource.transform.position, Quaternion.identity);
        CarController carController = car.GetComponent<CarController>();

        carController.source = randomSource;
        carController.destination = randomDestination;
        carController.OnDestroy.AddListener(CarDestroyed);

        switch (randomSource.gameObject.tag)
        {
            case Strings.sourceSouth:
                break;
            case Strings.sourceWest:
                car.transform.Rotate(0f, 90f, 0f);
                break;
            case Strings.sourceEast:
                car.transform.Rotate(0f, -90f, 0f);
                break;
            case Strings.sourceNorth:
                car.transform.Rotate(0f, 180f, 0f);
                break;
            default:
                break;
        }

        carCounter++;
        car.SetActive(true);
    }

    private void SpawnPedestrian()
    {
        pedestrianPrefab.SetActive(false);

        Transform randomSource = sourcesForPedestrians[Random.Range(0, sourcesForPedestrians.Count)];

        while (randomSource.GetComponent<SourceForPedestrians>().busy)
        {
            randomSource = sourcesForPedestrians[Random.Range(0, sourcesForPedestrians.Count)];
        }

        GameObject pedestrian = Instantiate(pedestrianPrefab, randomSource.position, Quaternion.identity);

        PedestrianController pedestrianController = pedestrian.GetComponent<PedestrianController>();
        pedestrianController.OnDestroy.AddListener(PedestrianDestroyed);

        pedestrian.transform.position = new Vector3(randomSource.transform.position.x, 0.1f, randomSource.transform.position.z);

        switch (randomSource.gameObject.tag)
        {
            case Strings.sourceWest:
                pedestrian.transform.Rotate(0f, 90f, 0f);
                break;
            case Strings.sourceEast:
                pedestrian.transform.Rotate(0f, -90f, 0f);
                break;
            default:
                break;
        }

        pedestrianCounter++;
        pedestrian.SetActive(true);
    }

    public void CarDestroyed()
    {
        carCounter--;
    }

    public void PedestrianDestroyed()
    {
        pedestrianCounter--;
    }
}
