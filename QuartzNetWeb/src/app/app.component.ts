
import { Component, Injectable, OnInit } from '@angular/core';
import { HttpClientModule, HttpClient, HttpHeaders } from '@angular/common/http';
import * as addDays from 'date-fns/add_days';
import * as getISOWeek from 'date-fns/get_iso_week';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgModule } from '@angular/core';

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
  title = 'app';
  private headers = new HttpHeaders({
    'Content-Type': 'application/json'
  });
  private baseUrl = "";// "http://localhost:52725";   开发的时候可以先设置本地接口地址
  public resultData = [{}];
  dateFormat = 'yyyy/MM/dd';
  jobInfoEntity: any = {};

  constructor(private http: HttpClient, private fb2: FormBuilder) {
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

  handleCancel(): void {

    this.isVisible = false;
  }

  showJobModal(groupName) {
    this.isJobVisible = true;
    this.jobInfoEntity.jobGroup = groupName;
  }

  handleJobCancel() {

    this.isJobVisible = false;
  }

  validata() {
    for (const i in this.validateJobForm.controls) {
      this.validateJobForm.controls[i].markAsDirty();
      this.validateJobForm.controls[i].updateValueAndValidity();
    }
  }

  //新增计划任务
  handleJobOk() {

    if (!this.validateJobForm.valid) {
      this.validata();
      return;
    }

    console.log(this.jobInfoEntity);
    var url = this.baseUrl + "/api/Job/AddJob";
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

  //暂停
  stopJob(name, group) {
    var url = this.baseUrl + "/api/Job/StopJob";
    this.http.post(url, { name: name, group: group }, { headers: this.headers })
      .subscribe((result: any) => {
        alert(result.msg);
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
        alert(result.msg);
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
        alert(result.msg);
      }, (err) => {

      }, () => {
        this.loadJobInfo();
      });
  }

  showMessg(msg) {
    alert(msg);
  }
}
