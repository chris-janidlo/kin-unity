using System.Collections;
using Native;
using TMPro;
using UnityEngine;

public class NativePluginTest : MonoBehaviour
{
    public TextMeshProUGUI text;

    private IEnumerator Start()
    {
        var i = 0;
        while (true)
        {
            text.text = Plugin.Instance.Invoke<mul_by_5, int>(i++)
                .ToString();
            yield return new WaitForSeconds(1);
        }
        // ReSharper disable once IteratorNeverReturns
    }

    // ReSharper disable once InconsistentNaming
    private delegate int mul_by_5(int i);
}