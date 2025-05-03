using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeObject : MonoBehaviour
{
    public string UUID;
    public List<string> Connections = new List<string>();
    public Vector3 Position;
    public Vector3 Scale;
    public List<GameObject> refs = new List<GameObject> ();
    public Dictionary<string,GameObject> ConnectionLines = new Dictionary<string, GameObject>();
    bool washov = false;
    public RectTransform rt;

    public TextMeshProUGUI titler;
    public Image selff;

    public string Name = "";
    public string Desc = "";
    public Color Color;


    private void Start()
    {
        dragstate = -1;
        for (int i = 0; i < 4; i++)
        {
            refs[i].SetActive(false);
        }
        UpdateDisplay();
    }

    public int dragstate = -1;

    public Dictionary<string,string> GetBase()
    {
        return new Dictionary<string, string>() {
            {"UUID", ""},
            {"Cons", ""},
            {"Pos", ""},
            {"Scl", ""},
            {"Name", ""},
            {"Desc", ""},
            {"Col", ""},
        };
    }


    public string ItemToString()
    {
        Position = rt.anchoredPosition;
        Scale = rt.sizeDelta;
        var cd = GetBase();
        Dictionary<string,string> list = new Dictionary<string,string>() {
            {"UUID", UUID },
            {"Cons", Converter.ListToString(Connections)},
            {"Pos", Position.ToString()},
            {"Scl", Scale.ToString()},
            {"Name", Name.ToString()},
            {"Desc", Desc.ToString()},
            {"Col", ColorUtility.ToHtmlStringRGB(Color)},
        };
        foreach(var a in list)
        {
            cd[a.Key] = a.Value;
        }
        return Converter.EscapedDictionaryToString(cd);
    }
    
    public void StringToItem(string ee)
    {
        var e = GetBase();
        var wank = Converter.EscapedStringToDictionary(ee);
        foreach(var a in wank)
        {
            e[a.Key] = a.Value;
        }
        UUID = e["UUID"];
        Debug.Log(e["UUID"]);
        Name = e["Name"];
        Desc = e["Desc"];
        Color = Converter.StringToColor(e["Col"]);
        rt.anchoredPosition = Converter.StringToVector3(e["Pos"]);
        rt.sizeDelta = Converter.StringToVector3(e["Scl"]);
        Connections = Converter.StringToList(e["Cons"]);
        if (Connections.Contains("")) Connections.Remove("");
    }

    public void UpdateConnectionLines()
    {
        List<string> aa = new List<string>();
        foreach(var a in Connections)
        {
            if (!aa.Contains(a))
            {
                if (Gamer.Instance.myhomies.ContainsKey(a))
                {
                    aa.Add(a);
                }
                else
                {
                    if (ConnectionLines.ContainsKey(a))
                    {
                        Destroy(ConnectionLines[a]);
                        ConnectionLines.Remove(a);
                    }
                }
            }
        }
        if(aa.Contains(UUID)) aa.Remove(UUID);
        Connections = aa;
        foreach (var conn in Connections)
        {
            var e = Gamer.Instance.myhomies[conn];
            if (!ConnectionLines.ContainsKey(conn))
            {
                var a = Instantiate(Gamer.Instance.things[3], transform.position, Quaternion.identity, Gamer.Instance.things[2].transform);
                a.GetComponent<LineNerd>().oneparentuuid = UUID;
                ConnectionLines.Add(conn, a);
            }
            var gm = ConnectionLines[conn];
            if (!e.ConnectionLines.ContainsKey(UUID))
            {
                e.ConnectionLines.Add(UUID, gm);
                gm.GetComponent<LineNerd>().otherparentuuid = e.UUID;
            }
            LineAllign(gm, transform.position, e.transform.position);
        }
    }
    public static void LineAllign(GameObject gm, Vector3 pos, Vector3 otherpos)
    {
        gm.transform.position = Vector3.Lerp(pos, otherpos, 0.5f);
        gm.transform.rotation = RandomFunctions.PointAtPoint2D(pos, otherpos, 0);
        gm.transform.localScale = new Vector3(RandomFunctions.Dist(pos, otherpos) * 0.8f / Viewport.Instance.scalem, 1, 1);
    }

    public void Update()
    {
        bool ww = Gamer.Instance.IsHovering(gameObject);
        var ee = InputManager.IsKey(KeyCode.Mouse0, "Game");
        bool rr = false;
        var dd = dragstate == -1 && Gamer.Instance.CurrentMouse == Gamer.MouseState.None;
        if (washov || ww)
        {
            for(int i = 0; i < 4; i++)
            {
                bool smex = Gamer.Instance.IsHovering(refs[i]);
                if (smex)
                {
                    if(ee && dd) dragstate = i;
                    ww = true;
                    rr = true;
                    break;
                }
            }
        }

        if (!rr && ee && ww && dd)
        {
            dragstate = 4;
        }


        if (ww && dd && InputManager.IsKeyDown(KeyCode.Mouse2, "Game"))
        {
            Gamer.Instance.OpenEditorMenu(UUID);
        }


        if (ww && dd && InputManager.IsKeyDown(KeyCode.Q, "Game"))
        {
            Gamer.Instance.CurrentMouse = Gamer.MouseState.Connecting;
            Gamer.Instance.connecting_uuid = UUID;
            Gamer.Instance.FakeLine = Instantiate(Gamer.Instance.things[3], transform.position, transform.rotation, Gamer.Instance.things[2].transform);
        }
        else if (ww && Gamer.Instance.CurrentMouse == Gamer.MouseState.Connecting && (InputManager.IsKeyDown(KeyCode.Q, "Game")|| InputManager.IsKeyDown(KeyCode.Mouse0, "Game")) && Gamer.Instance.connecting_uuid != UUID)
        {
            CompleteLineConnect();
        }

        if(ww && InputManager.IsKeyDown(KeyCode.Backspace, "Game"))
        {
            foreach (var a in Connections)
            {
                if(!Gamer.Instance.myhomies.ContainsKey(a)) { continue; }
                var aa = Gamer.Instance.myhomies[a];
                aa.Connections.Remove(a);
                Destroy(ConnectionLines[aa.UUID]);
                aa.UpdateConnectionLines();
            }

            if(Gamer.Instance.connecting_uuid == UUID)
            {
                Gamer.Instance.connecting_uuid = "";
                Gamer.Instance.CurrentMouse = Gamer.MouseState.None;
                Destroy(Gamer.Instance.FakeLine);
            }
            Gamer.Instance.myhomies.Remove(UUID);
            Destroy(gameObject);
            return;
        }



        if (dragstate != -1)
        {
            Gamer.Instance.connecting_uuid = UUID;
            var eee = rt.sizeDelta;
            var pos = rt.anchoredPosition;
            switch (dragstate)
            {
                case 0:
                    eee.x += Viewport.Instance.MouseOffset.x;
                    eee.y -= Viewport.Instance.MouseOffset.y;
                    pos.x += Viewport.Instance.MouseOffset.x / 2;
                    pos.y += Viewport.Instance.MouseOffset.y / 2;
                    break;
                case 1:
                    eee.x -= Viewport.Instance.MouseOffset.x;
                    eee.y -= Viewport.Instance.MouseOffset.y;
                    pos.x += Viewport.Instance.MouseOffset.x / 2;
                    pos.y += Viewport.Instance.MouseOffset.y / 2;
                    break;
                case 2:
                    eee.x += Viewport.Instance.MouseOffset.x;
                    eee.y += Viewport.Instance.MouseOffset.y;
                    pos.x += Viewport.Instance.MouseOffset.x / 2;
                    pos.y += Viewport.Instance.MouseOffset.y / 2;
                    break;
                case 3:
                    eee.x -= Viewport.Instance.MouseOffset.x;
                    eee.y += Viewport.Instance.MouseOffset.y;
                    pos.x += Viewport.Instance.MouseOffset.x / 2;
                    pos.y += Viewport.Instance.MouseOffset.y / 2;
                    break;
                case 4:
                    pos.x += Viewport.Instance.MouseOffset.x;
                    pos.y += Viewport.Instance.MouseOffset.y;
                    break;
            }
            rt.sizeDelta = eee;
            rt.anchoredPosition = pos;
            UpdateConnectionLines();
        }



        if (ww || dragstate != -1)
        {
            Gamer.Instance.hov_uuid = UUID;
            if(!washov)
            {
                for (int i = 0; i < 4; i++)
                {
                    refs[i].SetActive(true);
                }
            }
            washov = true;
        }
        else
        {
            if (washov)
            {
                for (int i = 0; i < 4; i++)
                {
                    refs[i].SetActive(false);
                }
            }
            washov = false;
        }
        if(dragstate != -1)
        {
            Gamer.Instance.CurrentMouse = Gamer.MouseState.Dragging;
            if (!ee)
            {
                Gamer.Instance.CurrentMouse = Gamer.MouseState.None;
                dragstate = -1;
            }
        }
    }

    public void CompleteLineConnect()
    {
        var nerd = Gamer.Instance.myhomies[Gamer.Instance.connecting_uuid];
        Connections.Add(Gamer.Instance.connecting_uuid);
        nerd.Connections.Add(UUID);
        Destroy(Gamer.Instance.FakeLine);
        Gamer.Instance.connecting_uuid = "";
        Gamer.Instance.CurrentMouse = Gamer.MouseState.DraggingWait;
        UpdateConnectionLines();
    }

    public void UpdateDisplay()
    {
        titler.text = Name;
        selff.color = Color;
    }

}
