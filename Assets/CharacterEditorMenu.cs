using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEditorMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse2))
        {
            Gamer.Instance.CloseCharacterEditorMenu(Gamer.Instance.nerd_uuid);
        }
    }
}
