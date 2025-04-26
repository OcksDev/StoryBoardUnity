using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEngine.EventSystems;
public class Gamer : MonoBehaviour
{
    public List<GameObject> things;
    public Dictionary<string,NodeObject> myhomies = new Dictionary<string, NodeObject>();
    public MouseState CurrentMouse = MouseState.None;

    public string CurrentBoard = "";
    public string connecting_uuid = "";
    public float mult = 1;
    public void Start()
    {
        //StartCoroutine(gumgum());
        StartCoroutine(WaitToLoad());
    }
    public IEnumerator WaitToLoad()
    {
        yield return new WaitForSeconds(0.1f);
        LoadAll("RCD");
        foreach (var a in myhomies)
        {
            a.Value.UpdateConnectionLines();
        }
    }



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

    private void Update()
    {
        CanHover = true;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var ee = things[0].transform.position;
            //ee.z = 0;
            var e = Instantiate(things[1], ee, Quaternion.identity, things[0].transform).GetComponent<NodeObject>();
            e.UUID = Tags.GenerateID();
            e.rt.anchoredPosition = - Viewport.Instance.curpos;
            myhomies.Add(e.UUID, e);
        }
    }

    public void SaveAll()
    {
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
        CurrentBoard = dict;
        SaveSystem.Instance.GetDataFromFile(CurrentBoard);
        var e = SaveSystem.Instance.GetList("Objects", new List<string>(), CurrentBoard);
        var ee = things[0].transform.position;
        foreach (var a in e)
        {
            if (a == "" || a == " ") continue;
            var d = Instantiate(things[1], ee, Quaternion.identity, things[0].transform).GetComponent<NodeObject>();
            d.StringToItem(a);
            myhomies.Add(d.UUID, d);
        }
    }
    public enum MouseState
    {
        None,
        Connecting,
        Dragging,
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

    public string CommitAndSync()
    {
        Git.Command("C:\\Users\\milom\\AppData\\Roaming\\Ocks\\Storyboard", "add .");
        Git.Command("C:\\Users\\milom\\AppData\\Roaming\\Ocks\\Storyboard", $"commit -m \"{System.DateTime.Now.ToString("MM-dd-yy HH:mm:ss")}\"");
        return Git.Command("C:\\Users\\milom\\AppData\\Roaming\\Ocks\\Storyboard", $"push");
    }

}
