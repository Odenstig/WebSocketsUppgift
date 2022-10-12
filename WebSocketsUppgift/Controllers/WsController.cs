using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace WebSocketsUppgift.Controllers
{
    public class WsController : Controller
    {
        //Server

        //Setting up logger to receive data sent from client in console
        private readonly ILogger<WsController> _logger;

        //Simple class for Json conversion
        private class Text
        {
            public string Message { get; set; }
        }

        public WsController(ILogger<WsController> logger)
        {
            _logger = logger;
        }

        //Get Task setup to receive websocket requests from client
        [HttpGet("/ws")]
        public async Task Get()
        {
            //If it's a ws request establish connection and start receiving/sending data, else throw 400 error
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await Reader(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        //For sending/receiving data in established connection with client
        private async Task Reader(WebSocket webSocket)
        {

            //Creates a new object to be serialized 
            var establishedMessage = new Text()
            {
                Message = "Server ready to receive data!"
            };

            //Serialize above object
            var establishedJson = JsonSerializer.Serialize(establishedMessage);

            //encodes our json object into bytes and sends our encoded object over the connection to our client
            var bytes = Encoding.UTF8.GetBytes(establishedJson);
            await webSocket.SendAsync(bytes, WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);

            Text message = new Text()
            {
                Message = "Hello Client!"
            };

            var json = JsonSerializer.Serialize(message);

            // Receives/Sends data while handshake between client is open
            while (!webSocket.CloseStatus.HasValue)
            {

                //Creates and allocates bytes to buffer
                var buffer = new byte[2048];

                //Receives byte encoded json object from client and fills buffer
                var content = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                //Grabs encoded json object from buffer and decodes into json string
                var s = Encoding.UTF8.GetString(buffer, 0, content.Count);

                //Deserializes json string into Text object
                var receivedJson = JsonSerializer.Deserialize<Text>(s);

                //Writes out our Text objects' content
                _logger.LogInformation("Message from client: {0} ", receivedJson.Message);

                //Encodes our serialized 'message' Text object we made earlier into bytes
                var bytes2 = Encoding.UTF8.GetBytes(json);

                //Sends our encoded 'message' over the connection to our client
                await webSocket.SendAsync(bytes2, WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);
            }

            //Closes handshake/connection between server/client. Reached if potential client closes connection which causes the whileloop to break
            await webSocket.CloseAsync(
                webSocket.CloseStatus.Value,
                webSocket.CloseStatusDescription,
                CancellationToken.None);
        }
    }
}
