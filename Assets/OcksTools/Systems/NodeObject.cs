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
    public string NodeType = "";
    /* NodeTypes: 
     * Node
     * Char
     */
    public SpecialNodeData NodeData = null;
    public Color Color;

    public A GetMyData<A>() where A : SpecialNodeData
    {
        return (A)NodeData;
    }

    private void Start()
    {
        dragstate = -1;
        for (int i = 0; i < 4; i++)
        {
            refs[i].SetActive(false);
        }
        UpdateDisplay();
        int_size = rt.sizeDelta;
        int_pos = rt.anchoredPosition;
    }

    public int dragstate = -1;

    public Dictionary<string,string> GetBase()
    {
        return new Dictionary<string, string>() {
            {"UUID", ""},
            {"Cons", ""},
            {"Pos", "(0, 0, 0)"},
            {"Scl", "(100, 100, 100)"},
            {"Name", ""},
            {"Desc", ""},
            {"Type", "Node"},
            {"Col", ""},
            {"ND", ""},
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
            {"Type", NodeType},
            {"Col", ColorUtility.ToHtmlStringRGB(Color)},
            {"ND", NodeData!=null?NodeData.DataToString():""},
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
        NodeType = e["Type"];
        Color = Converter.StringToColor(e["Col"]);
        Debug.Log("Try: " + e["Pos"]); 
        rt.anchoredPosition = Converter.StringToVector3(e["Pos"]);
        rt.sizeDelta = Converter.StringToVector3(e["Scl"]);
        Connections = Converter.StringToList(e["Cons"]);
        if (Connections.Contains("")) Connections.Remove("");

        if (e["ND"] == "")
        {
            NodeData = null;
        }
        else
        {
            switch (NodeType)
            {
                default:
                    Debug.LogWarning("node data detected on node with no handling");
                    break;
                case "Char":
                    NodeData = new CharacterData();
                    NodeData.StringToData(e["ND"]);
                    //((CharacterData)NodeData).personalities.Add(new PersonalityRef<string, int>("HELLO WORLD LOL", 69));
                    break;
            }
        }
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


    public Vector2 int_size;
    public Vector2 int_pos;
    public Vector2 intit_pos;
    public Vector2 intit_size;
    public bool int_was = false;
    public bool intit_was = false;
    public void Update()
    {
        bool ww = !Gamer.Instance.inmenu && Gamer.Instance.IsHovering(gameObject);
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
            OpenMyEditor();
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
            var eee = int_size;
            var pos = int_pos;
            int_was = true;
            int ym = 1;
            int xm = 1;
            switch (dragstate)
            {
                case 0:
                    eee.x += Viewport.Instance.MouseOffset.x;
                    eee.y -= Viewport.Instance.MouseOffset.y;
                    pos.x += Viewport.Instance.MouseOffset.x / 2;
                    pos.y += Viewport.Instance.MouseOffset.y / 2;
                    ym = -1;
                    break;
                case 1:
                    eee.x -= Viewport.Instance.MouseOffset.x;
                    eee.y -= Viewport.Instance.MouseOffset.y;
                    pos.x += Viewport.Instance.MouseOffset.x / 2;
                    pos.y += Viewport.Instance.MouseOffset.y / 2;
                    ym = -1;
                    xm = -1;
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
                    xm = -1;
                    break;
                case 4:
                    pos.x += Viewport.Instance.MouseOffset.x;
                    pos.y += Viewport.Instance.MouseOffset.y;
                    break;
            }

            if(!intit_was && Input.GetKey(KeyCode.LeftControl))
            {
                intit_size = int_size;
                intit_pos = int_pos;
                intit_was = true;
            }
            else if (intit_was && !Input.GetKey(KeyCode.LeftControl))
            {
                intit_was = false;
            }


            int_pos = pos;
            int_size = eee;
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (dragstate == 4)
                {

                    eee.x = Mathf.Round(eee.x / 50) * 50;
                    eee.y = Mathf.Round(eee.y / 50) * 50;

                    pos.x = Mathf.Floor(pos.x / 50) * 50;
                    pos.y = Mathf.Floor(pos.y / 50) * 50;
                    if ((eee.x % 100) != 0) pos.x += 25;
                    if ((eee.y % 100) != 0) pos.y += 25;
                }
                else
                {
                    Vector2 d = Vector2.zero;


                    d.x = Mathf.Round(intit_size.x / 50) * 50;
                    d.y = Mathf.Round(intit_size.y / 50) * 50;
                   // var fpos = pos -= d / 2;
                    eee.x = Mathf.Round(eee.x / 50) * 50;
                    eee.y = Mathf.Round(eee.y / 50) * 50;

                    pos.x = Mathf.Floor(intit_pos.x / 50) * 50;
                    pos.y = Mathf.Floor(intit_pos.y / 50) * 50;
                    pos.x += ((eee.x - d.x)/2)*xm;
                    pos.y += ((eee.y - d.y)/2)*ym;

                    //pos += d / 2;

                    //if ((intit_size.x % 100) != 0) pos.x += 25;
                    //if ((intit_size.y % 100) != 0) pos.y += 25;

                }


            }


            rt.sizeDelta = eee;
            rt.anchoredPosition = pos;
            UpdateConnectionLines();
        }
        else
        {
            if (int_was)
            {
                intit_was = false;
                int_was = false;
                int_size = rt.sizeDelta;
                int_pos = rt.anchoredPosition;
            }
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
    public void OpenMyEditor()
    {
        switch (NodeType)
        {
            default:
                Gamer.Instance.OpenEditorMenu(UUID);
                break;
            case "Char":
                Gamer.Instance.OpenCharacterEditorMenu(UUID);
                break;
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
    Image ddd;
    Image ddd1;
    Image ddd2;
    Image ddd3;
    public void UpdateDisplay()
    {
        titler.text = Name;
        selff.color = Color;
        refs[4].SetActive(NodeType != "Node");
        refs[5].SetActive(NodeType != "Node");
        refs[6].SetActive(NodeType != "Node");
        refs[7].SetActive(NodeType != "Node");
        if(NodeType != "Node")
        {
            if(ddd == null) ddd = refs[4].GetComponent<Image>();
            if(ddd1 == null) ddd1 = refs[5].GetComponent<Image>();
            if(ddd2 == null) ddd2 = refs[6].GetComponent<Image>();
            if(ddd3 == null) ddd3 = refs[7].GetComponent<Image>();
            ddd.color = Gamer.Instance.NodeTypeColorDict[NodeType];
            ddd1.color = Gamer.Instance.NodeTypeColorDict[NodeType];
            ddd2.color = Gamer.Instance.NodeTypeColorDict[NodeType];
            ddd3.color = Gamer.Instance.NodeTypeColorDict[NodeType];
        }
    }

}


public interface SpecialNodeData
{
    public string DataToString();
    public void StringToData(string data);
}
public class CharacterData : SpecialNodeData
{
    public List<PersonalityRef<string, int>> personalities = new List<PersonalityRef<string, int>>();
    public string DataToString()
    {
        Dictionary<string,string> mydata = new Dictionary<string,string>();

        var a = new List<string>();
        foreach(var b in personalities)
        {
            a.Add(Converter.EscapedListToString(new List<string>() { b.a, b.b.ToString() }, "!!!"));
        }

        mydata.Add("persons", Converter.EscapedListToString(a));

        return Converter.EscapedDictionaryToString(mydata);
    }
    public void StringToData(string data)
    {
        var mydata = Converter.EscapedStringToDictionary(data);
        var d = Converter.EscapedStringToList(mydata["persons"]);
        personalities.Clear();
        foreach (var b in d)
        {
            if (b == "") continue;
            var bb = Converter.EscapedStringToList(b, "!!!");
            personalities.Add(new PersonalityRef<string, int>(bb[0], int.Parse(bb[1])));
        }

    }
}

public class PersonalityRef<A,B>
{
    public A a;
    public B b;
    public PersonalityRef(A a, B b)
    {
        this.a = a;
        this.b = b;
    }
}
