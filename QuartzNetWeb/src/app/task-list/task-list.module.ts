import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TaskListRoutingModule } from './task-list-routing.module';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgZorroAntdModule } from 'ng-zorro-antd';
import { TaskListComponent } from './task-list/task-list.component';

@NgModule({
  imports: [
    CommonModule,
    TaskListRoutingModule,
    NgZorroAntdModule.forRoot(),
    FormsModule,
    ReactiveFormsModule
  ],
  declarations: [TaskListComponent]
})
export class TaskListModule { }
