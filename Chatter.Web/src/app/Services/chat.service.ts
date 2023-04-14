import { HttpClient } from "@angular/common/http";
import { Injectable } from '@angular/core';
import { environment } from "src/Environments/environment";


@Injectable({
    providedIn: 'root'
})

export class ChatService{
    domainApiBaseUrl = environment.domainApiUrl;
    messageSent = environment.messageSent;
    messageSeen = environment.messageSeen;
    sendMessage = environment.sendMessage;

    constructor(private http: HttpClient) {
        
    }

    openChat(member1ID: string, member2ID: string){
        return this.http.post(this.domainApiBaseUrl + "chat", { Member1ID: member1ID, Member2ID: member2ID });
    }
}