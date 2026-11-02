using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    public Material mat;
    float red;
    float green;
    float blue;
    
    void Start()
    {
       red = redSlider.value = LivesCounter.instance.red;
        green = greenSlider.value = LivesCounter.instance.green;
       blue = blueSlider.value = LivesCounter.instance.blue;

    }
    public void ChangeRedVal()
    {
        red = redSlider.value;
    }

    public void ChangeGreenVal()
    {
        green = greenSlider.value;

    }

    public void ChangeBlueVal()
    {
        blue = blueSlider.value;
    }
    public void SetMaterial()
    {
        mat.SetColor("_Tint", new Color(red, green, blue));
    }

    public void SaveColors()
    {
        LivesCounter.instance.SetGlobalTint(new Color(red, green, blue));
    }

}
