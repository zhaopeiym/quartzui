import { Injectable } from '@angular/core';
import { HttpClientModule, HttpClient, HttpHeaders } from '@angular/common/http';
import { Util } from './util';
import { Router } from '@angular/router';

@Injectable()
export class MyHttpService {



    constructor(private http: HttpClient,
        private router: Router, ) {

    }

    post(url: string, body: any, succeed?: Function, error?: Function) {
        var headers = new HttpHeaders({
            'Content-Type': 'application/json',
            "token": Util.GetStorage("userInfo").token || ""
        });
        return this.http.post(url, body, { headers: headers })
            .subscribe((result: any) => {
                succeed && succeed(result);
            }, (err) => {
                error && error(err);
                if (err.status == 401 && err.error && err.error.resultUrl)
                    this.router.navigate([err.error.resultUrl]);
            }, () => { });
    }


    get(url: string, succeed?: Function, error?: Function) {
        var headers = new HttpHeaders({
            'Content-Type': 'application/json',
            "token": Util.GetStorage("userInfo").token || ""
        });
        return this.http.get(url, { headers: headers })
            .subscribe((result: any) => {
                succeed && succeed(result);
            }, (err) => {
                error && error(err);
                if (err.status == 401 && err.error && err.error.resultUrl)
                    this.router.navigate([err.error.resultUrl]);
            }, () => { });
    }
}