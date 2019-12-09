using UnityEngine;

public class SourceForPedestrians : Source
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Strings.pedestrian)
        {
            busy = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == Strings.pedestrian)
        {
            busy = false;
        }
    }
}

