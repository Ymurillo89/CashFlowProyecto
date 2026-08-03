import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Deposit } from '../core/interfaces/deposit.interface';

@Injectable({
  providedIn: 'root'
})
export class DepositService {
  private mockDeposits: Deposit[] = [];

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
