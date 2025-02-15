using UnityEngine;
using WebSocketSharp;
using System.Collections.Generic;

public class BasicWebSocketClient : MonoBehaviour
{
    private WebSocket ws;
    public ChatManager chatManager;
    private Queue<string> messageQueue = new Queue<string>();

    void Start()
    {
        ws = new WebSocket("ws://127.0.0.1:7777/");

        // Intentar encontrar automáticamente el ChatManager si no está asignado
        if (chatManager == null)
        {
            chatManager = FindObjectOfType<ChatManager>();
        }

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket conectado correctamente.");
        };

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Mensaje recibido: " + e.Data);
            lock (messageQueue)
            {
                messageQueue.Enqueue(e.Data);
            }
        };

        ws.OnError += (sender, e) =>
        {
            Debug.LogError("Error en el WebSocket: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket cerrado. Código: " + e.Code + ", Razón: " + e.Reason);
        };

        ws.ConnectAsync();
    }

    void Update()
    {
        // Procesar mensajes en la cola de mensajes en el hilo principal
        while (messageQueue.Count > 0)
        {
            string message;
            lock (messageQueue)
            {
                message = messageQueue.Dequeue();
            }

            if (chatManager != null)
            {
                chatManager.ReceiveMessage(message);
            }
            else
            {
                Debug.LogWarning("ChatManager no está asignado, mensaje no mostrado.");
            }
        }
    }

    public void SendMessageToServer(string message)
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Send(message);
        }
        else
        {
            Debug.LogError("No se puede enviar el mensaje. La conexión no está abierta.");
        }
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }
    }
}