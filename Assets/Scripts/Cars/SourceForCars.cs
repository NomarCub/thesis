using UnityEngine;

public class SourceForCars : Source
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Strings.car)
        {
            busy = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == Strings.car)
        {
            busy = false;
        }
    }
}
