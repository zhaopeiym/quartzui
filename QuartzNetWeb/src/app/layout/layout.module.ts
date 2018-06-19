import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LayoutRoutingModule } from './layout-routing.module'; 
import { LayoutComponent } from './layout/layout.component';
import { NgZorroAntdModule } from 'ng-zorro-antd';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  imports: [
    CommonModule,
    LayoutRoutingModule,
    NgZorroAntdModule.forRoot(),
    FormsModule,
    ReactiveFormsModule
  ],
  declarations: [LayoutComponent]
})
export class LayoutModule { }
