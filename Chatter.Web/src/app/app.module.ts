import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginInputComponent } from './Forms/Login/login-input/login-input.component';
import { RegistrationComponent } from './Forms/Registration/registration/registration.component';
import { LoginComponent } from './Forms/Login/login-window/login/login.component';
import { HomeComponent } from './Home/home/home.component';
import { ChatsComponent } from './Chats/chats/chats.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { JwtInterceptor } from './Interceptors/jwt.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    LoginInputComponent,
    RegistrationComponent,
    LoginComponent,
    HomeComponent,
    ChatsComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [{provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true}],
  bootstrap: [AppComponent]
})
export class AppModule { }