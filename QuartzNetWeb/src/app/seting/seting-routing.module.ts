import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SetingComponent } from './seting/seting.component';

const routes: Routes = [ {
  path: '',
  component: SetingComponent
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SetingRoutingModule { }
