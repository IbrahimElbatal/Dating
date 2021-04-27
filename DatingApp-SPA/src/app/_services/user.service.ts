import { MessageParams } from './../_models/MessageParams.model';
import { tap,map } from 'rxjs/operators';
import { PaginatedResult } from './../_models/paging-result.model';
import { User } from './../_models/user.model';
import { Observable } from 'rxjs';
import { environment } from './../../environments/environment';
import { HttpClient, HttpParams} from '@angular/common/http';
import { Injectable } from "@angular/core";
import { Message } from '../_models/message.model';

@Injectable({
    providedIn :'root'
})
export class UserService{
    url:string = environment.baseUrl+'/Dating/'
    constructor(private http:HttpClient) {
    }

    getUsers(pageNumber?,pageSize?,fromAge?,toAge?,gender?,sort?,likeParams?) :Observable<PaginatedResult<User[]>>{
        const paginationResult = new PaginatedResult<User[]>();
        
        let params = new HttpParams(); 
        if(pageNumber != null && pageSize != null){
            params = params.append('pageNumber',pageNumber);
            params = params.append('pageSize',pageSize);
        }
        if(gender != null){
            params = params.append('gender',gender);
        }
        if(fromAge !=null || toAge !=null){
            params = params.append('ageFrom',fromAge);
            params = params.append('ageTo',toAge);
        }
        if(likeParams == 'Likers' ){
            params = params.append('likers','true');
        }else if(likeParams == 'Likees'){
            params = params.append('likees','true');
        }
        if(sort != null){
            params = params.append('sort',sort);
        }
        return this.http.get<User[]>(this.url,{
            observe : 'response',
            params : params
        }).pipe(
            map((res) => {
                paginationResult.pagination = JSON.parse(res.headers.get("pagination"));
                paginationResult.result = res.body;
                return paginationResult;
            })
        );
    }
    getUser(id:number) :Observable<User>{
        return this.http.get<User>(this.url+id);
    }
    updateUser(id:number,user:User){
        return this.http.put(this.url+id,user);
    }
    deletePhoto(id:number,userId:number){
        return this.http.delete(environment.baseUrl + `/users/${userId}/photos/${id}`);
    }
    setMainPhoto(id:number,userId:number){
        return this.http.post(environment.baseUrl + `/users/${userId}/photos/${id}`,{});
    }
    sendLike(id:number,recipientId:number){
        return this.http.post(`${this.url}/user/${id}/like/${recipientId}`,{});
    }
    getUsersMessages(userId:number,messageParams :MessageParams) :Observable<PaginatedResult<Message[]>>{
        const paginationResult = new PaginatedResult<Message[]>();
        
        let params = new HttpParams(); 
        if(userId !=null)
            params = params.append('userId',userId.toString());

        if(messageParams.pageNumber != null && messageParams.pageSize != null){
            params = params.append('pageNumber',messageParams.pageNumber.toString());
            params = params.append('pageSize',messageParams.pageSize.toString());
        }
        if(messageParams.messageContainer != null){
            params = params.append('messageContainer',messageParams.messageContainer);
        }
        
        return this.http.get<Message[]>(environment.baseUrl +'/message',{
            observe : 'response',
            params : params
        }).pipe(
            map((res) => {
                paginationResult.pagination = JSON.parse(res.headers.get("pagination"));
                paginationResult.result = res.body;
                return paginationResult;
            })
        );
    }

    getUserMessageThread(userId:number,recipientId:number){
        return this.http.get(`${environment.baseUrl}/message/user/${userId}/threadWith/${recipientId}`);
    }
    newMessage(userId:number,data){
        return this.http.post(`${environment.baseUrl}/message/sender/${userId}/newMessage`,data);
    }
    deleteMessage(id,userId){
        return this.http.post(`${environment.baseUrl}/message/${id}/user/${userId}/delete`,{});
    }
    markAsRead(id,userId){
        return this.http.post(`${environment.baseUrl}/message/${id}/user/${userId}/markAsRead`,{})
            .subscribe();
    }
}