using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Gamer : MonoBehaviour
{
    public List<GameObject> things;
    public List<TwoRef<string, Color>> NodeTypeColors = new List<TwoRef<string, Color>>();
    public Dictionary<string, Color> NodeTypeColorDict = new Dictionary<string, Color>();
    public Dictionary<string, NodeObject> myhomies = new Dictionary<string, NodeObject>();
    public static List<string> AvailableGraphs = new List<string>();
    public MouseState CurrentMouse = MouseState.None;
    public GameObject hovvovov;
    public GameObject hovvovova;
    public RectTransform hovvovov2;
    public RectTransform hovvovova2;
    public string CurrentBoard = "";
    public string connecting_uuid = "";
    public GameObject FakeLine;
    public float mult = 1;
    public List<string> Personalities = new List<string>();



    public List<Color32> color32s = new List<Color32>();

    public void Start()
    {
        //StartCoroutine(gumgum());
        StartCoroutine(WaitToLoad());
        ree = true;
        UpdateUtilMenu();
        gitmenu = false;
        UpdateGitMenu();

        foreach (var a in NodeTypeColors)
        {
            NodeTypeColorDict.Add(a.a, a.b);
        }


    }
    public IEnumerator WaitToLoad()
    {
        yield return new WaitForSeconds(0.1f);
        LoadAll(CurrentBoard);
        Tags.refs["GitLoad"].gameObject.SetActive(false);
    }

    public bool captured_esc = false;

    private bool CanHover = false;
    public static Gamer Instance;
    private void Awake()
    {
        Instance = this;
    }

    private void OnApplicationQuit()
    {
        SaveAll();
    }

    public string hov_uuid = "";

    public bool inmenu = false;
    public NodeObject CreateNewNode(string type = "Node", bool flat = false)
    {
        var ee = things[0].transform.position;
        //ee.z = 0;
        var zz = ee;

        var e = Instantiate(things[1], ee, Quaternion.identity, things[0].transform).GetComponent<NodeObject>();
        e.UUID = Tags.GenerateID();
        e.NodeType = type;
        e.rt.anchoredPosition = -Viewport.Instance.curpos;
        if (!flat && CurrentMouse == MouseState.Connecting)
        {
            ee = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ee.z = zz.z;
            e.transform.position = ee;
            e.Color = myhomies[connecting_uuid].Color;
        }

        switch (type)
        {
            case "Char":
                e.NodeData = new CharacterData();
                break;
        }

        if (!flat)
        {
            myhomies.Add(e.UUID, e);
            e.OpenMyEditor();
            if (CurrentMouse == MouseState.Connecting)
            {
                e.CompleteLineConnect();
            }
        }
        return e;
    }


    private void Update()
    {

        CanHover = true;
        if ((InputManager.IsKeyDown(KeyCode.Space, "Game") || (CurrentMouse == MouseState.Connecting && InputManager.IsKeyDown(KeyCode.Mouse2, "Game"))) && !inmenu)
        {
            CreateNewNode();
        }
        if (CurrentMouse == MouseState.Connecting && !inmenu)
        {
            var e = myhomies[connecting_uuid];
            var d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            d.z = e.transform.position.z;
            NodeObject.LineAllign(FakeLine, e.transform.position, d);
        }
        if (CurrentMouse == MouseState.DraggingWait)
        {
            if (!Input.GetKey(KeyCode.Mouse0)) CurrentMouse = MouseState.None;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!inmenu && CurrentMouse == MouseState.Connecting)
            {
                captured_esc = true;
                CurrentMouse = MouseState.None;
                connecting_uuid = "";
                Destroy(FakeLine);
            }
        }
        if (inmenu) hov_uuid = "";

        if (hov_uuid != "")
        {
            var c = myhomies[hov_uuid];
            if (c.Desc != "")
            {
                hovvovov.SetActive(true);

                hovvovov.transform.position = c.transform.position;
                hovvovov2.GetComponent<TextMeshProUGUI>().text = c.Desc;
                hovvovov2.GetComponent<ContentSizeFitter>().SetLayoutVertical();
                var rt = hovvovov.GetComponent<RectTransform>();
                var fuck = rt.anchoredPosition;
                var zz = rt.sizeDelta;
                zz.y = hovvovov2.sizeDelta.y + (2 * 14f);
                rt.sizeDelta = zz;
                float ss = Mathf.Lerp(1, Viewport.Instance.scalem, 0.7f);
                fuck.x += (c.GetComponent<RectTransform>().sizeDelta.x / 2) * Viewport.Instance.scalem;
                fuck.x += (rt.sizeDelta.x / 2) * ss;
                fuck.x += (16) * ss;
                rt.anchoredPosition = fuck;
                hovvovov.transform.localScale = Vector3.one * ss;
            }
            else
            {
                hovvovov.SetActive(false);
            }

            if (c.NodeType == "Char")
            {
                hovvovova.SetActive(true);

                hovvovova.transform.position = c.transform.position;

                List<string> de = new List<string>();
                var d = c.GetMyData<CharacterData>();
                foreach (var a in d.personalities)
                {
                    de.Add($"<color=#{Converter.ColorToString(Gamer.Instance.color32s[a.b + 3])}>[{a.b}]</color> {a.a}");
                }
                if (de.Count < 1)
                {
                    hovvovova.SetActive(false);
                }
                else
                {
                    hovvovova2.GetComponent<TextMeshProUGUI>().text = Converter.ListToString(de, System.Environment.NewLine);
                    hovvovova2.GetComponent<ContentSizeFitter>().SetLayoutVertical();
                    var rt = hovvovova.GetComponent<RectTransform>();
                    var fuck = rt.anchoredPosition;
                    var zz = rt.sizeDelta;
                    zz.y = hovvovova2.sizeDelta.y + (2 * 14f);
                    rt.sizeDelta = zz;
                    float ss = Mathf.Lerp(1, Viewport.Instance.scalem, 0.7f);
                    fuck.x -= (c.GetComponent<RectTransform>().sizeDelta.x / 2) * Viewport.Instance.scalem;
                    fuck.x -= (rt.sizeDelta.x / 2) * ss;
                    fuck.x -= (16) * ss;
                    rt.anchoredPosition = fuck;
                    hovvovova.transform.localScale = Vector3.one * ss;
                }


            }
            else
            {
                hovvovova.SetActive(false);
            }

        }
        else
        {
            hovvovov.SetActive(false);
            hovvovova.SetActive(false);
        }

        hov_uuid = "";
    }

    public void SaveAll()
    {
        //return;
        List<string> fins = new List<string>();
        foreach (var a in myhomies)
        {
            fins.Add(a.Value.ItemToString());
        }
        SaveSystem.Instance.SetList("Objects", fins, CurrentBoard);
        SaveSystem.Instance.SaveDataToFile(CurrentBoard);

        FileSystem.Instance.WriteFile(FileSystem.Instance.FileLocations["Charpers"],
            Converter.ListToString(Personalities), true);
    }

    public void LoadAll(string dict)
    {
        foreach (var a in myhomies)
        {
            foreach (var b in a.Value.ConnectionLines)
            {
                if (b.Value != null) Destroy(b.Value);
            }
            Destroy(a.Value.gameObject);
        }
        myhomies.Clear();
        Viewport.Instance.PosTarget = Vector3.zero;

        CurrentBoard = dict;
        FileSystem.Instance.AssembleFilePaths();
        var fp = SaveSystem.Instance.DictNameToFilePath(dict);

        Personalities = Converter.StringToList(FileSystem.Instance.ReadFile(FileSystem.Instance.FileLocations["Charpers"]));
        for (int i = 0; i < Personalities.Count; i++)
        {
            Personalities[i] = killme(Personalities[i]);
        }
        Personalities.Sort();


        var text = File.ReadAllText(fp);
        text = text.Substring("Objects: ".Length);
        var e = Converter.EscapedStringToList(text);
        var ee = things[0].transform.position;
        foreach (var a in e)
        {
            if (a == "" || a == " ") continue;
            Debug.Log("Found thiung");
            var d = Instantiate(things[1], ee, Quaternion.identity, things[0].transform).GetComponent<NodeObject>();
            d.StringToItem(a);
            myhomies.Add(d.UUID, d);
        }

        foreach (var a in myhomies)
        {
            a.Value.UpdateConnectionLines();
        }
        //Viewport.Instance.scalem = 1;
        CurrentMouse = MouseState.None;
        connecting_uuid = "";
        inmenu = false;
        hov_uuid = "";

    }

    public IEnumerator gamin()
    {
        yield return new WaitForFixedUpdate();
        Viewport.Instance.scalem = 1;
    }


    public enum MouseState
    {
        None,
        Connecting,
        Dragging,
        DraggingWait,
        Multiselecting,
    }
    public string nerd_uuid = "";
    public void OpenEditorMenu(string uuid)
    {
        inmenu = true;
        InputManager.SetLockLevel("Editor");
        nerd_uuid = uuid;
        Tags.refs["EditorMenu"].SetActive(true);
        var aa = Tags.refs["EditorMenu"].GetComponent<EditorMenu>();
        var c = myhomies[uuid];
        aa.Title.text = c.Name;
        aa.Description.text = c.Desc;
        aa.ColorHex.text = ColorUtility.ToHtmlStringRGB(c.Color);
    }
    public void CloseEditorMenu(string uuid)
    {
        captured_esc = true;
        inmenu = false;
        InputManager.ResetLockLevel();
        var aa = Tags.refs["EditorMenu"].GetComponent<EditorMenu>();
        var c = myhomies[uuid];
        c.Name = aa.Title.text;
        c.Desc = aa.Description.text;
        c.Color = Converter.StringToColor(aa.ColorHex.text);
        c.UpdateDisplay();
        if (CurrentMouse == MouseState.Connecting)
        {
            c.CompleteLineConnect();
        }
        Tags.refs["EditorMenu"].SetActive(false);
    }
    public bool REWE = false;
    public void OpenCharacterEditorMenu(string uuid)
    {
        REWE = true;
        inmenu = true;
        Tags.refs["sugg_input"].GetComponent<TMP_InputField>().text = "";
        InputManager.SetLockLevel("Editor");
        nerd_uuid = uuid;
        Tags.refs["EditorMenu_Character"].SetActive(true);
        var aa = Tags.refs["EditorMenu_Character"].GetComponent<CharacterEditorMenu>();
        var c = myhomies[uuid];
        aa.Title.text = c.Name;
        aa.Description.text = c.Desc;
        aa.ColorHex.text = ColorUtility.ToHtmlStringRGB(c.Color);

        ReloadPersonalityList(c);
    }

    public void ReloadPersonalityList(NodeObject c)
    {
        var a = Tags.refs["Personaliti"].GetComponentsInChildren<PersonalityOb>();
        foreach (var b in a)
        {
            Destroy(b.gameObject);
        }
        var d = c.GetMyData<CharacterData>();
        //d.personalities.Add(new PersonalityRef<string, int>("Test", 1));
        foreach (var b in d.personalities)
        {
            var e = Instantiate(things[4], Tags.refs["Personaliti"].transform).GetComponent<PersonalityOb>();
            e.characterData = d;
            e.SetFromData(b);
        }
        UpdatePerSuggList(c);
    }
    public List<string> taken_suggs = new List<string>();
    public List<perssugg> spawned_suggs = new List<perssugg>();
    public NodeObject suggin = null;
    public void UpdatePerSuggList(NodeObject c)
    {
        suggin = c;
        var d = c.GetMyData<CharacterData>();
        taken_suggs.Clear();
        foreach (var b in d.personalities)
        {
            taken_suggs.Add(b.a);
        }
        RefreshSuggList();
    }

    public void RefreshSuggList()
    {
        var d = suggin.GetMyData<CharacterData>();
        foreach (var b in spawned_suggs)
        {
            Destroy(b.gameObject);
        }
        spawned_suggs.Clear();
        foreach (var b in Personalities)
        {
            if (taken_suggs.Contains(b)) continue;
            var e = Instantiate(things[5], Tags.refs["suggholder"].transform).GetComponent<perssugg>();
            spawned_suggs.Add(e);
            e.SetData(b);
            e.AmCool("");
        }
    }
    public void TextUpdateonSugglist()
    {
        var a = Tags.refs["sugg_input"].GetComponent<TMP_InputField>();
        foreach (var b in spawned_suggs)
        {
            b.AmCool(a.text);
        }
        a.text = killme(a.text);
    }
    public void Autocompletw()
    {
        var a = Tags.refs["sugg_input"].GetComponent<TMP_InputField>();
        string aa = a.text;
        foreach (var b in spawned_suggs)
        {
            b.AmCool(a.text);
            if (b.IsAv)
            {
                aa = b.mydata;
                break;
            }
        }
        a.text = killme(aa);
    }

    public static string killme(string a)
    {
        if (a.Length < 2) return a.ToUpper();
        return a.ToUpper()[0].ToString() + a.Substring(1).ToLower();
    }

    public void SubmitNewPers()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) return;
        if (Input.GetKeyDown(KeyCode.Mouse2)) return;
        var dd = Tags.refs["sugg_input"].GetComponent<TMP_InputField>();
        var ddd = dd.text;
        if (ddd == "") return;
        dd.text = "";
        var d = suggin.GetMyData<CharacterData>();
        d.personalities.Add(new PersonalityRef<string, int>(ddd, 0));
        ReloadPersonalityList(suggin);
        Tags.refs["FixSelect"].GetComponent<Button>().Select();
        Tags.refs["sugg_input"].GetComponent<TMP_InputField>().Select();
    }


    public void CloseCharacterEditorMenu(string uuid)
    {
        REWE = false;
        captured_esc = true;
        inmenu = false;
        InputManager.ResetLockLevel();
        var aa = Tags.refs["EditorMenu_Character"].GetComponent<CharacterEditorMenu>();
        var c = myhomies[uuid];
        c.Name = aa.Title.text;
        c.Desc = aa.Description.text;
        c.Color = Converter.StringToColor(aa.ColorHex.text);
        c.UpdateDisplay();
        Tags.refs["EditorMenu_Character"].SetActive(false);
    }





    private bool ree = false;
    public void ToggleUtilMenu()
    {
        ree = !ree;
        UpdateUtilMenu();
    }
    public void UpdateUtilMenu()
    {
        Tags.refs["Util"].GetComponent<TextMeshProUGUI>().text = ree ? "<" : ">";
        Tags.refs["Utils"].gameObject.SetActive(ree);
    }


    private bool gitmenu = false;
    public void ToggleGitMenu()
    {
        gitmenu = !gitmenu;
        UpdateGitMenu();
    }
    public void UpdateGitMenu()
    {
        ExitMultiSelect();
        inmenu = gitmenu;
        Tags.refs["GitMenu"].gameObject.SetActive(gitmenu);
        Tags.refs["GitSure"].gameObject.SetActive(false);
    }





    [HideInInspector]
    public List<RaycastResult> rcl = new List<RaycastResult>();
    public void HoverDataCooler()
    {
        if (!CanHover) return;
        CanHover = false;
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = Input.mousePosition;
        rcl.Clear();
        EventSystem.current.RaycastAll(ped, rcl);
    }
    public bool IsHovering(GameObject sussy)
    {
        HoverDataCooler();

        foreach (var ray in rcl)
        {
            if (ray.gameObject == sussy)
            {
                return true;
            }
        }
        return false;
    }


    public IEnumerator gumgum()
    {
        yield return new WaitForSeconds(0.1f);

        Thread e = new Thread(() =>
        {
            var rr = Git.Command("C:\\Users\\milom\\AppData\\Roaming\\Ocks\\Storyboard", "pull");

            if (rr.Contains("would be overwritten"))
            {
                Debug.LogError("Merge conflict!"); //bring up thing to let me keep local or use remote
                SaveSystem.Instance.LoadGame();
            }
            else
            {
                var a = CommitAndSync();
                if (a.Contains("Everything up"))
                {
                    Debug.Log("No Push (up to date)");
                }
                else
                {
                    Debug.Log("Pushed (I hope)");
                }

                SaveSystem.Instance.LoadGame();
            }
        });

        e.Start();
        //
        /*yield return new WaitForSeconds(0.2f);
        Debug.Log("3" + Git.Command("C:\\Users\\milom\\AppData\\Roaming\\Ocks\\Storyboard", "stash pop"));
        yield return new WaitForSeconds(0.2f);
        Debug.Log("4" + Git.Command("C:\\Users\\milom\\AppData\\Roaming\\Ocks\\Storyboard", "status"));*/
    }

    public bool captured_left = false;

    private void LateUpdate()
    {
        if (!captured_esc && Input.GetKeyDown(KeyCode.Escape))
        {
            if (has_dragged)
            {
                ExitMultiSelect();
            }
            else if (gitmenu)
            {
                ToggleGitMenu();
            }
            else if (inmenu)
            {
                CloseSwitcher();
            }
            else
            {
                OpenSwitcher();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!inmenu)
            {
                ToggleUtilMenu();
            }
            else if (REWE)
            {
                Autocompletw();
            }
        }
        else if (ree && !inmenu)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ToggleGitMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CreateNewNode("Char");
            }
        }
        else if (gitmenu && Input.GetKeyDown(KeyCode.Alpha1))
        {
            SexAndSync();
        }

        if (!captured_left && !inmenu && InputManager.IsKey(KeyCode.Mouse0, "Game") && CurrentMouse != MouseState.Multiselecting)
        {
            StartDragSelect();
        }
        else if (CurrentMouse == MouseState.Multiselecting && !InputManager.IsKey(KeyCode.Mouse0, "Game"))
        {
            EndDragSelect();
        }

        if (CurrentMouse == MouseState.Multiselecting)
        {
            var c = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            c.z = 0;
            drag_endp = c;
            var x = Vector3.Lerp(Tags.refs["SelectionStart"].transform.position, drag_endp, 0.5f);
            Vector2 scale = new Vector2(Mathf.Abs(drag_endp.x - Tags.refs["SelectionStart"].transform.position.x), Mathf.Abs(drag_endp.y - Tags.refs["SelectionStart"].transform.position.y));
            x.z = Tags.refs["SmallParent"].transform.position.z;
            Tags.refs["Selection"].transform.position = x;
            drag_rect.sizeDelta = scale * 80 / Viewport.Instance.scalem;
        }


        captured_esc = false;
        captured_left = false;
    }
    public List<string> dragging_ids = new List<string>();
    public Vector3 drag_startp = Vector3.zero;
    public Vector3 drag_endp = Vector3.zero;
    public RectTransform drag_rect;
    public bool has_dragged = false;
    public void StartDragSelect()
    {
        ExitMultiSelect();
        dragging_ids.Clear();
        CurrentMouse = MouseState.Multiselecting;
        drag_rect = Tags.refs["Selection"].GetComponent<RectTransform>();
        var c = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        c.z = 0;
        //c = Vector3.Lerp(c - Viewport.Instance.curposworld, Viewport.Instance.curposworld + c, 0.5f);
        Tags.refs["Selection"].transform.position = c;
        Tags.refs["SelectionStart"].transform.position = c;
        drag_endp = c;
        Tags.refs["Selection"].SetActive(true);
        Debug.Log("Started Drag!");
        has_dragged = true;
    }
    public void ExitMultiSelect()
    {
        has_dragged = false;
        Tags.refs["SelectionBG"].SetActive(false);
    }
    public void EndDragSelect()
    {
        CurrentMouse = MouseState.None;
        Tags.refs["Selection"].SetActive(false);
        Debug.Log("Ended Drag!");
        Tags.refs["SelectionBG"].SetActive(true);
        UpdateSelectionAreaToMatchSelection();
        if (dragging_ids.Count <= 1) ExitMultiSelect();
    }
    public void UpdateSelectionAreaToMatchSelection()
    {
        float minx = 0;
        float miny = 0;
        float maxx = 0;
        float maxy = 0;
        bool first = true;
        foreach (var a in dragging_ids)
        {
            var b = myhomies[a];
            if (first || b.rt.anchoredPosition.x - b.rt.sizeDelta.x / 2 < minx) minx = b.rt.anchoredPosition.x - b.rt.sizeDelta.x / 2;
            if (first || b.rt.anchoredPosition.x + b.rt.sizeDelta.x / 2 > maxx) maxx = b.rt.anchoredPosition.x + b.rt.sizeDelta.x / 2;
            if (first || b.rt.anchoredPosition.y - b.rt.sizeDelta.y / 2 < miny) miny = b.rt.anchoredPosition.y - b.rt.sizeDelta.y / 2;
            if (first || b.rt.anchoredPosition.y + b.rt.sizeDelta.y / 2 > maxy) maxy = b.rt.anchoredPosition.y + b.rt.sizeDelta.y / 2;
            first = false;
        }
        var x = Vector3.Lerp(new Vector3(minx, miny, Tags.refs["SmallParent"].transform.position.z), new Vector3(maxx, maxy, Tags.refs["SmallParent"].transform.position.z), 0.5f);
        Tags.refs["SelectionBG"].GetComponent<RectTransform>().anchoredPosition = x;
        Tags.refs["SelectionBG"].GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(maxx - minx), Mathf.Abs(maxy - miny));
    }

    public void GenericMenuOpen()
    {
        ExitMultiSelect();
        inmenu = true;
    }


    public void OpenSwitcher()
    {
        GenericMenuOpen();
        ree = false;
        UpdateUtilMenu();

        InputManager.SetLockLevel("Editor");
        Tags.refs["Switcher"].SetActive(true);
        Tags.refs["Switcher"].GetComponent<SwitcherMover>().selection = AvailableGraphs.IndexOf(CurrentBoard);
        Tags.refs["Switcher"].GetComponent<SwitcherMover>().UpdateDisplay();
    }
    public void CloseSwitcher()
    {
        inmenu = false;
        InputManager.ResetLockLevel();
        Tags.refs["Switcher"].SetActive(false);
        ree = true;
        UpdateUtilMenu();
    }



    public void SexAndSync()
    {
        Tags.refs["GitLoad"].gameObject.SetActive(true);
        StartCoroutine(a());
    }
    public IEnumerator a()
    {
        yield return null;
        SaveAll();
        var rc = CommitAndSync();
        Tags.refs["GitLast"].GetComponent<TextMeshProUGUI>().text = "Git Last: \n" + rc;
        Tags.refs["GitLoad"].gameObject.SetActive(false);
    }

    public void FuckAndShit()
    {
        Tags.refs["GitLoad"].gameObject.SetActive(true);
        StartCoroutine(b());

    }
    public IEnumerator b()
    {
        yield return null;
        SaveAll();
        SaveSystem.Instance.SaveGame();
        Tags.refs["GitSure"].gameObject.SetActive(false);
        var rc = PanicButton();
        Tags.refs["GitLast"].GetComponent<TextMeshProUGUI>().text = "Git Last: \n" + rc;
        StartCoroutine(WaitToLoad());
    }
    public void YayAndPull()
    {
        Tags.refs["GitLoad"].gameObject.SetActive(true);
        StartCoroutine(c());

    }
    public IEnumerator c()
    {
        yield return null;
        var rc = Pull();
        Tags.refs["GitLast"].GetComponent<TextMeshProUGUI>().text = "Git Last: \n" + rc;
        StartCoroutine(WaitToLoad());
    }

    public string CommitAndSync()
    {
        Git.Command("C:\\Users\\milom\\AppData\\Roaming\\Ocks\\Storyboard", "add .");
        Git.Command("C:\\Users\\milom\\AppData\\Roaming\\Ocks\\Storyboard", $"commit -m \"{System.DateTime.Now.ToString("MM-dd-yy HH:mm:ss")}\"");
        return Git.Command("C:\\Users\\milom\\AppData\\Roaming\\Ocks\\Storyboard", $"push");
    }
    public string PanicButton()
    {
        Git.Command("C:\\Users\\milom\\AppData\\Roaming\\Ocks\\Storyboard", "restore .");
        return Git.Command("C:\\Users\\milom\\AppData\\Roaming\\Ocks\\Storyboard", $"pull");
    }
    public string Pull()
    {
        return Git.Command("C:\\Users\\milom\\AppData\\Roaming\\Ocks\\Storyboard", $"pull");
    }
    public void RevealSure()
    {
        Tags.refs["GitSure"].gameObject.SetActive(true);
    }

}

[System.Serializable]
public class TwoRef<A, B>
{
    public A a; public B b;
}