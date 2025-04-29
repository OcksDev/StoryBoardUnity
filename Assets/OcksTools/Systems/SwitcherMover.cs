using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwitcherMover : MonoBehaviour
{
    public TextMeshProUGUI top;
    public TextMeshProUGUI bottom;
    public TextMeshProUGUI select;
    public int selection = 0;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S)|| Input.GetKeyDown(KeyCode.DownArrow)) AttemptSelectionDown();
        if(Input.GetKeyDown(KeyCode.W)|| Input.GetKeyDown(KeyCode.UpArrow)) AttemptSelectionUp();
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Gamer.Instance.SaveAll();
            var wanker = Gamer.AvailableGraphs[selection];
            Gamer.Instance.CloseSwitcher();
            if(wanker != Gamer.Instance.CurrentBoard)Gamer.Instance.LoadAll(wanker);
        }
    }


    public void AttemptSelectionUp()
    {
        selection = RandomFunctions.Mod(selection - 1, Gamer.AvailableGraphs.Count);
        UpdateDisplay();
    }
    public void AttemptSelectionDown()
    {
        selection = RandomFunctions.Mod(selection + 1, Gamer.AvailableGraphs.Count);
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        List<string> before = new List<string>();
        List<string> after = new List<string>();
        Debug.Log(selection + Gamer.Instance.CurrentBoard);
        select.text = Gamer.AvailableGraphs[selection];
        for(int i = 0; i < Gamer.AvailableGraphs.Count; i++)
        {
            if (i < selection) before.Add(Gamer.AvailableGraphs[i]);
            else if (i > selection) after.Add(Gamer.AvailableGraphs[i]);
        }

        top.text = Converter.ListToString(before, "<br>");
        bottom.text = Converter.ListToString(after, "<br>");
    }

}
