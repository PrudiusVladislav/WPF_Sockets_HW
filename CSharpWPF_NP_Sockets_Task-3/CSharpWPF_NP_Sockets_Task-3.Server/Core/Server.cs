using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSharpWPF_NP_Sockets_Task_3.Server.Core;

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

    public string? ChatPartner {  get; set; }

    public Server(string hostName = DefaultHost, int port = DefaultPort)
    {
        //var host = Dns.GetHostEntry(hostName);
        //var ipAddress = host.AddressList[0];
        //endPoint = new IPEndPoint(ipAddress, port);
        endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
    }

    public void Start()
    {
        var starterSocket = CreateSocket();
        starterSocket.Bind(endPoint);
        starterSocket.Listen(10);
        server = starterSocket.Accept();
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

    public void Send(string message)
    {
        try
        {
            if (server is null)
                return;

            var sendBytes = Encoding.ASCII.GetBytes(message);
            server.Send(sendBytes);
            OnMessageSent(message);
        }
        catch(Exception ex)
        {
            OnErrorOccurred(ex.Message);
        }
       
    }

    public string Receive()
    {
        try
        {
            if (server is null)
                return "";

            var buffer = new byte[1024];
            var receivedBytes = server!.Receive(buffer);
            var receivedMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
            ChatPartner = server.RemoteEndPoint.ToString();
            OnMessageReceived(receivedMessage);
            return receivedMessage;
        }
        catch(Exception ex)
        {
            OnErrorOccurred(ex.Message);
            return "";
        }
    }

    public async Task<string> ReceiveAsync()
    {
        if (server is null)
            return "";

        var buffer = new byte[1024];
        var receivedBytes = await server!.ReceiveAsync(buffer);
        var receivedMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
        ChatPartner = server.RemoteEndPoint.ToString();
        OnMessageReceived(receivedMessage);
        return receivedMessage;
    }

    public void Dispose()
    {
        Finish();
        socket?.Dispose();
        server?.Dispose();
    }
}
