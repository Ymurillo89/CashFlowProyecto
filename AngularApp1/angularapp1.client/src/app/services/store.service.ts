import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Store {
  id: number;
  companyId: number;
  code: string;
  name: string;
  city: string;
  address: string;
  managerName: string;
  managerPhone: string;
  isActive: boolean;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class StoreService {
  private apiUrl = '/api/store';

  constructor(private http: HttpClient) { }

  getStores(): Observable<Store[]> {
    return this.http.get<Store[]>(this.apiUrl);
  }

  getStore(id: number): Observable<Store> {
    return this.http.get<Store>(`${this.apiUrl}/${id}`);
  }

  createStore(store: Store): Observable<Store> {
    return this.http.post<Store>(this.apiUrl, store);
  }

  updateStore(id: number, store: Store): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, store);
  }

  deleteStore(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
