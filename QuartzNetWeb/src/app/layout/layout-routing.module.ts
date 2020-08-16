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
        // loadChildren: "app/task-list/task-list.module#TaskListModule"
        loadChildren: () => import('../../app/task-list/task-list.module').then(m => m.TaskListModule)
      },
      {
        path: 'seting',
        // loadChildren: "app/seting/seting.module#SetingModule"
        loadChildren: () => import('../../app/seting/seting.module').then(m => m.SetingModule)
      },
      {
        path: 'explain',
        // loadChildren: "app/explain/explain.module#ExplainModule"
        loadChildren: () => import('../../app/explain/explain.module').then(m => m.ExplainModule)
      }
    ]
  },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LayoutRoutingModule { }
