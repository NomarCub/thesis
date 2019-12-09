using UnityEngine;

public class PedestrianSensor : MonoBehaviour
{
    [SerializeField]
    private GameObject pedestrian = null;
    private GameObject pedestrianCrossing = null;

    private void Update()
    {
        if (pedestrianCrossing != null)
        {
            PedestrianCrossingController pedestrianCrossingController = pedestrianCrossing.GetComponent<PedestrianCrossingController>();
            PedestrianController pedestrianController = pedestrian.GetComponent<PedestrianController>();

            if (pedestrianCrossingController.carCounter > 0 || pedestrianCrossingController.dangerous)
            {
                pedestrianController.stop = true;
            }
            else
            {
                pedestrianController.stop = false;
            }

            if (pedestrianCrossingController.carCounter > 0 && pedestrianCrossingController.pedestrianCounter > 0)
            {
                pedestrianController.reversed = true;
            }
            else
            {
                pedestrianController.reversed = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Strings.pedestrianCrossing)
        {
            pedestrianCrossing = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == Strings.pedestrianCrossing)
        {
            PedestrianController pedestrianController = pedestrian.GetComponent<PedestrianController>();
            pedestrianCrossing = null;
            pedestrianController.reversed = false;
            pedestrianController.stop = false;
        }
    }
}
