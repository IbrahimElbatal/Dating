import { MessagesResolver } from './_resolvers/messages.resolver';
import { ListResolver } from './_resolvers/list.resolver';
import { UnSaveEditUser } from './_services/unsave-edit-user.guard';
import { UserDetailsResolver } from './_resolvers/user-details.resolver';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { AuthGuard } from './_services/auth.guard';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { Routes, RouterModule, Resolve } from '@angular/router';
import { NgModule } from "@angular/core";
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { UserListResolver } from './_resolvers/user-list.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { UserEditResolver } from './_resolvers/user-edit.resolver';

const routes : Routes = [
    {component :HomeComponent , path:''},
    {
        path :'',
        runGuardsAndResolvers :'always',
        canActivate:[AuthGuard],
        children:[
            {
                component: MemberListComponent ,
                path:'members', 
                resolve : {'users':UserListResolver}
            },
            {
                component: MemberEditComponent ,
                path:'members/edit' ,
                resolve:{'user': UserEditResolver},
                canDeactivate :[UnSaveEditUser]

            },
            {
                component: MemberDetailsComponent ,
                path:'members/:id' ,
                resolve:{'user': UserDetailsResolver}
            },
            {component:ListsComponent,path:'lists',resolve :{'list' : ListResolver}},
            {component:MessagesComponent,path:'messages',resolve :{'messages' : MessagesResolver}}
        ]
    },
    {component :RegisterComponent , path:'register'},
    {path:'**',redirectTo:'',pathMatch:'full'}
];

@NgModule({
    imports:[RouterModule.forRoot(routes)],
    exports:[RouterModule]
})
export class AppRoutingModule{
}