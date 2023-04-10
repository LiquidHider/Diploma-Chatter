import { Component } from '@angular/core';
import { RegistrationRequest } from 'src/app/Models/registrationRequest';
import { AccountService } from 'src/app/Services/account.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent {

  model: RegistrationRequest = new RegistrationRequest;

  constructor(public accountService: AccountService) {}

  register()
  {
    this.accountService.register(this.model).subscribe(response => {
      
    });
  }
}
