import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject,  map, switchMap } from 'rxjs';
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

    constructor(private http: HttpClient) {
      var userFromLocalStorage = localStorage.getItem('user');
      if(userFromLocalStorage){
        this.setCurrentUser(JSON.parse(userFromLocalStorage!));
      }
    }


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
      var identityExistsRequest = this.http.post(this.securityApiBaseUrl + 'exists',
       {email: model.email, userTag: model.userTag});

       var isIdentityExists = identityExistsRequest.pipe(map(request => request));
       if(isIdentityExists){
        return identityExistsRequest;
       }

      const domainApiRequest = {
        lastName: model.lastName,
        firstName: model.firstName,
        patronymic: model.patronymic,
        universityName: model.universityName,
        universityFaculty: model.universityFaculty
      };
      return this.http.post(this.domainApiBaseUrl + 'user/new', domainApiRequest).pipe(
        switchMap((domainResponse: any) => {
          var securityApiRequest = {};
          if (domainResponse) {
            securityApiRequest = {
              Email: model.email,
              UserTag: model.userTag,
              Password: model.password,
              UserId: domainResponse.createdId
            };
          }
          return this.http.post<User>(this.securityApiBaseUrl + 'signup', securityApiRequest);
        })
      );
    }

    isEmailValue(email: string): boolean {
      let regexp = new RegExp("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$");
      return regexp.test(email);
    }
    
    isUserLoggedIn()
    { 
      return this.currentUser.pipe( 
      map(
        (user: User | null) => {
        return (user != null)
      }));
    }

    getDecodedToken(token: string){
        return JSON.parse(atob(token.split('.')[1]));
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