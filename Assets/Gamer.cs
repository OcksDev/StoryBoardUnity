using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;
public class Gamer : MonoBehaviour
{
    public List<GameObject> things;
    public Dictionary<string,NodeObject> myhomies = new Dictionary<string, NodeObject>();
    public static List<string> AvailableGraphs = new List<string>();
    public MouseState CurrentMouse = MouseState.None;
    public GameObject hovvovov;
    public RectTransform hovvovov2;
    public string CurrentBoard = "";
    public string connecting_uuid = "";
    public GameObject FakeLine;
    public float mult = 1;


    public void Start()
    {
        //StartCoroutine(gumgum());
        StartCoroutine(WaitToLoad());
        ree = true;
        UpdateUtilMenu();
        gitmenu = false;
        UpdateGitMenu();
    }
    public IEnumerator WaitToLoad()
    {
        yield return new WaitForSeconds(0.1f);
        LoadAll(CurrentBoard);
        Tags.refs["GitLoad"].gameObject.SetActive(false);
    }

    public bool captured_esc = false;

    bool CanHover = false;
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
    public void CreateNewNode(string type = "Node")
    {
        var ee = things[0].transform.position;
        //ee.z = 0;
        var zz = ee;

        var e = Instantiate(things[1], ee, Quaternion.identity, things[0].transform).GetComponent<NodeObject>();
        e.UUID = Tags.GenerateID();
        e.NodeType = type;
        e.rt.anchoredPosition = -Viewport.Instance.curpos;
        if (CurrentMouse == MouseState.Connecting)
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

        myhomies.Add(e.UUID, e);
        e.OpenMyEditor();
        if (CurrentMouse == MouseState.Connecting)
        {
            e.CompleteLineConnect();
        }
    }


    private void Update()
    {

        CanHover = true;
        if ((InputManager.IsKeyDown(KeyCode.Space, "Game") || (CurrentMouse==MouseState.Connecting && InputManager.IsKeyDown(KeyCode.Mouse2, "Game"))) && !inmenu)
        {
            CreateNewNode();
        }
        if(CurrentMouse == MouseState.Connecting && !inmenu)
        {
            var e = myhomies[connecting_uuid];
            var d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            d.z = e.transform.position.z;
            NodeObject.LineAllign(FakeLine, e.transform.position, d);
        }
        if(CurrentMouse == MouseState.DraggingWait)
        { 
            if(!Input.GetKey(KeyCode.Mouse0)) CurrentMouse = MouseState.None;
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

        if(hov_uuid != "")
        {
            var c = myhomies[hov_uuid];
            if(c.Desc != "")
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

        }
        else
        {
            hovvovov.SetActive(false);
        }

        hov_uuid = "";
    }

    public void SaveAll()
    {
        //return;
        List<string> fins = new List<string>();
        foreach(var a in myhomies)
        {
            fins.Add(a.Value.ItemToString());
        }
        SaveSystem.Instance.SetList("Objects", fins, CurrentBoard);
        SaveSystem.Instance.SaveDataToFile(CurrentBoard);
    }
    
    public void LoadAll(string dict)
    {
        foreach(var a in myhomies)
        {
            foreach(var b in a.Value.ConnectionLines)
            {
                if(b.Value!=null) Destroy(b.Value);
            }
            Destroy(a.Value.gameObject);
        }
        myhomies.Clear();
        Viewport.Instance.PosTarget = Vector3.zero;

        CurrentBoard = dict;
        FileSystem.Instance.AssembleFilePaths();
        var fp = SaveSystem.Instance.DictNameToFilePath(dict);

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
        if(CurrentMouse == MouseState.Connecting)
        {
            c.CompleteLineConnect();
        }
        Tags.refs["EditorMenu"].SetActive(false);
    }


    public void OpenCharacterEditorMenu(string uuid)
    {
        inmenu = true;
        InputManager.SetLockLevel("Editor");
        nerd_uuid = uuid;
        Tags.refs["EditorMenu_Character"].SetActive(true);
        /*var aa = Tags.refs["EditorMenu"].GetComponent<EditorMenu>();
        var c = myhomies[uuid];
        aa.Title.text = c.Name;
        aa.Description.text = c.Desc;
        aa.ColorHex.text = ColorUtility.ToHtmlStringRGB(c.Color);*/
    }


    public void CloseCharacterEditorMenu(string uuid)
    {
        captured_esc = true;
        inmenu = false;
        InputManager.ResetLockLevel();
        /*var aa = Tags.refs["EditorMenu"].GetComponent<EditorMenu>();
        var c = myhomies[uuid];
        c.Name = aa.Title.text;
        c.Desc = aa.Description.text;
        c.Color = Converter.StringToColor(aa.ColorHex.text);
        c.UpdateDisplay();*/
        Tags.refs["EditorMenu_Character"].SetActive(false);
    }





    bool ree = false;
    public void ToggleUtilMenu()
    {
        ree = !ree;
        UpdateUtilMenu();
    }
    public void UpdateUtilMenu()
    {
        Tags.refs["Util"].GetComponent<TextMeshProUGUI>().text = ree ?"<": ">";
        Tags.refs["Utils"].gameObject.SetActive(ree);
    }
    

    bool gitmenu = false;
    public void ToggleGitMenu()
    {
        gitmenu = !gitmenu;
        UpdateGitMenu();
    }
    public void UpdateGitMenu()
    {
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


    private void LateUpdate()
    {
        if(!captured_esc && Input.GetKeyDown(KeyCode.Escape))
        {
            if (gitmenu)
            {
                ToggleGitMenu();
            }
            else if(inmenu)
            {
                CloseSwitcher();
            }
            else
            {
                OpenSwitcher();
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (!inmenu)
            {
                ToggleUtilMenu();
            }
        }
        else if(ree && !inmenu)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ToggleGitMenu();
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                CreateNewNode("Char");
            }
        }
        else if (gitmenu && Input.GetKeyDown(KeyCode.Alpha1))
        {
            SexAndSync();
        }
        captured_esc = false;
    }

    public void OpenSwitcher()
    {
        inmenu = true;

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
