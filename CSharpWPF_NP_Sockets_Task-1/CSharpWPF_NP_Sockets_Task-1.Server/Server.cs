using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSharpWPF_NP_Sockets_Task_1.Server;

public class Server : IDisposable
{
    private const string DefaultHost = "localhost";
    private const int DefaultPort = 5000;

    private readonly EndPoint endPoint;
    private Socket? socket;
    private Socket? server;

    public event Action<string>? MessageReceived;
    public event Action<string>? MessageSent;
    public event Action<string>? ErrorOccurred;
    
    public Server(string hostName = DefaultHost, int port = DefaultPort)
    {
        var host = Dns.GetHostEntry(hostName);
        var ipAddress = host.AddressList[0];
        endPoint = new IPEndPoint(ipAddress, port);
    }

    public void Start()
    {
        var starterSocket = CreateSocket();
        starterSocket.Bind(endPoint);
        starterSocket.Listen(10);
        server = starterSocket.Accept();
        Handle();
    }

    public void Finish()
    {
        server?.Close();
        server = null;
    }

    private void OnMessageReceived(string message)
    {
        MessageReceived?.Invoke(message);
    }

    private void OnMessageSent(string message)
    {
        MessageSent?.Invoke(message);
    }

    private void OnErrorOccurred(string exceptionMessage)
    {
        ErrorOccurred?.Invoke(exceptionMessage);
    }

    private Socket CreateSocket()
    {
        return socket ??= new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    }

    private void Handle()
    {
        if (server is null)
            return;
        try
        {
            var buffer = new byte[1024];
            var receivedBytes = server.Receive(buffer);
            var receivedMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
            var senderIpAddress = server.RemoteEndPoint;
            var receivedMessageWithInfo = $"At {DateTime.UtcNow.ToString("HH:mm")} a message was received from '{senderIpAddress}': {receivedMessage}";
            OnMessageReceived(receivedMessageWithInfo);

            var messageBack = "Hello, client!";
            var sendBytes = Encoding.ASCII.GetBytes(messageBack);
            server.Send(sendBytes);
            var sentMessageWithInfo = $"At {DateTime.UtcNow.ToString("HH:mm")} a message was sent: {messageBack}";
            OnMessageSent(sentMessageWithInfo);
        }
        catch (Exception ex)
        {
            OnErrorOccurred(ex.Message);
        }
    }

    public void Dispose()
    {
        Finish();
        socket?.Dispose();
        server?.Dispose();
    }
}
