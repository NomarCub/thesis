using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    Material greenLight, redLight, yellowLight;

    Color brightGreen, brightRed, brightYellow;

    private bool green_ = false, red_ = false, yellow_ = false;

    public bool green
    {
        get => green_;
        set
        {
            if (value && !green_)
                greenLight.SetColor(Strings.emissionColorString, brightGreen);
            else if (!value && green_)
                greenLight.SetColor(Strings.emissionColorString, Color.black);
            green_ = value;
        }
    }
    public bool red
    {
        get => red_;
        set
        {
            if (value && !red_)
                redLight.SetColor(Strings.emissionColorString, brightRed);
            else if (!value && red_)
                redLight.SetColor(Strings.emissionColorString, Color.black);
            red_ = value;
        }
    }
    public bool yellow
    {
        get => yellow_;
        set
        {
            if (value && !yellow_)
                yellowLight.SetColor(Strings.emissionColorString, brightYellow);
            else if (!value && yellow_)
                yellowLight.SetColor(Strings.emissionColorString, Color.black);
            yellow_ = value;
        }
    }

    void Awake()
    {
        var lights = gameObject.GetComponentsInChildren<Renderer>();
        greenLight = lights[0].material;
        redLight = lights[1].material;
        yellowLight = lights[2].material;

        brightGreen = greenLight.GetColor(Strings.emissionColorString);
        brightRed = redLight.GetColor(Strings.emissionColorString);
        brightYellow = yellowLight.GetColor(Strings.emissionColorString);

        greenLight.SetColor(Strings.emissionColorString, Color.black);
        redLight.SetColor(Strings.emissionColorString, Color.black);
        yellowLight.SetColor(Strings.emissionColorString, Color.black);
    }

}