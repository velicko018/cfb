import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class JwtTokenService {
    private ROLE = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
    private NAME_IDENTIFIER = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
    

    getRole(token) {
        let jsonResult = this.parseToken(token);

        return jsonResult[this.ROLE];
    }

    getUserId(token) {
        let jsonResult = this.parseToken(token);

        return jsonResult[this.NAME_IDENTIFIER];
    }

    private parseToken(token) {
        let base64Url = token.split('.')[1];
        let base64 = base64Url
            .replace(/-/g, '+')
            .replace(/_/g, '/');
        let jsonPayload = decodeURIComponent(atob(base64)
            .split('')
            .map(c => `%${('00' + c.charCodeAt(0).toString(16)).slice(-2)}`)
            .join(''));

        return JSON.parse(jsonPayload);
    }
}
