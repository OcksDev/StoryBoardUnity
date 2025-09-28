using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterEditorMenu : MonoBehaviour
{
    public TMP_InputField Title;
    public TMP_InputField Description;
    public TMP_InputField ColorHex;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse2))
        {
            Gamer.Instance.CloseCharacterEditorMenu(Gamer.Instance.nerd_uuid);
        }
    }
}
