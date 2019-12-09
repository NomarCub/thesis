using UnityEngine;

public class ParkingLotEntry : MonoBehaviour
{
    [SerializeField]
    private float speedLimit = 2.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Strings.car)
        {
            CarController carController = other.GetComponent<CarController>();

            carController.speedLimit = speedLimit;
        }
    }
}
