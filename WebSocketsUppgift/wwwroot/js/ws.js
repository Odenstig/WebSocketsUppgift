//Our 'client'

//Establishes connection to our server
const socket = new WebSocket("ws://localhost:5156/ws");

//Creates object to be serialized
let messageObject = {
    Message: "Hello Server!"}

//Serializes object
let jsonObject = JSON.stringify(messageObject)

//Keeps sending our serialized message object to our server as long as connection is open
socket.addEventListener('open', (event) => {
    console.log("Socket open!", event);

    setInterval(() => {
        socket.send(jsonObject);
    }, 3000)
});

//Receives and deserializes message object from server and prints message to console
socket.addEventListener('message', (event) => {

    const obj = JSON.parse(event.data)

    console.log("Message from server: ", obj.Message);
});