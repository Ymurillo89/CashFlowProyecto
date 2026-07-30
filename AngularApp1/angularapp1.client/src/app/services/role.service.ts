import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Role } from '../core/interfaces/role.interface';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private http = inject(HttpClient);
  private apiUrl = '/api/role';

  getRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(this.apiUrl);
  }
}
