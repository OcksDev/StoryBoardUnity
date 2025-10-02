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
        Title.text = $"<color=#{Converter.ColorToString(Gamer.Instance.color32s[Value+3])}>[{Value}]</color> {Nm}";
        ExportButton.SetActive(!Gamer.Instance.Personalities.Contains(dd.a));
    }

    public void SliderSex()
    {
        SetVal((int)Slider.value);
    }

    public void RemoveMe()
    {
        characterData.personalities.Remove(dd);
        Gamer.Instance.ReloadPersonalityList(Gamer.Instance.suggin);
    }
    public void AddMe()
    {
        Gamer.Instance.Personalities.Add(dd.a);
        UpdateName();
        Gamer.Instance.ReloadPersonalityList(Gamer.Instance.suggin);
    }
    public void MoveMe(bool up)
    {
        var i = characterData.personalities.IndexOf(dd);
        if (i <= 0 && up) return;
        if (i >= characterData.personalities.Count - 1 && !up) return;
        if (up)
        {
            characterData.personalities.RemoveAt(i);
            characterData.personalities.Insert(i - 1, dd);
        }
        else
        {
            characterData.personalities.RemoveAt(i);
            characterData.personalities.Insert(i+1, dd);
        }

        Gamer.Instance.ReloadPersonalityList(Gamer.Instance.suggin);
    }
}
