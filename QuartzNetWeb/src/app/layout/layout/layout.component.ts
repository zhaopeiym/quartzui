import { Component, Injectable, OnInit } from '@angular/core';
import { HttpClientModule, HttpClient, HttpHeaders } from '@angular/common/http';
import * as addDays from 'date-fns/add_days';
import * as getISOWeek from 'date-fns/get_iso_week';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgModule } from '@angular/core';
import { NzNotificationService, NzTreeModule, NzModalService } from 'ng-zorro-antd';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';
import { en_US, zh_CN, NzI18nService } from 'ng-zorro-antd/i18n';

import { TranslateService } from '@ngx-translate/core';

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
  // logTitle = "任务调度平台";
  private headers = new HttpHeaders({
    'Content-Type': 'application/json'
  });
  private baseUrl = environment.baseUrl;// "http://localhost:52725";   开发的时候可以先设置本地接口地址
  public resultData: any = [{}];
  dateFormat = 'yyyy/MM/dd';
  jobInfoEntity: any = {};
  Language: string = "English";
  IsEnglish = false;
  TaskList: string;
  Seting: string;
  Explain: string;
  constructor(private http: HttpClient,
    private fb2: FormBuilder,
    private notification: NzNotificationService,
    private modalService: NzModalService,
    private i18n: NzI18nService,
    //https://blog.csdn.net/qq_36822018/article/details/81504473
    //https://segmentfault.com/a/1190000019852382
    private translate: TranslateService,
    private router: Router) {
      this.translate.setDefaultLang("zh");
  }

  ngOnInit(): void {
    if (this.IsEnglish)
      this.showEnglish();
    else
      this.showChinese();
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

  // 切换语言
  switchLanguage() {
    if (!this.IsEnglish) {
      this.Language = "中文";
      this.i18n.setLocale(en_US);
      this.showEnglish();
    }
    else {
      this.Language = "English";
      this.i18n.setLocale(zh_CN);
      this.showChinese();
    }
    this.IsEnglish = !this.IsEnglish;
  }

  showEnglish() {
    //this.logTitle = "Task scheduling platform";
    this.TaskList = "task list";
    this.Seting = "system setup";
    this.Explain = "direction for use";

    this.translate.use("en");
  }

  showChinese() {
    // this.logTitle = "任务调度平台";
    this.TaskList = "任务列表";
    this.Seting = "系统设置";
    this.Explain = "使用说明"; 
    this.translate.use("zh");
  }
}
