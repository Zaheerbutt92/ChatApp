import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/popmodels/roles-modal/roles-modal.component';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-user-managment',
  templateUrl: './user-managment.component.html',
  styleUrls: ['./user-managment.component.css']
})
export class UserManagmentComponent implements OnInit {
  users: Partial<User[]> = [];
  bsModalRef! : BsModalRef;

  constructor(private adminService: AdminService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles(){
    this.adminService.getUsersWithRoles().subscribe(users =>{
        this.users = users;
    });
  }

  openRolesModal(user:User){
    const config = {
      class:'modal-dialog-centered',
      initialState: {
        user,
        roles: this.getRolesArray(user)
      }
    }
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    this.bsModalRef.content.updateSelectRoles.subscribe((values: any[]) =>{
      const rolesToUpdate= {
        roles: [...values.filter(el =>el.checked == true).map(el => el.name)]
      }
      if(rolesToUpdate){
        this.adminService.updateUserRoles(user.username,rolesToUpdate.roles).subscribe(() =>{
          user.roles = [...rolesToUpdate.roles];
        })
      }
    });
  }

  private getRolesArray(user:User){
    const roles:any[]= [];
    const userRoles = user.roles;
    const availableRoles: any[]= [
      {name: 'Admin',value: 'Admin'},
      {name: 'Moderator',value: 'Moderator'},
      {name: 'Member',value: 'Member'},
    ];

    availableRoles.forEach(role => {
      let isMatch = false;
      for(const userRole of userRoles){
        if(role.name === userRole){
          isMatch = true;
          role.checked = true;
          roles.push(role);
          break;
        }
      }
      if(!isMatch){
        role.checked = false;
        roles.push(role);
      }
    })
    return roles;
  }
}
