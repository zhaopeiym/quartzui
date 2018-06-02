
import { Component, Injectable, OnInit } from '@angular/core';
import { HttpClientModule, HttpClient, HttpHeaders } from '@angular/common/http';
import * as addDays from 'date-fns/add_days';
import * as getISOWeek from 'date-fns/get_iso_week';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgModule } from '@angular/core';
import { environment } from '../environments/environment';
import { NzNotificationService } from 'ng-zorro-antd';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  validateJobForm: FormGroup;
  isJobVisible: boolean;
  isVisible: boolean;
  jobGroupName: any;
  jobGroupDescribe: any;
  modalTitle = "新增任务";
  title = 'app';
  private headers = new HttpHeaders({
    'Content-Type': 'application/json'
  });
  private baseUrl = environment.baseUrl;// "http://localhost:52725";   开发的时候可以先设置本地接口地址
  public resultData = [{}];
  dateFormat = 'yyyy/MM/dd';
  jobInfoEntity: any = {};

  constructor(private http: HttpClient, private fb2: FormBuilder, private notification: NzNotificationService) {
    this.loadJobInfo();
  }

  ngOnInit(): void {
    this.validateJobForm = this.fb2.group({
      jobName: [null, [Validators.required]],
      beginTime: [null, [Validators.required]],
      endTime: [],
      cron: [null, [Validators.required]],
      requestUrl: [null, [Validators.required]],
      requestType: [null, [Validators.required]],
      requestParameters: [],
      description: [],
      jobGroup: [null, [Validators.required]],
    });
  }

  loadJobInfo() {
    var url = this.baseUrl + "/api/Job/GetAllJob";
    this.http.get(url, { headers: this.headers })
      .subscribe((result: any) => {
        this.resultData = result;
        console.log(result);
      }, (err) => {

      }, () => {

      });

  }

  showJobModal(groupName) {
    this.isJobVisible = true;
    this.jobInfoEntity.jobGroup = groupName;
  }

  handleJobCancel() {
    this.isJobVisible = false;
    this.modalTitle = "新增任务";
  }

  validata() {
    for (const i in this.validateJobForm.controls) {
      this.validateJobForm.controls[i].markAsDirty();
      this.validateJobForm.controls[i].updateValueAndValidity();
    }
  }

  //新增/编辑 计划任务
  handleJobOk() {
    if (!this.validateJobForm.valid) {
      this.validata();
      return;
    }
    console.log(this.jobInfoEntity);
    var url = this.baseUrl + "/api/Job/AddJob";
    if (this.modalTitle === "编辑任务")
      url = this.baseUrl + "/api/Job/ModifyJob";
    this.http.post(url, this.jobInfoEntity, { headers: this.headers })
      .subscribe((result: any) => {
        console.log(result);
      }, (err) => {

      }, () => {
        this.loadJobInfo();
      });

    this.isJobVisible = false;
  }

  onChange(result: Date): void {
    console.log('onChange: ', result);
  }

  //编辑任务
  editJob(name, group) {
    this.modalTitle = "编辑任务";
    var url = this.baseUrl + "/api/Job/QueryJob";
    this.http.post(url, { name: name, group: group }, { headers: this.headers })
      .subscribe((result: any) => {
        this.jobInfoEntity = result;
        this.jobInfoEntity.requestType = this.jobInfoEntity.requestType.toString();
        this.isJobVisible = true;
      }, (err) => {

      }, () => {
        //this.loadJobInfo();
      });
  }

  /*  //修改
   modifyJob() {
     if (!this.validateJobForm.valid) {
       this.validata();
       return;
     }
     console.log(this.jobInfoEntity);
     var url = this.baseUrl + "/api/Job/ModifyJob";
     this.http.post(url, this.jobInfoEntity, { headers: this.headers })
       .subscribe((result: any) => {
         console.log(result);
       }, (err) => {
 
       }, () => {
         this.loadJobInfo();
       });
 
     this.isJobVisible = false;
   } */

  //暂停
  stopJob(name, group) {
    var url = this.baseUrl + "/api/Job/StopJob";
    this.http.post(url, { name: name, group: group }, { headers: this.headers })
      .subscribe((result: any) => {
        this.msgInfo(result.msg);
      }, (err) => {

      }, () => {
        this.loadJobInfo();
      });
  }

  //删除
  removeJob(name, group) {
    var url = this.baseUrl + "/api/Job/RemoveJob";
    this.http.post(url, { name: name, group: group }, { headers: this.headers })
      .subscribe((result: any) => {
        this.msgInfo(result.msg);
      }, (err) => {

      }, () => {
        this.loadJobInfo();
      });
  }

  //恢复
  resumeJob(name, group) {
    var url = this.baseUrl + "/api/Job/ResumeJob";
    this.http.post(url, { name: name, group: group }, { headers: this.headers })
      .subscribe((result: any) => {
        this.msgInfo(result.msg);
      }, (err) => {

      }, () => {
        this.loadJobInfo();
      });
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
}
