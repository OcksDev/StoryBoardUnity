using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public int dragstate = -1;

    public string ItemToString()
    {
        Position = rt.anchoredPosition;
        Scale = rt.sizeDelta;
        Dictionary<string,string> list = new Dictionary<string,string>() {
            {"UUID", UUID },
            {"Cons", Converter.ListToString(Connections)},
            {"Pos", Position.ToString()},
            {"Scl", Scale.ToString()},
            {"Name", Name.ToString()},
            {"Desc", Desc.ToString()},
        };
        return Converter.EscapedDictionaryToString(list);
    }
    
    public void StringToItem(string ee)
    {
        var e = Converter.EscapedStringToDictionary(ee);
        UUID = e["UUID"];
        Name = e["Name"];
        Desc = e["Desc"];
        rt.anchoredPosition = Converter.StringToVector3(e["Pos"]);
        rt.sizeDelta = Converter.StringToVector3(e["Scl"]);
        Connections = Converter.StringToList(e["Cons"]);
        if (Connections.Contains("")) Connections.Remove("");
    }

    public void UpdateConnectionLines() /
    {
        foreach (var conn in Connections)
        {
            var e = Gamer.Instance.myhomies[conn];
            if (!ConnectionLines.ContainsKey(conn))
            {
                var a = Instantiate(Gamer.Instance.things[3], transform.position, Quaternion.identity, Gamer.Instance.things[2].transform);
                ConnectionLines.Add(conn, a);
            }
            var gm = ConnectionLines[conn];
            if (!e.ConnectionLines.ContainsKey(UUID))
            {
                e.ConnectionLines.Add(UUID, gm);
            }
            gm.transform.position = Vector3.Lerp(transform.position, e.transform.position, 0.5f);
            gm.transform.rotation = RandomFunctions.PointAtPoint2D(transform.position, e.transform.position, 0);
            gm.transform.localScale = new Vector3(RandomFunctions.Dist(transform.position, e.transform.position) * 0.8f / Viewport.Instance.scalem, 1, 1);
        }
    }


    public void Update()
    {
        bool ww = Gamer.Instance.IsHovering(gameObject);
        var ee = Input.GetKey(KeyCode.Mouse0);
        bool rr = false;
        if (washov || ww)
        {
            for(int i = 0; i < 4; i++)
            {
                bool smex = Gamer.Instance.IsHovering(refs[i]);
                if (smex)
                {
                    if(ee && Gamer.Instance.CurrentMouse == Gamer.MouseState.None) dragstate = i;
                    ww = true;
                    rr = true;
                    break;
                }
            }
        }

        if (!rr && ee && ww && dragstate == -1 && Gamer.Instance.CurrentMouse == Gamer.MouseState.None)
        {
            dragstate = 4;
        }

        if (ww && dragstate == -1 && Gamer.Instance.CurrentMouse == Gamer.MouseState.None && Input.GetKeyDown(KeyCode.Q))
        {
            Gamer.Instance.CurrentMouse = Gamer.MouseState.Connecting;
            Gamer.Instance.connecting_uuid = UUID;
        }
        else if (ww && ee && Gamer.Instance.CurrentMouse == Gamer.MouseState.Connecting && Gamer.Instance.connecting_uuid != UUID)
        {
            var nerd = Gamer.Instance.myhomies[Gamer.Instance.connecting_uuid];
            Connections.Add(Gamer.Instance.connecting_uuid);
            nerd.Connections.Add(UUID);
            Gamer.Instance.CurrentMouse = Gamer.MouseState.None;
            UpdateConnectionLines();
        }

        if (dragstate != -1)
        {

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
}
