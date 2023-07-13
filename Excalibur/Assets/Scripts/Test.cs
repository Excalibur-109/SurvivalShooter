using Excalibur;
using Excalibur.Algorithms;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;

    public float t = 5f;
    public float ret;

    private void Start()
    {
        InputResponser input = new InputResponser();
        input.AttachInputKeys(new List<KeyCode>() { KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.Q});
        input.AttachKeyDownAction(KeyCode.A, InputKeyADown);
        input.AttachKeyDownAction(KeyCode.A, InputKeyBDown);
        input.AttachKeyDownAction(KeyCode.B, InputKeyBDown);
        input.AttachKeyDownAction(KeyCode.C, InputKeyCDown);
        input.AttachKeyDownAction(KeyCode.Q, AssetsManager.Instance.Print);
        input.Activate();
    }

    private void Update()
    {
        //text1.text = TimingSchedule.Instance.averageFrameRate.ToString();
    }

    public void TestTimer()
    {
        //TimingSchedule.Instance.Schedule(t, () =>
        //{
        //    text.text = "Complete";
        //}, state => text.text = (state / 1000f).ToString("f1"), 1f, 0, -1);
        //TimingSchedule.Instance.Schedule(t, null, state => text.text = (state / 1000f).ToString("f1"), 1f, 0, -1);
        //TimingSchedule.Instance.ScheduleAsync(5, () => text.text = "Complete",
        //    state => text.text = (state / 1000f).ToString("f1"), 1, 0, 1);
        TimingSchedule.Instance.Tick(t, state => text1.text = (t - state / 1000f).ToString("f1"), () => text2.text = "Complete");
        //TimingSchedule.Instance.TickAsync(t, state => text2.text = state.ToString());
    }

    void InputKeyADown()
    {
        AssetsManager.Instance.LoadAsset<GameObject>("Cube", (t) =>
        {
            GameObject go = Instantiate(t);
            go.transform.position = Vector3.zero;
            //AssetsManager.Instance.UnloadBundleMunual("Cube");
        });
    }

    void InputKeyBDown()
    {
        AssetsManager.Instance.LoadAsset<GameObject>("Sphere", (t) =>
        {
            GameObject go = Instantiate(t);
            go.transform.position = Vector3.up * 4f;
            //AssetsManager.Instance.UnloadBundleMunual("Sphere");
        });
    }

    void InputKeyCDown()
    {
        AssetsManager.Instance.LoadAsset<GameObject>("Cylinder", (t) =>
        {
            GameObject go = Instantiate(t);
            go.transform.position = Vector3.up * 6f;
            //AssetsManager.Instance.UnloadBundleMunual("Cylinder");
        });
    }
}
