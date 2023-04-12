import { Dictionary } from "../DataTypes/dictionary";

export class RegistrationValidationOptions{
    userTagMinLength: number = 3;
    userTagMaxLength: number = 20;

    lastNameMinLength: number = 2;
    lastNameMaxLength: number = 20;

    firstNameMinLength: number = 2;
    firstNameMaxLength: number = 20;

    patronymicMinLength: number = 2;
    patronymicMaxLength: number = 20;

    universityNameMinLength: number = 3;
    universityNameMaxLength: number = 50;

    universityFacultyMinLength: number = 2;
    universityFacultyMaxLength: number = 50;

    passwordMinLength: number = 6;
    passwordMaxLength: number = 50;

    userTagErrorMessages: Dictionary<string> = {
        'minlength': "User tag length should be at least " + this.userTagMinLength + " characters.",
        'maxlength': "User tag length should be less than " + this.userTagMaxLength + " characters.",
    };

    emailErrorMessages: Dictionary<string> = {
        'required': "Email is required field.",
        'email': "Wrong email format.",
    }; 

    lastNameErrorMessages: Dictionary<string> = {
        'required': "Last name is required field.",
        'minlength': "Last name length should be at least " + this.lastNameMinLength + " characters.",
        'maxlength': "Last name length should be less than " + this.lastNameMaxLength + " characters.",
    };

    firstNameErrorMessages: Dictionary<string> = {
        'required': "First name is required field.",
        'minlength': "First name length should be at least " + this.firstNameMinLength + " characters.",
        'maxlength': "First name length should be less than " + this.firstNameMaxLength + " characters.",
    };

    patronymicErrorMessages: Dictionary<string> = {
        'minlength': "Patronymic length should be at least " + this.patronymicMinLength + " characters.",
        'maxlength': "Patronymic length should be less than " + this.patronymicMaxLength + " characters.",
    };

    universityNameErrorMessages: Dictionary<string> = {
        'minlength': "University name length should be at least " + this.universityNameMinLength + " characters.",
        'maxlength': "University name length should be less than " + this.universityNameMaxLength + " characters.",
    };

    universityFacultyErrorMessages: Dictionary<string> = {
        'minlength': "University faculty length should be at least " + this.universityFacultyMinLength + " characters.",
        'maxlength': "University faculty length should be less than " + this.universityFacultyMaxLength + " characters.",
    };

    passwordErrorMessages: Dictionary<string> = {
        'required': "Password is required.",
        'minlength': "Password length should be at least " + this.passwordMinLength + " characters.",
        'maxlength': "Password length should be less than " + this.passwordMaxLength + " characters.",
    };

    confirmPasswordErrorMessages: Dictionary<string> = {
        'required': "Password confirmation is required.",
        'isMatching': "Passwords do not match."
    };
}