using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    public TMP_Text chatDisplay;      
    public TMP_InputField inputField; 
    public Button sendButton;         
    public ScrollRect scrollRect;    
    public BasicWebSocketClient wsClient;


    // Variable para almacenar el nombre de usuario de esta instancia.
    public string userName;

    // Contador est�tico para asignar nombres �nicos.
    private static int instanceCount = 0;

    // Diccionario de colores para cada usuario (opcional, puedes asignar un color en funci�n del n�mero de usuario).
    private Dictionary<string, string> userColors;

    void Awake()
    {
        instanceCount++;
        PlayerPrefs.SetInt("InstanceCount", instanceCount);
        PlayerPrefs.Save();
        userName = "Usuario" + instanceCount;
    }


    void Start()
    {
        sendButton.onClick.AddListener(SendMessage);
        inputField.onSubmit.AddListener(delegate { SendMessage(); });

        // Limpiar el chatDisplay
        chatDisplay.text = "";

        // Dar foco autom�tico al input al iniciar
        inputField.Select();
        inputField.ActivateInputField();
    }

    public void SendMessage()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            // Formatear el mensaje con el color y el nombre del usuario.
            string formattedMessage = $"{inputField.text}";

            // Enviar el mensaje al servidor vía WebSocket.
            BasicWebSocketClient wsClient = FindObjectOfType<BasicWebSocketClient>();
            if (wsClient != null)
            {
                wsClient.SendMessageToServer(formattedMessage);
            }
            else
            {
                Debug.LogError("No se encontr� el BasicWebSocketClient en la escena.");
            }


            // Limpiar el input y volver a activarlo.
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    public void ReceiveMessage(string message)
    {
        // Agrega el mensaje recibido al historial.
        chatDisplay.text += "\n" + message;

        // Actualiza el layout y realiza el scroll.
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatDisplay.rectTransform);
        ScrollToBottom();
    }

    void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}