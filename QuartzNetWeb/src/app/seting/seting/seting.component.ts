import { Component, OnInit } from '@angular/core';

import { HttpClientModule, HttpClient, HttpHeaders } from '@angular/common/http';
import * as addDays from 'date-fns/add_days';
import * as getISOWeek from 'date-fns/get_iso_week';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgModule } from '@angular/core';
import { NzNotificationService, NzTreeModule, NzModalService } from 'ng-zorro-antd';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-seting',
  templateUrl: './seting.component.html',
  styleUrls: ['./seting.component.css']
})
export class SetingComponent implements OnInit {
  validateForm: FormGroup;
  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.validateForm = this.fb.group({});
  }

}
