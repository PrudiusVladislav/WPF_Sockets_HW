using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSharpWPF_NP_Sockets_Task_1.Client;

public class Client : IDisposable
{
    private const string DefaultHost = "localhost";
    private const int DefaultPort = 5000;

    private readonly EndPoint endPoint;
    private Socket? client;

    public event Action<string>? MessageReceived;
    public event Action<string>? MessageSent;

    public Client(string hostName = DefaultHost, int port = DefaultPort)
    {
        var host = Dns.GetHostEntry(hostName);
        var ipAddress = host.AddressList[0];
        endPoint = new IPEndPoint(ipAddress, port);
    }

    public void Connect()
    {
        //try
        //{
            client = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(endPoint);
        //}
        //catch (Exception ex)
        //{
        //    MessageBox.Show(ex.ToString());
        //}
    }

    private void OnMessageReceived(string message)
    {
        MessageReceived?.Invoke(message);
    }

    private void OnMessageSent(string message)
    {
        MessageSent?.Invoke(message);
    }

    public void Send(string message)
    {
        if (client is null || !client.Connected)
            Connect();

        var sendBytes = Encoding.ASCII.GetBytes(message);
        client!.Send(sendBytes);
        var sentMessageWithInfo = $"At {DateTime.UtcNow.ToString("HH:mm")} a message was sent: {message}";
        OnMessageSent(sentMessageWithInfo);
    }

    public string Receive()
    {
        if (client is null || !client.Connected)
            Connect();

        var buffer = new byte[1024];
        var receivedBytes = client!.Receive(buffer);
        var receivedMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
        var senderIpAddress = client.RemoteEndPoint;
        var receivedMessageWithInfo = $"At {DateTime.UtcNow.ToString("HH:mm")} a message was received from '{senderIpAddress}': {receivedMessage}";
        OnMessageReceived(receivedMessageWithInfo);
        return receivedMessage;
    }

    public string SendAndReceive(string message)
    {
        Send(message);
        return Receive();
    }

    public void Disconnect()
    {
        client!.Shutdown(SocketShutdown.Both);
        client.Close();
        client = null;
    }

    public void Dispose()
    {
        Disconnect();
        client?.Dispose();
    }
}
