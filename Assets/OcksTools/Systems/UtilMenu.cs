using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilMenu : MonoBehaviour
{
    public void Tingle()
    {
        Gamer.Instance.ToggleUtilMenu();
    }
}
