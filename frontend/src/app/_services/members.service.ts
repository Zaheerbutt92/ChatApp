import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';


@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user!: User;
  userParams!: UserParams;

  constructor(private http: HttpClient,private accountService:AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    });
  }

  getUserParams(){
    return this.userParams;
  }

  setUserParams(params:UserParams){
    this.userParams = params;
  }

  resetUserParams(){
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }
  
  getMembers(userParams: UserParams) {
    var response = this.memberCache.get(Object.values(userParams).join('-'));
    if(response)
    {
      return of(response);
    }

    let params = this.getPaginationHeaders(userParams.pageNumber,userParams.pageSize);

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params)
    .pipe(map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      }));
  }

  getMemberByUserName(username: string) {
    const member = [...this.memberCache.values()]
    .reduce((arr,elem)=> arr.concat(elem.result), [])
    .find((member:Member) => member.username == username);

    if(member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  getMemberById(id: number) {
    const member = [...this.memberCache.values()]
    .reduce((arr,elem)=> arr.concat(elem.result), [])
    .find((member:Member) => member.id == id);

    if(member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + id);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  private getPaginatedResult<T>(url: string, params: HttpParams) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map((response) => {
        paginatedResult.result = response.body as T;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(
            response.headers.get('Pagination') as string
          );
        }
        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);

    return params;
  }
}
