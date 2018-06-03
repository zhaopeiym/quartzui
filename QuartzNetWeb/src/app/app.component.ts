
import { Component, Injectable, OnInit } from '@angular/core';
import { HttpClientModule, HttpClient, HttpHeaders } from '@angular/common/http';
import * as addDays from 'date-fns/add_days';
import * as getISOWeek from 'date-fns/get_iso_week';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgModule } from '@angular/core';
import { environment } from '../environments/environment';
import { NzNotificationService, NzTreeModule, NzModalService } from 'ng-zorro-antd';

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
    private modalService: NzModalService) {
    this.loadJobInfo();
    setInterval(() => {//定时刷新
      this.renovateJobInfo();
    }, 1000 * 5);
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
      triggerType: ['1', [Validators.required]],
      intervalSecond: [],
      intervalUnit: ['1'],
    });
  }

  //加载
  loadJobInfo(isReset?) {
    var url = this.baseUrl + "/api/Job/GetAllJob";
    this.http.get(url, { headers: this.headers })
      .subscribe((result: any) => {
        result.forEach(element => {
          element.active = localStorage.getItem(element.groupName);
        });
        this.resultData = result;
      }, (err) => {

      }, () => {
        if (isReset !== false)
          this.formReset();
      });
  }

  //刷新
  renovateJobInfo() {    
    var url = this.baseUrl + "/api/Job/GetAllJobBriefInfo";
    this.http.get(url, { headers: this.headers })
      .subscribe((result: any) => {
        this.resultData.forEach(element => {
          var jobs = result.find(t => t.groupName === element.groupName);
          element.jobInfoList.forEach(eleJob => {
            var jobs = result.find(t => t.groupName === element.groupName).jobInfoList;
            var job = jobs.find(t => t.name === eleJob.name);
            //更新部分数据
            eleJob.previousFireTime = job.previousFireTime;
            eleJob.nextFireTime = job.nextFireTime;
            eleJob.lastErrMsg = job.lastErrMsg;
            eleJob.displayState = job.displayState;
          });
        });
      });
  }

  showJobModal(groupName) {
    this.isJobVisible = true;
    this.jobInfoEntity.jobGroup = groupName;
  }

  //取消
  handleJobCancel() {
    this.isJobVisible = false;
    this.modalTitle = "新增任务";
    this.formReset();
  }

  //验证
  validata() {
    for (const i in this.validateJobForm.controls) {
      this.validateJobForm.controls[i].markAsDirty();
      this.validateJobForm.controls[i].updateValueAndValidity();
    }
  }

  //重置
  formReset() {
    this.validateJobForm.reset();
    this.validateJobForm.controls["triggerType"].setValue("1");
    this.validateJobForm.controls["intervalUnit"].setValue("1");
  }

  //新增-编辑 计划任务
  handleJobOk() {
    /*  e.stopPropagation();
     e.preventDefault(); */

    if (!this.validateJobForm.valid) {
      this.validata();
      if (!this.validateJobForm.valid)
        return;
    }
    this.jobInfoEntity.intervalSecond = this.jobInfoEntity.intervalSecond * parseInt(this.validateJobForm.controls["intervalUnit"].value);
    var url = this.baseUrl + "/api/Job/AddJob";
    if (this.modalTitle === "编辑任务")
      url = this.baseUrl + "/api/Job/ModifyJob";
    this.http.post(url, this.jobInfoEntity, { headers: this.headers })
      .subscribe((result: any) => {
        this.msgInfo("保存任务计划成功！");
      }, (err) => {
        this.msgError("保存任务计划失败！");
      }, () => {
        this.loadJobInfo();
      });

    this.isJobVisible = false;
  }

  //编辑任务
  editJob(name, group) {

    this.modalTitle = "编辑任务";
    var url = this.baseUrl + "/api/Job/QueryJob";
    this.http.post(url, { name: name, group: group }, { headers: this.headers })
      .subscribe((result: any) => {
        this.jobInfoEntity = result;
        this.jobInfoEntity.requestType = this.jobInfoEntity.requestType.toString();
        this.jobInfoEntity.triggerType = this.jobInfoEntity.triggerType.toString();
        this.isJobVisible = true;
      }, (err) => {

      }, () => {

      });
  }

  //暂停
  stopJob(name, group) { 
    var url = this.baseUrl + "/api/Job/StopJob";
    this.http.post(url, { name: name, group: group }, { headers: this.headers })
      .subscribe((result: any) => {
        this.msgInfo(result.msg);
      }, (err) => {

      }, () => {
        this.renovateJobInfo();
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
        this.renovateJobInfo();
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

  //修改触发器类型时
  changeTriggerType(triggerType) {
    if (triggerType === "1") {//cron
      this.isCron = true;
      this.validateJobForm.controls["intervalSecond"].setValidators(null);
      this.validateJobForm.controls["cron"].setValidators(Validators.required);
    }
    else if (triggerType === "2") {//Simple
      this.isCron = false;
      this.validateJobForm.controls["cron"].setValidators(null);
      this.validateJobForm.controls["intervalSecond"].setValidators(Validators.required);
    }
  }

  //立即执行
  triggerJob(name, group) {
    var url = this.baseUrl + "/api/Job/TriggerJob";
    this.http.post(url, { name: name, group: group }, { headers: this.headers })
      .subscribe((result: any) => {
        this.msgInfo("执行成功！");
      }, (err) => {
        this.msgError("执行失败！");
      }, () => {
        this.renovateJobInfo();
      });
  }

  //查看日志
  getJobLogs(name, group) {
    var url = this.baseUrl + "/api/Job/GetJobLogs";
    this.http.post(url, { name: name, group: group }, { headers: this.headers })
      .subscribe((result: any) => {
        var logs = "";
        result.forEach(element => {
          logs += "<p>" + element + "</p>"
        });
        this.modalService.create({
          nzTitle: '日志',
          nzContent: logs,
          nzFooter: null,
          nzBodyStyle: {
            "max-height": '500px',
            "overflow-y": "auto"
          }
        });
      }, (err) => {
        this.msgError("查询失败！");
      });
  }

  //切换 折叠任务组
  clickPanel(active, groupName) {
    localStorage.setItem(groupName, active);
  }

  showModalMsg(title, content) {
    this.modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null
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
