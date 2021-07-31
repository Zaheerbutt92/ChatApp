import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-user-edit',
  templateUrl: './user-edit.component.html',
  styleUrls: ['./user-edit.component.css']
})
export class UserEditComponent implements OnInit {
  @ViewChild('editFrom') editFrom!: NgForm;
  member!: Member;
  user!: User;
  @HostListener('window:beforeunload',['$event']) unloadNotification($event: any){
    if(this.editFrom.dirty){
      $event.returnValue = true;
    }
  }
  constructor(private accountService: AccountService, 
      private memberService:MembersService,
      private toastr:ToastrService) 
  {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.loadMember()
  }

  loadMember(){
    this.memberService.getMemberByUserName(this.user.username).subscribe(
      member => this.member = member
    )
  }

  updateMember(){
    this.memberService.updateMember(this.member).subscribe(()=>{
      this.toastr.success('Profile updated successfully.')
      this.editFrom.resetForm(this.member);
    })
  };
}
