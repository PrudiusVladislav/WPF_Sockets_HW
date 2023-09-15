using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace CSharpWPF_NP_Sockets_Task_3.Client.Core;

public class Client : IDisposable
{
    private const string DefaultIpAddress = "127.0.0.1";
    private const int DefaultPort = 5000;

    private readonly EndPoint endPoint;
    private Socket? client;

    public event Action<string>? MessageReceived;
    public event Action<string>? MessageSent;
    public event Action<string>? ErrorOccurred;

    CancellationTokenSource cancellationTokenSource;

    public Client(string ipAddress = DefaultIpAddress, int port = DefaultPort)
    {
        endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        cancellationTokenSource = new CancellationTokenSource();
    }

    public void Connect()
    {
        try
        {
            client = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(endPoint);
        }
        catch(Exception ex)
        {
            OnErrorOccurred(ex.Message);
        }
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

    public void Send(string message)
    {
        if (client is null || !client.Connected)
            Connect();

        var sendBytes = Encoding.ASCII.GetBytes(message);
        client!.Send(sendBytes);
        OnMessageSent(message);
    }

    public string Receive()
    {
        if (client is null || !client.Connected)
            Connect();

        var buffer = new byte[1024];
        var receivedBytes = client!.Receive(buffer);
        var receivedMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
        OnMessageReceived(receivedMessage);       
        return receivedMessage;
    }

    public async Task<string> ReceiveAsync()
    {
        if (client is null || !client.Connected)
            Connect();

        try
        {
            var buffer = new byte[1024];
            var receivedBytes = await client!.ReceiveAsync(buffer, cancellationTokenSource.Token);
            var receivedMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
            OnMessageReceived(receivedMessage);
            return receivedMessage;
        }
        catch(OperationCanceledException) { return ""; }
    }

    public string SendAndReceive(string message)
    {
        Send(message);
        return Receive();
    }

    public void CancelAsyncReceiving()
    {
        cancellationTokenSource.Cancel();
    }

    public void Disconnect()
    {
        CancelAsyncReceiving();
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
