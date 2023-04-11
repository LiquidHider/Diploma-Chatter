import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { RegistrationRequest } from 'src/app/Models/registrationRequest';
import { User } from 'src/app/Models/user';
import { AccountService } from 'src/app/Services/account.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent {

  model: RegistrationRequest = new RegistrationRequest;

  constructor(public accountService: AccountService, private router: Router) {}

  register()
  {
    this.accountService.register(this.model).subscribe((response: Object | User) => {
      this.router.navigateByUrl('/');
    });
  }
}
