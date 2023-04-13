import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './Home/home/home.component';
import { RegistrationComponent } from './Forms/Registration/registration/registration.component';

const routes: Routes = [
  {path: '', component: HomeComponent,
   runGuardsAndResolvers: 'always',
  children: []},
  {path: 'signup', component: RegistrationComponent},  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
