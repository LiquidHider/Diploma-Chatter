import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Login } from 'src/app/Models/login';
import { AccountService } from 'src/app/Services/account.service';

@Component({
  selector: 'app-login-input',
  templateUrl: './login-input.component.html',
  styleUrls: ['./login-input.component.css']
})
export class LoginInputComponent {

  constructor(public accountService: AccountService, private router: Router) {
  
  }

  loginModel: Login = new Login;
  wrongCreds = false;
 
  login() {
    this.wrongCreds = false;
    this.accountService.login(this.loginModel).subscribe(response => {
      if(response === 400){
        this.wrongCreds = true;
      }
    });
  }

  logout() {
    this.router.navigateByUrl('/');
    this.accountService.logout();
  }
}
