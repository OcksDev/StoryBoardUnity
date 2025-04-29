using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewport : MonoBehaviour
{
    public RectTransform TheGamer;
    public RectTransform TheGamerScaler;
    public Vector3 PosTarget;
    // Update is called once per frame

    public static Viewport Instance;

    private void OnEnable()
    {
        Instance = this;
        PosTarget = Vector3.zero;
        TheGamer.transform.localPosition = PosTarget;
        TheGamerScaler.localScale = Vector3.one;
        scalem = 0.6823537f;
    }
    public float mult = 1;
    public float scrolmult = 1;
    public float scalem = 1;
    public float mousescale = 1;

    public Vector3 oldmouseshung = Vector3.zero;
    public Vector3 curpos = Vector3.zero;
    bool hasset = false;
    public Vector3 MouseOffset = Vector3.zero;


    float timer = 0;
    private void OnApplicationFocus(bool focus)
    {
        StartCoroutine(waitsex());
        timer = 0.01f;
    }
    public IEnumerator waitsex()
    {
        yield return null;
        var c = Camera.main.ScreenToWorldPoint(Input.mousePosition) * mousescale;
        c.z = 0;
        MouseOffset = Vector3.zero;
        oldmouseshung = c;
    }
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }
        Vector3 dir = Vector3.zero;
        if (InputManager.IsKey("move_forward", "Game")) dir -= Vector3.up;
        if (InputManager.IsKey("move_back", "Game")) dir -= Vector3.down;
        if (InputManager.IsKey("move_right", "Game")) dir -= Vector3.right;
        if (InputManager.IsKey("move_left", "Game")) dir -= Vector3.left;
        if (Input.GetAxis("Mouse ScrollWheel") != 0f && InputManager.CheckAvailability("Game")) // forward
        {
            scalem = Mathf.Clamp(scalem + (Input.GetAxis("Mouse ScrollWheel") * scrolmult * (scalem)), 0.01f, 5f);
        }

        var c = Camera.main.ScreenToWorldPoint(Input.mousePosition) * mousescale;
        c.z = 0;
        MouseOffset = (c - oldmouseshung) * (1 / scalem);

        oldmouseshung = c;
        if (InputManager.IsKey(KeyCode.Mouse1))
        {
            if (hasset)
            {
                PosTarget += MouseOffset;
                TheGamer.transform.localPosition += MouseOffset;
            }
            hasset = true;
        }
        else
        {
            hasset = false;
        }

        PosTarget += dir * Time.deltaTime * mult * Mathf.Lerp(1, 1 / scalem, 0.35f);
        TheGamer.transform.localPosition = Vector3.Lerp(TheGamer.transform.localPosition, PosTarget, 10 * Time.deltaTime);
        //TheGamerSM.transform.localPosition = c/scalem;
        TheGamerScaler.localScale = Vector3.one * scalem;
        curpos = TheGamer.transform.localPosition;
    }
}
