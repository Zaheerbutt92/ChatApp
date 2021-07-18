import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model:any={};

  constructor(private accountService:AccountService,
    private router: Router,
    private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  register() {
    this.accountService.register(this.model).subscribe(
      (response) => {
        this.router.navigateByUrl('/');
        console.log(response);
      },
      (error) => {
        console.log(error);
        this.toastr.error(error.error);
      }
    );
  }
}
