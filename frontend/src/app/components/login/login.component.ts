import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  model: any = {};
  registerMode = false;

  constructor(private accountService: AccountService,
    private router:Router,
    private toastr: ToastrService) {}
  ngOnInit(): void {}

  login() {
    this.accountService.login(this.model).subscribe(response => {
        this.router.navigateByUrl('/');
        console.log(response);
      },
      error => {
        this.toastr.error(error.error);
        console.log(error);
      }
    );
  }

  cancelLogin(){
    this.registerMode = !this.registerMode;
  }
}
