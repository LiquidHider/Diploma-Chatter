import { Component } from '@angular/core';
import { AccountService } from './Services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'chatter-web';

  constructor(private accountService: AccountService) {
    this.setCurrentUser();
  }

  setCurrentUser(){
    var userFromLocalStorage = localStorage.getItem('user');
    if(userFromLocalStorage){
      this.accountService.setCurrentUser(JSON.parse(userFromLocalStorage!));
    }
  }
}
