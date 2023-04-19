const chatBody = document.getElementById("chatBody");

const chatMessages = document.getElementById("chatMessages");
const chatMessageInput = document.getElementById("messageInput");

const selectChatMessage = document.getElementById("selectChatMessage");

const parsedCookie = JSON.parse(decodeCookie(getCookie("User")));

const domainBaseUrl = 'https://localhost:7258/';

const currentUserId = parsedCookie.userID;

var currentInterlocutorId = "";
var currentInterlocutor = null;

const userJwtToken = parsedCookie.token;

var connection = new signalR.HubConnectionBuilder()
    .withUrl(domainBaseUrl + "chatHub", {
        transport: signalR.HttpTransportType.WebSockets
    })
    .build();


connection.on("NewMessg-" + currentUserId, function (mesgResult) {
    if (currentInterlocutorId = mesgResult.value.sender) {
        var messageModel = {
            body: mesgResult.value.body,
            id: mesgResult.value.id,
            isEdited: mesgResult.value.isEdited,
            isRead: mesgResult.value.isRead,
            recipientID: mesgResult.value.recipientID,
            senderId: mesgResult.value.sender,
            sent: mesgResult.value.sent
        };
        displayMessage(messageModel, chatMessages);
    }

});

connection.on("SentMessg-" + currentUserId, function (mesgResult) {
    if (currentInterlocutorId = mesgResult.value.sender) {
        var messageModel = {
            body: mesgResult.value.body,
            id: mesgResult.value.id,
            isEdited: mesgResult.value.isEdited,
            isRead: mesgResult.value.isRead,
            recipientID: mesgResult.value.recipientID,
            senderId: mesgResult.value.sender,
            sent: mesgResult.value.sent
        };
    }

    displayMessage(messageModel, chatMessages);

});

connection.start();


chatBody.hidden = true;

function sendMessage() {
    let message = {
        SenderID: currentUserId,
        RecipientID: currentInterlocutorId,
        Body: chatMessageInput.value
    };
    connection.invoke("SendChatMessage", message);
    chatMessageInput.value = "";
}

function openPrivateChat(member1Id, member2Id) {
    selectChatMessage.hidden = true;
    chatBody.hidden = false;
    const requestModel = {
        Member1ID: member1Id,
        Member2ID: member2Id
    };

    fetch(domainBaseUrl + 'chat', {
        method: 'POST',
        body: JSON.stringify(requestModel),
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + userJwtToken
        }
    })
        .then(response => {
            clearPrivateChat();
            response.json().then(messages => {
                currentInterlocutorId = member2Id;

                fetch(domainBaseUrl + 'user/?id=' + currentInterlocutorId, {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + userJwtToken
                    }
                })
                    .then(response => {
                        response.json().then(user => {
                            currentInterlocutor = user
                            for (var i = 0; i < messages.length; i++) {
                                displayMessage(messages[i], chatMessages);
                            }
                        })
                    })
                    .catch(error => console.log('Error finding current interlocutor.', error));
            })
        })
        .catch(error => console.log('Error opening private chat.', error));
}


function displayMessage(mes, chatBodyElement) {
    var chatMessage = document.createElement("div");
    var chatMessageBody = document.createElement("div");
    var chatMessageSender = document.createElement("div");
    var chatMessageTime = document.createElement("div");
    
    chatMessageSender.innerHTML += isSenderCurrentUser(mes) == true ? "You" : currentInterlocutor.firstName;
    chatMessageBody.innerHTML += mes.body;
    chatMessageTime.innerHTML += mes.sent;

    chatMessage.appendChild(chatMessageSender);
    chatMessage.appendChild(chatMessageBody);
    chatMessage.appendChild(chatMessageTime);
  
    chatBodyElement.appendChild(chatMessage);
}

function isSenderCurrentUser(message) {
    return message.senderId == currentUserId;
}

function clearPrivateChat(){
    chatMessages.innerHTML = "";
}

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}

function decodeCookie(cookieValue) {
    return decodeURIComponent(cookieValue.replace(/\+/g, ' '));
}