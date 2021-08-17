import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';

@Injectable({
  providedIn: 'root'
})
export class UserDetailedResolver implements Resolve<Member> {

  constructor(private memberService:MembersService){}

  resolve(route: ActivatedRouteSnapshot): Observable<Member> {
    return this.memberService.getMemberByUserName(<string>route.paramMap.get('username'));
  }
}
