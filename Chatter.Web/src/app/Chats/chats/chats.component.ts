import { Component } from '@angular/core';
import { AccountService } from 'src/app/Services/account.service';

@Component({
  selector: 'app-chats',
  templateUrl: './chats.component.html',
  styleUrls: ['./chats.component.css']
})
export class ChatsComponent {

  constructor(public accountService: AccountService) {
    
  }

  logOut(){
    this.accountService.logout();
  }
}
