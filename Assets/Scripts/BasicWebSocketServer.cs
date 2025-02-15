using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Generic;

// Este script se adjunta a un GameObject en Unity para iniciar el servidor WebSocket.
public class BasicWebSocketServer : MonoBehaviour
{
    // Instancia del servidor WebSocket.
    private WebSocketServer wss;


    // Se ejecuta al iniciar la escena.
    void Start()
    {
        // Crear un servidor WebSocket que escucha en el puerto 7777.
        wss = new WebSocketServer(7777);

        // Añadir un servicio en la ruta "/" que utiliza el comportamiento ChatBehavior.
        wss.AddWebSocketService<ChatBehavior>("/");

        // Iniciar el servidor.
        wss.Start();

        Debug.Log("Servidor WebSocket iniciado en ws://127.0.0.1:7777/");


    }

    // Se ejecuta cuando el objeto se destruye (por ejemplo, al cerrar la aplicación o cambiar de escena).
    void OnDestroy()
    {
        // Si el servidor no está activo, se detiene de forma limpia.
        if (wss != null)
        {
            wss.Stop();
            wss = null;
            Debug.Log("Servidor WebSocket detenido.");
        }
    }


}

// Comportamiento del servicio WebSocket para un chat, que asigna un identificador único a cada cliente.
public class ChatBehavior : WebSocketBehavior
{
    // Contador estático para asignar identificadores únicos a los usuarios.
    private static int connectionCount = 0;
    private string userName;
    // Diccionario de colores para cada usuario (opcional, puedes asignar un color en función del número de usuario).
    private Dictionary<string, string> userColors;



    // Se invoca cuando un cliente se conecta.
    protected override void OnOpen()
    {


        userColors = new Dictionary<string, string>
        {
            { "Usuario1", "#FF5733" },
            { "Usuario2", "#33FF57" },
            { "Usuario3", "#3357FF" },
            { "Usuario4", "#F5B041" },
            { "Usuario5", "#9B59B6" }
        };
        // Incrementa el contador y asigna un nombre de usuario único.
        connectionCount++;
        userName = "Usuario" + connectionCount;

        // Notifica a todos los clientes que un usuario se ha conectado.
        string userColor = userColors.ContainsKey(userName) ? userColors[userName] : "#FFFFFF";

        Sessions.Broadcast($"<color={userColor}><b>{userName} se ha conectado.</b></color>");
        Debug.Log($"{userName} se ha conectado.");
    }

    // Se invoca cuando se recibe un mensaje desde un cliente.
    protected override void OnMessage(MessageEventArgs e)
    {
        // Notifica a todos los clientes que el usuario se ha desconectado.
        string userColor = userColors.ContainsKey(userName) ? userColors[userName] : "#FFFFFF";
        // Formatea el mensaje incluyendo el identificador del usuario.
        string formattedMessage = $"<color={userColor}><b>{userName}:</b></color>{e.Data}";

        // Envía el mensaje a todos los clientes conectados.
        Sessions.Broadcast(formattedMessage);
        Debug.Log($"Mensaje de {userName}: {e.Data}");
    }

    // Se invoca cuando un cliente se desconecta.
    protected override void OnClose(CloseEventArgs e)
    {
        // Notifica a todos los clientes que el usuario se ha desconectado.
        string userColor = userColors.ContainsKey(userName) ? userColors[userName] : "#FFFFFF";

        Sessions.Broadcast($"<color={userColor}><b>{userName} se ha desconectado.</b></color>");
        Debug.Log($"{userName} se ha desconectado.");
    }
}