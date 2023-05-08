using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;

namespace Code.Player.AIComms
{
    public class NativePluginTest : MonoBehaviour
    {
        public TextMeshProUGUI text;

        private void Start()
        {
            using (var server = new AiServer())
            {
                var tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, server.Port);
                NetworkStream stream = tcpClient.GetStream();

                var encoding = new ASCIIEncoding();
                byte[] command = encoding.GetBytes("\"go\"");

                var buffer = new byte[1];

                stream.Write(command);
                stream.Read(buffer);
                byte response = buffer[0];

                text.text = response.ToString();

                server.IntentionallyError();
            }
        }
    }
}