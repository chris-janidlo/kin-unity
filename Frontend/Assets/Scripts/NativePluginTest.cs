using System.Collections;
using kInvoke;
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
            text.text = LibraryFunction<mul_by_5>.Invoke(i++)
                .ToString();
            yield return new WaitForSeconds(1);
        }
        // ReSharper disable once IteratorNeverReturns
    }

    // ReSharper disable once InconsistentNaming
    [LibraryImport("KinAI")]
    private delegate int mul_by_5(int i);
}