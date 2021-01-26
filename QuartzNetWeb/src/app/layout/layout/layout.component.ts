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
import { debounceTime } from 'rxjs/operators';

import { TranslateService } from '@ngx-translate/core';
import { fromEvent } from 'rxjs';
import { Util } from '../../../shared/util';

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
  private baseUrl = environment.baseUrl;// "http://localhost:52725";   开发的时候可以先设置本地接口地址
  public resultData: any = [{}];
  dateFormat = 'yyyy/MM/dd';
  jobInfoEntity: any = {};
  Language: string = "English";
  IsEnglish = false;
  TaskList: string;
  Seting: string;
  Explain: string;
  currentUrl: string;
  contentPaddingWidth: string = "0 40px";
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

    if (location.href.indexOf("/seting") >= 0) {
      this.currentUrl = "seting";
    } else if (location.href.indexOf("/explain") >= 0) {
      this.currentUrl = "explain";
    }
    else {
      this.currentUrl = "home";
    }

    this.IsEnglish = JSON.parse(localStorage.getItem("IsEnglish"));
    this.setSwitchLanguage();

    this.initWindowAdapt();
    fromEvent(window, 'resize').pipe(debounceTime(50)).subscribe(() => {
      this.initWindowAdapt();
    });
  }
  initWindowAdapt() {
    var w = document.documentElement.offsetWidth || document.body.offsetWidth;
    if (w < 1500)
      this.contentPaddingWidth = "0 10px";
    else
      this.contentPaddingWidth = "0 40px";
    console.log(w);
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

  clickGitHub() {
    window.open("https://github.com/zhaopeiym/quartzui");
  }

  // 切换语言
  switchLanguage() {
    this.IsEnglish = !this.IsEnglish;
    this.setSwitchLanguage();
  }

  setSwitchLanguage() {
    if (this.IsEnglish) {
      this.i18n.setLocale(en_US);
      this.showEnglish();
      this.Language = "中文";
    }
    else {
      this.i18n.setLocale(zh_CN);
      this.showChinese();
      this.Language = "English";
    }

    localStorage.setItem("IsEnglish", JSON.stringify(this.IsEnglish));
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

  logout() {
    Util.SetStorage("userInfo", {});
    this.router.navigate(['/signin']);
  }
}
