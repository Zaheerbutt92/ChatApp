import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Chat App';
  users: any;
  
   constructor(public accountService:AccountService, private presence:PresenceService){}
  
   ngOnInit() {
    this.setCurrentUser();
  }

  setCurrentUser(){
    const user:User = JSON.parse(<string>localStorage.getItem('user')); 
    if(user){
      this.accountService.setCurrentUser(user);
      this.presence.createHubConnection(user);
    } 
  }
}


