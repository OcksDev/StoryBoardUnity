using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PersonalityOb : MonoBehaviour
{
    public CharacterData characterData;
    public TextMeshProUGUI Title;
    public Slider Slider;
    public int Value;
    public GameObject ExportButton;
    public List<Color32> color32s = new List<Color32>();
    public string Nm;
    private PersonalityRef<string,int> dd;
    public void SetFromData(PersonalityRef<string,int> dd)
    {
        this.dd = dd;
        Nm = dd.a;
        SetVal(dd.b);
    }
    public void SetVal(int val)
    {
        Value = val;
        Slider.value = Value;

        if(characterData != null) characterData.personalities[characterData.personalities.IndexOf(dd)].b = val;
        UpdateName();
    }
    public void UpdateName()
    {
        Title.text = $"<color=#{Converter.ColorToString(color32s[Value+3])}>[{Value}]</color> {Nm}";
    }

    public void SliderSex()
    {
        SetVal((int)Slider.value);
    }

}
