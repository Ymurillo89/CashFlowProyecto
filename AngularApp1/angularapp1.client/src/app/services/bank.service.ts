import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Bank } from '../core/interfaces/bank.interface';

@Injectable({
  providedIn: 'root'
})
export class BankService {
  private http = inject(HttpClient);
  private apiUrl = '/api/bank';

  getBanks(): Observable<Bank[]> {
    return this.http.get<Bank[]>(this.apiUrl);
  }

  getBank(id: number): Observable<Bank> {
    return this.http.get<Bank>(`${this.apiUrl}/${id}`);
  }

  createBank(bank: Bank): Observable<any> {
    return this.http.post<any>(this.apiUrl, bank);
  }

  updateBank(id: number, bank: Bank): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, bank);
  }

  deleteBank(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}
