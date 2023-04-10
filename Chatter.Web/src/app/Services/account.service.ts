import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject, empty, map } from 'rxjs';
import { environment } from 'src/Environments/environment';
import { User } from '../Models/user';
import { Login } from '../Models/login';
import { RegistrationRequest } from '../Models/registrationRequest';

@Injectable({
    providedIn: 'root'
})

export class AccountService
{
    securityApiBaseUrl = environment.securityApiUrl;
    domainApiBaseUrl = environment.domainApiUrl;
    userSourceBufferSize = 1;
    private currentUserSource = new ReplaySubject<User | null>(this.userSourceBufferSize);
    currentUser = this.currentUserSource.asObservable();

    constructor(private http: HttpClient) {}

    login(model: Login){
      const mappedRequest = {
        Email: this.isEmailValue(model.emailOrUserTag) === true ? model.emailOrUserTag : null,
        UserTag: this.isEmailValue(model.emailOrUserTag) === false ? model.emailOrUserTag : null,
        Password: model.password
      };

        return this.http.post<User>(this.securityApiBaseUrl + 'signIn', mappedRequest).pipe(
            map((response: User) => {
                const user = response;
                if(user){
                  this.setCurrentUser(user);
                }
              })
        );
    }

    register(model: RegistrationRequest){
      const domainApiRequest = {
        LastName: model.lastName,
        FirstName: model.firstName,
        Patronymic: model.patronymic,
        UniversityName: model.universityName,
        UniversityFaculty: model.universityFaculty
      };
      var securityApiRequest = {};

      this.http.post(this.domainApiBaseUrl + 'new', domainApiRequest).pipe(
        map((response : any) => {
          if(response){
            securityApiRequest = { 
              Email: model.email,
              UserTag: model.userTag,
              Password: model.password,
              UserId: response.CreatedId
            };
          }
        })
      );

      return this.http.post<User>(this.securityApiBaseUrl + 'signup', securityApiRequest).pipe(
        map((response : User) => {
          if(response){
            this.setCurrentUser(response);
          }
      }));
    }

    isEmailValue(email: string): boolean {
      let regexp = new RegExp("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$");
      return regexp.test(email);
    }
    
    isUserLoggedIn = this.currentUser.pipe( 
      map(
        (user: User | null) => {
        return (user !== null)
      }) 
    );

    getDecodedToken(token: string){
        return JSON.parse(token.split('.')[1]);
      }
    

    setCurrentUser(user: User){
        user.roles = [];
        const roles = this.getDecodedToken(user.token).role;
        Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
        localStorage.setItem('user', JSON.stringify(user));
        this.currentUserSource.next(user);
      }

      logout(){
        localStorage.removeItem('user');
        this.currentUserSource.next(null);
      }
}