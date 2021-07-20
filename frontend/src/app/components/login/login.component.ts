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
    private router:Router) {}
  ngOnInit(): void {}

  login() {
    this.accountService.login(this.model).subscribe(response => {
        this.router.navigateByUrl('/');
        console.log(response);
      }
    );
  }

  cancelLogin(){
    this.registerMode = !this.registerMode;
  }
}
