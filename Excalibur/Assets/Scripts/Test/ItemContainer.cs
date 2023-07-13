using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using UnityEditor;
using UnityEngine.UI;

public class TestData : BaseData
{
    public int value;

    public TestData (int value) => this.value = value;
}

public class ItemContainer : MonoBehaviour
{
    public GameObject leftPart;
    public Text text;
    public Button positive;
    public Button negative;
    public Button deleteSelect;

    public InputField input;
    public Button show;
    public Button addLast;
    public Button multiSelect;
    public Button clear;

    public Button scrollTo;
    public Button toTop;
    public Button toBottom;
    public Text scrollToText;
    Vector2 scrollToPos;
    int countScroll = 0;

    public Vector2 pos;

    int count = 100;

    ExcaliburList list;
    List<TestData> data = new List<TestData> ();

    private void Awake ()
    {
        list = GetComponent<ExcaliburList> ();
        leftPart.SetActive (false);
        text.text = "";
        list.onSelected = (data) =>
        {
            if (data == null) { text.text = ""; leftPart.SetActive (false); }
            else { text.text = (data as TestData).value.ToString (); leftPart.SetActive (true); }
        };
        multiSelect.GetComponentInChildren<Text> ().text = list.allowMultiSelect ? "可多选" : "仅单选";
    }

    private void Start ()
    {
        input.onEndEdit.AddListener ( (value) => int.TryParse (value, out count));
        positive.onClick.AddListener ( () =>
        {
            List<TestData> temp = list.GetSelections<TestData> ();
            for (int i = 0; i < temp.Count; ++i)
            {
                temp[i].value += 1;
            }
            list.OnRefresh (temp);
        });
        negative.onClick.AddListener ( () =>
        {
            List<TestData> temp = list.GetSelections<TestData> ();
            List<TestData> toremove = new List<TestData> ();
            for (int i = 0; i < temp.Count; ++i)
            {
                temp[i].value -= 1;
                if (temp[i].value == 0)
                {
                    toremove.Add (temp[i]);
                    data.Remove (temp[i]);
                }
            }
            list.OnRefresh (temp);
            list.OnRemove (toremove);
        });
        deleteSelect.onClick.AddListener ( () =>
        {
            List<TestData> temp = list.GetSelections<TestData> ();
            for (int i = 0; i < temp.Count; ++i)
            {
                data.Remove (temp[i]);
            }
            list.OnRemove (temp);
        });
        show.onClick.AddListener ( () =>
        {
            data.Clear ();
            for (int i = 0; i < count; ++i)
            {
                data.Add (new TestData (i + 1));
            }
            list.SurveilDatas (data);
        });
        addLast.onClick.AddListener ( () =>
        {
            if (data.Count == 0)
            {
                data.Add (new TestData (1));
            }
            else
            {
                data.Add (new TestData (data[data.Count - 1].value + 1));
            }
            list.SurveilDatas (data, false);
        });
        multiSelect.onClick.AddListener ( () =>
        {
            list.allowMultiSelect = !list.allowMultiSelect;
            multiSelect.GetComponentInChildren<Text> ().text = list.allowMultiSelect ? "可多选" : "仅单选";
        });
        clear.onClick.AddListener ( () =>
        {
            data.Clear ();
            list.SurveilDatas (data);
        });
        scrollTo.onClick.AddListener ( () =>
        {
            ++countScroll;
            if (countScroll == 1)
            {
                scrollToPos.y = Random.Range (0f, list.maxScrollRange.y);
                scrollToText.text = scrollToPos.y.ToString ();
            }
            else if (countScroll == 2)
            {
                countScroll = 0;
                list.ScrollTo (scrollToPos, true);
            }
        });
        toTop.onClick.AddListener ( () =>
        {
            list.ScrollTo (0f, 1, true);
        });
        toBottom.onClick.AddListener ( () =>
        {
            Debug.LogError (list.maxScrollRange);
            list.ScrollTo (list.maxScrollRange.y, 1, true);
        });
    }

    private void OnGUI ()
    {
        GUILayout.BeginVertical ();
        if (GUILayout.Button ("DDDD", GUILayout.Width (120), GUILayout.Height (80)))
        {
            data.RemoveAt (0);
            list.SurveilDatas (data, false);
        }
        GUILayout.EndVertical ();
    }
}