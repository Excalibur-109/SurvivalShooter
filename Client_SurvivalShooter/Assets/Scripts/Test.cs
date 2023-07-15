using Excalibur;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Excalibur.Algorithms.DataStructure;
using System.Text;

/// test gitlens

public class Test : MonoBehaviour
{
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;

    public float t = 5f;
    public float ret;

    private void Start()
    {
        TestBinaryTree();

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

    void TestBinaryTree()
    {
        BinaryTree<int> tree = new BinaryTree<int>();
        tree.Add(0);
        tree.Add(1);
        tree.Add(2);
        tree.Add(3);
        tree.Add(4);
        tree.Remove(3);
        BinaryTreeNode<int> n = tree.Find(4);
        if (n == null) { Debug.Log("没找到4."); }
        else { Debug.Log("找到了4."); }
        n = tree.Find(3);
        if (n == null) { Debug.Log("没找到3."); }
        else { Debug.Log("找到了3."); }
        StringBuilder sb = new StringBuilder();
        tree.BreadthFirstTraversal(value => sb.Append("-" + value));
        Debug.Log(sb.ToString());
        sb.Clear();
        tree.PreOrderTraversal(value => sb.Append("-" + value));
        Debug.Log(sb.ToString());
        sb.Clear();
        tree.InOrderTraversal(value => sb.Append("-" + value));
        Debug.Log(sb.ToString());
        sb.Clear();
        tree.PostOrderTraversal(value => sb.Append("-" + value));
        Debug.Log(sb.ToString());
        sb.Clear();
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
        Timing.Instance.Tick(t, state => text1.text = (t - state / 1000f).ToString("f1"), () => text2.text = "Complete");
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
