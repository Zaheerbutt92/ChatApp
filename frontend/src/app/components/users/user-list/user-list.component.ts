import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {
  members$!: Observable<Member[]>;

  constructor(private memberService:MembersService) { }

  ngOnInit(): void {
    this.members$ = this.memberService.getMembers()
  }
}
