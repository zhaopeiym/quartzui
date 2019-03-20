import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ExplainComponent } from './explain/explain.component';

const routes: Routes = [{
  path: '',
  component: ExplainComponent
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ExplainRoutingModule { }
