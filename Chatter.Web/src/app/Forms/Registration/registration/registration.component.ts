import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CustomValidators } from 'src/app/Extensions/validators.extention';
import { RegistrationValidationOptions } from 'src/app/Helpers/registrationValidationOptions';
import { RegistrationRequest } from 'src/app/Models/registrationRequest';
import { User } from 'src/app/Models/user';
import { AccountService } from 'src/app/Services/account.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit, OnDestroy {


  validationOptions = new RegistrationValidationOptions();
  model: RegistrationRequest = new RegistrationRequest;
  userAlreadyRegitered = false;
  registrationForm: FormGroup = this.formBuilder.group({

    userTag: ['userTag', [
      Validators.minLength(this.validationOptions.userTagMinLength), 
      Validators.maxLength(this.validationOptions.userTagMaxLength),
    ]],

    email: ['email', [Validators.required, Validators.email]],

    lastName: ['lastName', [
      Validators.required,
      CustomValidators.noDigits(null),
      Validators.minLength(this.validationOptions.lastNameMinLength), 
      Validators.maxLength(this.validationOptions.lastNameMaxLength),
      ]],

    firstName: ['firstName', [
      Validators.required, 
      CustomValidators.noDigits(null),
      Validators.minLength(this.validationOptions.firstNameMinLength), 
      Validators.maxLength(this.validationOptions.firstNameMaxLength)]],

    patronymic: ['patronymic', [
      CustomValidators.noDigits(null),
      Validators.minLength(this.validationOptions.patronymicMinLength), 
      Validators.maxLength(this.validationOptions.patronymicMaxLength)]],

    universityName: ['universityName', [
      CustomValidators.noDigits(null),
      Validators.minLength(this.validationOptions.universityNameMinLength), 
      Validators.maxLength(this.validationOptions.universityNameMaxLength)]],

    universityFaculty: ['universityFaculty', [
      CustomValidators.noDigits(null),
      Validators.minLength(this.validationOptions.universityFacultyMinLength), 
      Validators.maxLength(this.validationOptions.universityFacultyMaxLength)]],

    password: ['password', [
      Validators.required, 
      Validators.minLength(this.validationOptions.passwordMinLength), 
      Validators.maxLength(this.validationOptions.passwordMaxLength)]],

    confirmPassword: ['confirmPassword', [Validators.required, CustomValidators.matchValues('password')]]
  });


  constructor(public accountService: AccountService, private router: Router, private formBuilder: FormBuilder) {}

  ngOnInit(): void {

  }
 
  clearWrongCredsMessage(){
    this.userAlreadyRegitered = false;
  }

  ngOnDestroy(): void {
   
  }

  register(){
    this.userAlreadyRegitered = false;
    this.accountService.register(this.model).subscribe((response: Object | User) => {
      if(Boolean(response) === true){
        this.userAlreadyRegitered = true;
        return;
      }
      this.router.navigateByUrl('/');
    });
  }

  get userTag(): any {
    return this.registrationForm.get('userTag');
  } 
  
  get email(): any {
    return this.registrationForm.get('email');
  }

  get lastName(): any {
    return this.registrationForm.get('lastName');
  }

  get firstName(): any {
    return this.registrationForm.get('firstName');
  }

  get patronymic(): any {
    return this.registrationForm.get('patronymic');
  }

  get universityName(): any {
    return this.registrationForm.get('universityName');
  }

  get universityFaculty(): any {
    return this.registrationForm.get('universityFaculty');
  }

  get password(): any {
    return this.registrationForm.get('password');
  }

  get confirmPassword(): any {
    return this.registrationForm.get('confirmPassword');
  }
}
