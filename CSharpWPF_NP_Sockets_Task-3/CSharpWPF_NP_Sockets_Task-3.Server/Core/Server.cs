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
    public string? MessageToSend {  get; set; }
    private string[] responsesArray;
    public Server(string hostName = DefaultHost, int port = DefaultPort)
    {
        var host = Dns.GetHostEntry(hostName);
        var ipAddress = host.AddressList[0];
        endPoint = new IPEndPoint(ipAddress, port);
        responsesArray = new string[]
        {
            "Hello, how can I assist you?",
            "Thank you for your message.",
            "I'm here to help.",
            "Please wait a moment.",
            "Message received and noted.",
            "Got it, processing your request.",
            "Is there anything else you need?",
            "Request successfully completed.",
            "Your request has been forwarded.",
            "Have a great day!",
            "Bye"
        };
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
        var random = new Random();
        while (true)
        {
            try
            {
                var buffer = new byte[1024];
                var receivedBytes = server.Receive(buffer);
                var receivedMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                ChatPartner = server.RemoteEndPoint.ToString();
                OnMessageReceived(receivedMessage);

                if (receivedMessage.Equals("Bye", StringComparison.OrdinalIgnoreCase))
                    break;
                var response = string.IsNullOrWhiteSpace(MessageToSend) ? responsesArray[random.Next(responsesArray.Length)] : MessageToSend;
                var sendBytes = Encoding.ASCII.GetBytes(response);
                server.Send(sendBytes);
                OnMessageSent(response);
                MessageToSend = string.Empty;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex.Message);
            }
        }
    }

    public void Dispose()
    {
        Finish();
        socket?.Dispose();
        server?.Dispose();
    }
}
