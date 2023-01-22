using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using System.Runtime.InteropServices;
using UnityEngine;

public class NativePluginTest : MonoBehaviour
{
    [DllImport("bootstrapper")]
    private static extern int open_server();

    [DllImport("bootstrapper")]
    private static extern void close_server(int pid);

    [DllImport("bootstrapper")]
    private static extern int get_tcp_port(int pid);

    public TextMeshProUGUI text;

    private IEnumerator Start()
    {
        var pid = open_server();

        var port = get_tcp_port(pid);

        Debug.Log(port);

        var tcpClient = new TcpClient();
        tcpClient.Connect(IPAddress.Loopback, port);
        var stream = tcpClient.GetStream();

        var encoding = new ASCIIEncoding();
        var command = encoding.GetBytes("\"go\"");

        var buffer = new byte[1];

        for (int _ = 0; _ < 10; _++)
        {
            stream.Write(command);
            stream.Read(buffer);
            var response = buffer[0];

            text.text = response.ToString();
            yield return new WaitForSeconds(1);

            text.text = "";
            yield return new WaitForSeconds(1);
        }

        text.text = "done";

        close_server(pid);
    }
}
