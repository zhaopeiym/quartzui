import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { NzMessageService } from 'ng-zorro-antd';
import { Util } from '../../shared/util';
import { MyHttpService } from '../../shared/myhttp';
import { en_US, zh_CN, NzI18nService } from 'ng-zorro-antd/i18n';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginPassword: any = "";
  private baseUrl = environment.baseUrl;
  IsEnglish: any;

  constructor(private router: Router,
    private i18n: NzI18nService,
    private translate: TranslateService,
    private message: NzMessageService,
    private http: MyHttpService, ) {

    if (JSON.parse(localStorage.getItem("IsEnglish")))
      this.translate.setDefaultLang("en");//zh        
    else
      this.translate.setDefaultLang("zh");//en
  }

  ngOnInit() {
  }

  login() {
    var url = this.baseUrl + "/api/Seting/VerifyLoginInfo";
    this.http.post(url, {
      password: this.bash64(this.loginPassword)
    }, (result: any) => {
      if (result.token) {
        this.router.navigate(['/']);
        Util.SetStorage("userInfo", result);
      }
      else
        this.message.warning("登录失败");
    }, (err) => {
    });
  }

  bash64(str) {
    return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g, function (match, p1) {
      return String.fromCharCode(parseInt('0x' + p1, 16));
    }));
  }
}
