import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Deposit } from '../core/interfaces/deposit.interface';

@Injectable({
  providedIn: 'root'
})
export class DepositService {
  private mockDeposits: Deposit[] = [
    {
      id: '101',
      storeId: 1,
      storeName: 'North Station POS',
      bankName: 'Bancolombia',
      declaredAmount: 150000,
      ocrAmount: 150000,
      ocrBank: 'Bancolombia',
      reference: 'TX-987452',
      date: '2026-07-29',
      status: 'Matched'
    },
    {
      id: '102',
      storeId: 2,
      storeName: 'South Plaza Mall',
      bankName: 'Davivienda',
      declaredAmount: 320000,
      ocrAmount: 300000,
      ocrBank: 'Davivienda',
      reference: 'REF-77412',
      date: '2026-07-30',
      status: 'Discrepancy'
    }
  ];

  private deposits$ = new BehaviorSubject<Deposit[]>(this.mockDeposits);

  getDeposits(): Observable<Deposit[]> {
    return this.deposits$.asObservable();
  }

  addDeposit(deposit: Deposit) {
    const current = this.deposits$.value;
    this.deposits$.next([deposit, ...current]);
  }

  updateDepositStatus(id: string, status: 'Matched' | 'Error', notes?: string) {
    const current = this.deposits$.value;
    const updated = current.map(d => {
      if (d.id === id) {
        return { ...d, status, notes };
      }
      return d;
    });
    this.deposits$.next(updated);
  }
}
