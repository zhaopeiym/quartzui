import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SetingRoutingModule } from './seting-routing.module';
import { SetingComponent } from './seting/seting.component'; 
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgZorroAntdModule } from 'ng-zorro-antd'; 
@NgModule({
  imports: [
    CommonModule,
    SetingRoutingModule,
    NgZorroAntdModule,
    FormsModule,
    ReactiveFormsModule
  ],
  declarations: [SetingComponent]
})
export class SetingModule { }
