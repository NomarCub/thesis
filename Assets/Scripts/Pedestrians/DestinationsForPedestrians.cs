using UnityEngine;

public class DestinationsForPedestrians : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Strings.pedestrian)
        {
            other.GetComponent<PedestrianController>().Destroy();
        }
    }
}
