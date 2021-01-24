export class Util {
    //保存
    static SetStorage(key, value) {
        localStorage.setItem(key, JSON.stringify(value));
    }

    //获取
    static GetStorage(key) {        
        var value = localStorage.getItem(key);
        if (value == null || value === undefined || value === "undefined")
            return {};
        return JSON.parse(value);
    }
}