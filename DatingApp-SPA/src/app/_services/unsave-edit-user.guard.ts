import { MemberEditComponent } from './../members/member-edit/member-edit.component';
import { Injectable } from "@angular/core";
import { CanDeactivate, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';

@Injectable({
    providedIn:'root'
})

export class UnSaveEditUser implements CanDeactivate<MemberEditComponent>{
    canDeactivate(component: MemberEditComponent): boolean  {
            if(component.editForm.dirty){
                return confirm('Are you sure you want to leave ?');
            }
            return true;
    }

}