import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SetingRoutingModule } from './seting-routing.module';
import { SetingComponent } from './seting/seting.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgZorroAntdModule } from 'ng-zorro-antd';

import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClient } from '@angular/common/http';
import { MyHttpService } from '../../shared/myhttp';
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

@NgModule({
  imports: [
    CommonModule,
    SetingRoutingModule,
    NgZorroAntdModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    })
  ],
  declarations: [SetingComponent],
  providers: [
    MyHttpService
  ],
})
export class SetingModule { }
