import { Component, Injectable, OnInit } from '@angular/core';
import { HttpClientModule, HttpClient, HttpHeaders } from '@angular/common/http';
import * as addDays from 'date-fns/add_days';
import * as getISOWeek from 'date-fns/get_iso_week';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgModule } from '@angular/core';
import { NzNotificationService, NzTreeModule, NzModalService } from 'ng-zorro-antd';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css']
})
export class LayoutComponent implements OnInit {
  validateJobForm: FormGroup;
  isJobVisible: boolean;
  isVisible: boolean;
  jobGroupName: any;
  jobGroupDescribe: any;
  isCron = true;
  modalTitle = "新增任务";
  title = 'app';
  private headers = new HttpHeaders({
    'Content-Type': 'application/json'
  });
  private baseUrl = environment.baseUrl;// "http://localhost:52725";   开发的时候可以先设置本地接口地址
  public resultData: any = [{}];
  dateFormat = 'yyyy/MM/dd';
  jobInfoEntity: any = {};

  constructor(private http: HttpClient,
    private fb2: FormBuilder,
    private notification: NzNotificationService,
    private modalService: NzModalService,
    private router: Router) {
  }

  ngOnInit(): void {

  }

  msgError(str): void {
    this.notification.error(str, "")
  }
  msgInfo(str): void {
    this.notification.info(str, "")
  }
  msgSuccess(str): void {
    this.notification.success(str, "")
  }
  msgWarning(str): void {
    this.notification.warning(str, "")
  }

  //跳转到任务列表
  jumpTaskList() {
    this.router.navigate(['/']);
  }

  //跳转到设置页面
  jumpSeting() {
    this.router.navigate(['/seting']);
  }

  //跳转到说明
  jumpExplain() {
    this.router.navigate(['/explain']);
  }
}
