export class RegistrationRequest{
    lastName: string = "";
    firstName: string = "";
    patronymic!: string;
    userTag!: string;
    universityName!: string;
    universityFaculty!: string;
    email: string = "";
    password: string = "";
    confirmPassword: string = "";
}