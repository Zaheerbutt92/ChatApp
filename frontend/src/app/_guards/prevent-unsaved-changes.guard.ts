import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { UserEditComponent } from '../components/users/user-edit/user-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  // canDeactivate(
  //   component: unknown,
  //   currentRoute: ActivatedRouteSnapshot,
  //   currentState: RouterStateSnapshot,
  //   nextState?: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
  //   return true;
  // }
  canDeactivate(component: UserEditComponent):  boolean   {
    if(component.editFrom.dirty){
      return confirm('Are you sure you want to continue? Any unsaved changes will be lost');
    }
    return true;
  }
  
}
