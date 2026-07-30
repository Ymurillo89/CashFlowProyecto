import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Consignation, AuditConsignation } from '../core/interfaces/consignation.interface';

@Injectable({
  providedIn: 'root'
})
export class ConsignationService {
  private http = inject(HttpClient);
  private apiUrl = '/api/consignation';

  getPendingConsignations(): Observable<Consignation[]> {
    return this.http.get<Consignation[]>(`${this.apiUrl}/pending`);
  }

  getConsignationById(id: number): Observable<Consignation> {
    return this.http.get<Consignation>(`${this.apiUrl}/${id}`);
  }

  submitConsignation(formData: FormData): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.apiUrl, formData);
  }

  auditConsignation(id: number, auditData: AuditConsignation): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/audit/${id}`, auditData);
  }
}
