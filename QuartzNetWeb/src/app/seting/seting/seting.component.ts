import { Component, OnInit } from '@angular/core';

import { HttpClientModule, HttpClient, HttpHeaders } from '@angular/common/http';
import * as addDays from 'date-fns/add_days';
import * as getISOWeek from 'date-fns/get_iso_week';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgModule } from '@angular/core';
import { NzNotificationService, NzTreeModule, NzModalService, NzMessageService } from 'ng-zorro-antd';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';
import { Util } from '../../../shared/util';
import { MyHttpService } from '../../../shared/myhttp';

@Component({
  selector: 'app-seting',
  templateUrl: './seting.component.html',
  styleUrls: ['./seting.component.css']
})
export class SetingComponent implements OnInit {
  validateForm: FormGroup;
  refreshValue = 10;
  private baseUrl = environment.baseUrl;
  oldPassword: any;
  newPassword: any;
  sendMailLoading: boolean;
  mqttValidateForm: FormGroup;
  rabbitValidateForm: FormGroup;
  rabbitbut: string = "确定";
  rabbitbutLoading: boolean;

  constructor(private fb: FormBuilder,
    private router: Router,
    private http: MyHttpService,
    private message: NzMessageService) { }

  ngOnInit() {
    this.getRefreshInterval();
    this.validateForm = this.fb.group({
      mailFrom: [null, [Validators.required]],
      mailPwd: [null, [Validators.required]],
      mailHost: [null, [Validators.required]],
      mailTo: [null, [Validators.required]],
    });

    this.mqttValidateForm = this.fb.group({
      host: [null, [Validators.required]],
      port: [null, [Validators.required]],
      clientId: [null, [Validators.required]],
      userName: [null],
      password: [null],
      connectionMethod: ['4']
    });

    this.rabbitValidateForm = this.fb.group({
      rabbitHost: [null, [Validators.required]],
      rabbitPort: [null, [Validators.required]],
      rabbitUserName: [null],
      rabbitPassword: [null]
    });

    this.getMailInfo();
    this.getMqttSet();
    this.getRabbitSet();
  }

  //获取邮箱信息
  getMailInfo() {
    var url = this.baseUrl + "/api/Seting/GetMailInfo";
    this.http.post(url, {}, (result: any) => {
      this.validateForm.controls.mailFrom.setValue(result.mailFrom);
      this.validateForm.controls.mailPwd.setValue(result.mailPwd);
      this.validateForm.controls.mailHost.setValue(result.mailHost);
      this.validateForm.controls.mailTo.setValue(result.mailTo);
    }, (err) => {

    });
  }

  //更新表单对有效值的验证
  updateValue(): void {
    for (const i in this.validateForm.controls) {
      this.validateForm.controls[i].markAsDirty();
      this.validateForm.controls[i].updateValueAndValidity();
    }
  }

  handleOk() {
    this.updateValue();
    if (!this.validateForm.valid)
      return;

    var url = this.baseUrl + "/api/Seting/SaveMailInfo";
    this.http.post(url, this.validateForm.value, (result: any) => {
      this.message.success("保存成功！");
    }, (err) => {
      this.message.warning("保存失败！");
    });
  }

  saveMqttSet() {
    for (const i in this.mqttValidateForm.controls) {
      this.mqttValidateForm.controls[i].markAsDirty();
      this.mqttValidateForm.controls[i].updateValueAndValidity();
    }
    if (!this.mqttValidateForm.valid)
      return;
    var url = this.baseUrl + "/api/Seting/SaveMqttSet";
    this.http.post(url, this.mqttValidateForm.value, (result: any) => {
      this.message.success("保存成功！");
    }, (err) => {
      this.message.warning("保存失败！");
    });
  }

  getMqttSet() {
    var url = this.baseUrl + "/api/Seting/GetMqttSet";
    this.http.post(url, {}, (result: any) => {
      this.mqttValidateForm.controls.host.setValue(result.host);
      this.mqttValidateForm.controls.port.setValue(result.port);
      this.mqttValidateForm.controls.clientId.setValue(result.clientId);
      this.mqttValidateForm.controls.userName.setValue(result.userName);
      this.mqttValidateForm.controls.password.setValue(result.password);
      this.mqttValidateForm.controls.connectionMethod.setValue(result.connectionMethod.toString());
    }, (err) => {

    });
  }

  saveRabbitSet() {
    for (const i in this.rabbitValidateForm.controls) {
      this.rabbitValidateForm.controls[i].markAsDirty();
      this.rabbitValidateForm.controls[i].updateValueAndValidity();
    }
    if (!this.rabbitValidateForm.valid)
      return;
    var url = this.baseUrl + "/api/Seting/SaveRabbitSet";
    this.http.post(url, this.rabbitValidateForm.value, (result: any) => {
      this.message.success("保存成功！");
      this.restartRabbit();
    }, (err) => {
      this.message.warning("保存失败！");
    });
  }

  restartRabbit() {
    this.rabbitbut = "连接测试";
    this.rabbitbutLoading = true;
    var url = this.baseUrl + "/api/Seting/RestartRabbit";
    this.http.post(url, this.rabbitValidateForm.value, (result: any) => {
      this.rabbitbut = "确定";
      this.rabbitbutLoading = false;
      if (result)
        this.message.success("RestartMQ连接成功");
      else
        this.message.warning("RestartMQ连接失败！");
    }, (err) => {
      this.rabbitbut = "确定";
      this.rabbitbutLoading = false;
      this.message.warning("RestartMQ连接失败！");
    });
  }

  getRabbitSet() {
    var url = this.baseUrl + "/api/Seting/GetRabbitSet";
    this.http.post(url, {}, (result: any) => {
      this.rabbitValidateForm.controls.rabbitHost.setValue(result.rabbitHost);
      this.rabbitValidateForm.controls.rabbitPort.setValue(result.rabbitPort);
      this.rabbitValidateForm.controls.rabbitUserName.setValue(result.rabbitUserName);
      this.rabbitValidateForm.controls.rabbitPassword.setValue(result.rabbitPassword);
    }, (err) => {

    });
  }

  testMail() {
    this.sendMailLoading = true;
    var url = this.baseUrl + "/api/Seting/SendMail";
    this.http.post(url, { Title: "任务调度测试邮件", Content: "任务调度测试邮件发送成功！", MailInfo: this.validateForm.value }, (result: any) => {
      if (result)
        this.message.success("发送成功，请在邮箱查收！");
      else
        this.message.warning("发送失败！");
      this.sendMailLoading = false;
    }, (err) => {
      this.message.error("发送失败！");
      this.sendMailLoading = false;
    });
  }

  saverefresh() {
    var url = this.baseUrl + "/api/Seting/SaveRefreshInterval";
    this.http.post(url, { IntervalTime: this.refreshValue }, (result: any) => {
      this.message.success("保存成功");
    }, (err) => {
      this.message.error("保存失败");
    });
  }

  getRefreshInterval() {
    var url = this.baseUrl + "/api/Seting/GetRefreshInterval";
    this.http.post(url, {}, (result: any) => {
      this.refreshValue = result.intervalTime;
    }, (err) => {

    });
  }

  seaveLogin() {
    var url = this.baseUrl + "/api/Seting/SaveLoginInfo";
    this.http.post(url, {
      oldPassword: this.oldPassword,
      newPassword: this.newPassword,
    }, (result: any) => {
      if (result) {
        this.message.success("保存成功");
        this.oldPassword = "";
        this.newPassword = "";
        Util.SetStorage("userInfo", {});
        setTimeout(() => {
          this.router.navigate(['/signin']);
        }, 700);
      }
      else
        this.message.warning("保存失败，旧密码不对。");
    }, (err) => {

    });
  }
}
