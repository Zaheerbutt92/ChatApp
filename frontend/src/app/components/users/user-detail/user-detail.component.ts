import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.css']
})
export class UserDetailComponent implements OnInit {
  member!: Member;
  galleryOptions!: NgxGalleryOptions[];
  galleryImages!: NgxGalleryImage[];

  constructor(private memberService:MembersService, private route:ActivatedRoute) { }

  ngOnInit(): void {
    this.loadMember();
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent:100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview:false
      }];

  }

  getImages(): NgxGalleryImage[]
  {
    const imageUrls = [];
    for(const photo of this.member.photos ){
      imageUrls.push({
        small :photo?.url,
        medium :photo?.url,
        big :photo?.url,
      })
    }
    return imageUrls;
  }
  
  loadMember(){
    this.memberService.getMemberByUserName(<string>this.route.snapshot.paramMap.get('username')).subscribe(
      member => {
        this.member = member
        this.galleryImages = this.getImages();
      }
    )
  }

}
