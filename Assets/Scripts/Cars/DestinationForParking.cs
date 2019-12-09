using UnityEngine;

public class DestinationForParking : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Strings.car)
        {
            CarController carController = other.GetComponent<CarController>();
            carController.isParking = true;
            other.GetComponentInChildren<DistanceSensor>().gameObject.SetActive(false);
        }        
    }
}
