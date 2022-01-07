using UnityEngine;
using UnityEngine.UI;

public class CollectibleUI : MonoBehaviour
{

    public Slider slider;


    public void Update()
    {
        slider.value -= Time.deltaTime;
        if(slider.value <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void StartSlider(float duration)
    {
        slider.maxValue = duration;
        slider.value = duration;
        this.transform.SetAsLastSibling();
        this.gameObject.SetActive(true);
    }
}
