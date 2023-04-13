import { HttpClient } from "@angular/common/http";
import { Injectable } from '@angular/core';
import { ChatUser } from "../Models/chatUser";
import { environment } from "src/Environments/environment";
import { AccountService } from "./account.service";
import { PaginatedResult } from "../Models/paginatedResult";
import { ReplaySubject } from "rxjs";

@Injectable({
    providedIn: 'root'
})

export class ChatUserService{
    contacts: ChatUser[] = [];
    userSourceBufferSize = 1;
    private currentUserSource = new ReplaySubject<ChatUser>(this.userSourceBufferSize);
    currentUser = this.currentUserSource.asObservable();
    domainApiBaseUrl = environment.domainApiUrl;

    constructor(private http: HttpClient, private accountService: AccountService) {
        this.setCurrentUser();
        this.loadContacts();
    }


    setCurrentUser(){
        this.accountService.currentUser.subscribe(response =>{  
        this.http.get<ChatUser>(this.domainApiBaseUrl + 'user/?id='+ response?.userID)
        .subscribe((user: ChatUser) => {
            this.currentUserSource.next(user);
        });

        });
    }
    
    loadContacts(){
        var userId;
        this.accountService.currentUser.subscribe(x => userId = x?.userID);
        
        return this.http.post<PaginatedResult<ChatUser>>(this.domainApiBaseUrl + 'user/contacts/?userId=' + userId, {});
    }
}