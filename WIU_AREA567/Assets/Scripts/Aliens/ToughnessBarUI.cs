using UnityEngine;
using UnityEngine.UI;

public class ToughnessBarUI : MonoBehaviour
{
    public Slider slider;
    public BossDamageable target;

    public void RefreshBar()
    {
        slider.value = (float)target.CurrentToughness / target.maxToughness;
    }
}
