using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class perssugg : MonoBehaviour
{
    public TextMeshProUGUI Display;
    public string mydata;
    public bool IsAv;
    public void SetData(string aa)
    {
        Display.text = aa;
        mydata = aa.ToLower();
        IsAv = true;
    }
    public void AmCool(string aa)
    {
        aa = aa.ToLower();
        IsAv = aa == "" || mydata.StartsWith(aa);
        gameObject.SetActive(IsAv);
    }
    public void ClickMe()
    {
        var dd = Tags.refs["sugg_input"].GetComponent<TMP_InputField>();
        dd.text = Gamer.killme(mydata);
        Gamer.Instance.SubmitNewPers();
    }
}
