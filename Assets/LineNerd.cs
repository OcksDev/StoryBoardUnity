using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineNerd : MonoBehaviour
{
    public string oneparentuuid = "";
    public string otherparentuuid = "";
    // Update is called once per frame
    void Update()
    {
        bool ww = Gamer.Instance.IsHovering(gameObject);
        if (ww && InputManager.IsKeyDown(KeyCode.Backspace, "Game"))
        {
            var c = Gamer.Instance.myhomies[oneparentuuid];
            var c2 = Gamer.Instance.myhomies[otherparentuuid];

            c.Connections.Remove(otherparentuuid);
            c2.Connections.Remove(oneparentuuid);

            c.ConnectionLines.Remove(otherparentuuid);
            c2.ConnectionLines.Remove(oneparentuuid);

            Destroy(gameObject);

            c.UpdateConnectionLines();
            c2.UpdateConnectionLines();
        }
    }
}
