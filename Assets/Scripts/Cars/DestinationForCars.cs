using UnityEngine;

public class DestinationForCars : MonoBehaviour
{
    public bool available = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Strings.car)
        {
            other.gameObject.GetComponent<CarController>().Destroy();
        }
    }
}