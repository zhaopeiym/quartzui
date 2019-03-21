import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ExplainRoutingModule } from './explain-routing.module';
import { ExplainComponent } from './explain/explain.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgZorroAntdModule } from 'ng-zorro-antd'; 

@NgModule({
  imports: [
    CommonModule,
    ExplainRoutingModule,
    NgZorroAntdModule.forRoot(),
    FormsModule,
    ReactiveFormsModule
  ],
  declarations: [ExplainComponent]
})
export class ExplainModule { }
