import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SetingRoutingModule } from './seting-routing.module';
import { SetingComponent } from './seting/seting.component';

@NgModule({
  imports: [
    CommonModule,
    SetingRoutingModule
  ],
  declarations: [SetingComponent]
})
export class SetingModule { }
