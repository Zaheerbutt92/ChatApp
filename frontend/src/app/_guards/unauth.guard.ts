import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
export class UnauthGuard implements CanActivate {
  // canActivate(
  //   route: ActivatedRouteSnapshot,
  //   state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
  //   return true;
  // }
  constructor(private accountService:AccountService, 
    private toastr:ToastrService,
    private router:Router){}
    
  canActivate(route: ActivatedRouteSnapshot): Observable<boolean> {
    return (this.accountService.currentUser$).pipe(
      map(user  => {
        if(!user){
         return true;
        }
        this.toastr.warning('Please logout to visit this page.');
        this.router.navigateByUrl('/');
        return false;
      })
    );
  }
  
}

