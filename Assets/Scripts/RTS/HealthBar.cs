using RTS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour {
    public Transform defaultOwner;

    private IHealth owner;
    [SerializeField]
    private Slider slider;

    private Color? barColor;

    public Color BarColor
    {
        get
        {
            return barColor ?? Color.black;
        }
        set
        {
            barColor = value;
            SetBarColor(value);
        }
    }



    public IHealth Owner
    {
        get
        {
            return owner;
        }
        set
        {
            if(owner != null)
                owner.OnHealthChanged -= SetHealth;
            this.owner = value;
            owner.OnHealthChanged += SetHealth;
        }
    }



    void Awake()
    {
        if (defaultOwner != null)
            owner = defaultOwner.GetComponent<IHealth>();
        if(owner != null)
            owner.OnHealthChanged += SetHealth;
        if(slider == null)
            slider = GetComponent<Slider>();

        if (barColor == null)
            barColor = slider.colors.normalColor;
        else
            SetBarColor(BarColor);
    }

    void SetHealth(float health)
    {
        slider.value = health / owner.MaxHealth;
    }


    void SetBarColor(Color color)
    {
        if (slider == null)
            return;
        slider.fillRect.GetComponent<Image>().color = color;
    }
}
