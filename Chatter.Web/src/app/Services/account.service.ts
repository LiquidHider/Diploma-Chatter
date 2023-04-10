import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject, map } from 'rxjs';
import { environment } from 'src/Environments/environment';
import { User } from '../Models/user';
import { RegistrationRequest } from '../Models/registrationRequest';

@Injectable({
    providedIn: 'root'
})

export class AccountService
{
    securityApiBaseUrl = environment.securityApiUrl;
    userSourceBufferSize = 1;

    private currentUserSource = new ReplaySubject<User | null>(this.userSourceBufferSize);
    currentUser = this.currentUserSource.asObservable();

    constructor(private http: HttpClient) {}

    login(model: User){
        return this.http.post<User>(this.securityApiBaseUrl + 'signIn', model).pipe(
            map((response: User) => {
                const user = response;
                if(user){
                  this.setCurrentUser(user);
                }
              })
        );
    }

    register(model: RegistrationRequest){
      return this.http.post<User>(this.securityApiBaseUrl + 'signUp', model).pipe(
        map((user : User) => {
          
            if(user){
              this.setCurrentUser(user);
            }
        })
      );
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