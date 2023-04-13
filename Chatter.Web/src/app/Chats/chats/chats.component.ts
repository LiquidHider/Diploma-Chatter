import { Component, OnInit } from '@angular/core';
import { environment } from 'src/Environments/environment';
import { ChatUser } from 'src/app/Models/chatUser';
import { PaginatedResult } from 'src/app/Models/paginatedResult';
import { AccountService } from 'src/app/Services/account.service';
import { ChatService } from 'src/app/Services/chat.service';
import { ChatUserService } from 'src/app/Services/chatUser.service';

@Component({
  selector: 'app-chats',
  templateUrl: './chats.component.html',
  styleUrls: ['./chats.component.css']
})
export class ChatsComponent implements OnInit {
  defaultUserIcon: string = environment.userDefaultIcon;
  contacts: any = [];

  constructor(public accountService: AccountService, 
    private chatUserService: ChatUserService, 
    private chatService: ChatService) {
    
  } 
  ngOnInit(): void {
    this.chatUserService.loadContacts().subscribe(
      (object: PaginatedResult<ChatUser>) => {
        this.contacts = object.items;
      });
  }
  openChat(interlocutorId: string){
    //TODO: make messages display on screen
    console.log(interlocutorId);
    this.chatUserService.currentUser.subscribe((chatUser: ChatUser) => {
      var userID: string = chatUser.id.toString();
      this.chatService.openChat(userID, interlocutorId).subscribe(response => console.log(response));
    });

  }
  logOut(){
    this.accountService.logout();
  }
}
