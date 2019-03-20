import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      {
        path: '',
        loadChildren: "app/task-list/task-list.module#TaskListModule"
      },
      {
        path: 'seting',
        loadChildren: "app/seting/seting.module#SetingModule"
      },
      {
        path: 'explain',
        loadChildren: "app/explain/explain.module#ExplainModule"
      }
    ]
  },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LayoutRoutingModule { }
