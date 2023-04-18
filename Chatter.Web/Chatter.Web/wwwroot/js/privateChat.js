const chatBody = document.getElementById("chatBody");

const chatMessages = document.getElementById("chatMessages");

const selectChatMessage = document.getElementById("selectChatMessage");

const parsedCookie = JSON.parse(decodeCookie(getCookie("User")));

const domainBaseUrl = 'https://localhost:7258/';

const currentUserId = parsedCookie.userID;

const userJwtToken = parsedCookie.token;

chatBody.hidden = true;

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
            'Authorization' : 'Bearer ' + userJwtToken
        }
    })
        .then(response => {
            clearPrivateChat();
            response.json().then(messages => {
                console.log(messages)
                for (var i = 0; i < messages.length; i++) { 
                    displayMessage(messages[i], chatMessages);
                }
            })
        })
        .catch(error => console.log('Error opening private chat.', error));
}

function displayMessage(mes, chatBodyElement) {
    var chatMessage = document.createElement("div");
    var chatMessageBody = document.createElement("div");
    var chatMessageSender = document.createElement("div");
    var chatMessageTime = document.createElement("div");

    chatMessageSender.innerHTML += isSenderCurrentUser(mes.senderId) ? "You" : mes.senderId;
    chatMessageBody.innerHTML += mes.body;
    chatMessageTime.innerHTML += mes.sent;

    chatMessage.appendChild(chatMessageSender);
    chatMessage.appendChild(chatMessageBody);
    chatMessage.appendChild(chatMessageTime);
  
    chatBodyElement.appendChild(chatMessage);
}

function isSenderCurrentUser(message) {
    return message.senderId === currentUserId;
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