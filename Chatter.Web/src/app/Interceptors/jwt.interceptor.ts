import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountService } from '../Services/account.service';
import { switchMap, take } from 'rxjs/operators';
import { Login } from '../Models/login';
import { User } from '../Models/user';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService : AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return this.accountService.currentUser.pipe(take(1),
      switchMap((currentUser: User | null) => {
        if (currentUser) {
          request = request.clone({
            setHeaders: {
              Authorization: 'Bearer ' + currentUser.token
            }
          });
        }
        return next.handle(request);
      })
    );
  }
}
