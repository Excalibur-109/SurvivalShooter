using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using TMPro;
using Newtonsoft.Json.Linq;

public class TestBaseItem : BaseListItem
{
    TextMeshProUGUI text;

    protected override void Awake ()
    {
        base.Awake ();

        text = GetComponentInChildren<TextMeshProUGUI> ();
    }

    protected override void RefreshUI ()
    {
        text.text = (data as TestData).value.ToString ();
    }
}
