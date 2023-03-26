﻿using Chatter.Domain.BusinessLogic.Models.Abstract;

namespace Chatter.Domain.BusinessLogic.Models.ChatMessages
{
    public class PrivateChatMessage : ChatMessage
    {
        public ChatUser Recipient { get; set; }
    }
}
