using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using kInvoke;
using TMPro;
using UnityEngine;

public class NativePluginTest : MonoBehaviour
{
    public TextMeshProUGUI text;

    private IEnumerator Start()
    {
        var port = LibraryFunction<launch_server>.Invoke();

        Debug.Log(port);

        var tcpClient = new TcpClient();
        tcpClient.Connect(IPAddress.Loopback, port);
        var stream = tcpClient.GetStream();

        var encoding = new ASCIIEncoding();
        var command = encoding.GetBytes("\"go\"");

        var buffer = new byte[1];

        while (true)
        {
            stream.Write(command);
            stream.Read(buffer);
            var response = buffer[0];

            text.text = response.ToString();
            yield return new WaitForSeconds(1);

            text.text = "";
            yield return new WaitForSeconds(1);
        }
    }

    [LibraryImport("bootstrapper")]
    private delegate int launch_server();
}
