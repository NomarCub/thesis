using UnityEngine;

public class DestinationForCars : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Strings.car)
        {
            other.gameObject.GetComponent<CarController>().Destroy();
        }
    }
}
