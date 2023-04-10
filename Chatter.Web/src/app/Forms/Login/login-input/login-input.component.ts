import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LoginRequest } from 'src/app/Models/loginRequest';
import { AccountService } from 'src/app/Services/account.service';

@Component({
  selector: 'app-login-input',
  templateUrl: './login-input.component.html',
  styleUrls: ['./login-input.component.css']
})
export class LoginInputComponent {

  constructor(public accountService: AccountService, private router: Router) {
  
  }

 loginModel: LoginRequest = new LoginRequest;

 
  login() {
    this.accountService.login(this.loginModel).subscribe(response => {
      
    });
  }

  logout() {
    this.router.navigateByUrl('/');
    this.accountService.logout();
  }
}
