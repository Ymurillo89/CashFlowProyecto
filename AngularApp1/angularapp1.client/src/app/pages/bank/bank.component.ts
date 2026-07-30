import { Component, OnInit, inject } from '@angular/core';
import { BankService } from '../../services/bank.service';
import { MessageService } from 'primeng/api';
import { Bank } from '../../core/interfaces/bank.interface';

@Component({
  selector: 'app-bank',
  templateUrl: './bank.component.html',
  standalone: false
})
export class BankComponent implements OnInit {
  bankService = inject(BankService);
  messageService = inject(MessageService);

  banks: Bank[] = [];

  // Dialog State
  displayBankDialog: boolean = false;
  bankForm: Partial<Bank> = {};
  isEditingBank: boolean = false;

  ngOnInit() {
    this.loadBanks();
  }

  loadBanks() {
    this.bankService.getBanks().subscribe({
      next: (data) => this.banks = data,
      error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudieron cargar los bancos' })
    });
  }

  showAddBankDialog() {
    this.bankForm = {};
    this.isEditingBank = false;
    this.displayBankDialog = true;
  }

  showEditBankDialog(bank: Bank) {
    this.bankForm = { ...bank };
    this.isEditingBank = true;
    this.displayBankDialog = true;
  }

  saveBank() {
    if (!this.bankForm.name) {
      this.messageService.add({ severity: 'warn', summary: 'Validación', detail: 'El nombre del banco es obligatorio' });
      return;
    }

    if (this.isEditingBank) {
      this.bankService.updateBank(this.bankForm.id!, this.bankForm as Bank).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Banco actualizado' });
          this.loadBanks();
          this.displayBankDialog = false;
        },
        error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudo actualizar el banco' })
      });
    } else {
      this.bankService.createBank(this.bankForm as Bank).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Banco creado' });
          this.loadBanks();
          this.displayBankDialog = false;
        },
        error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudo crear el banco' })
      });
    }
  }

  deleteBank(id: number) {
    if (confirm('¿Está seguro de que desea eliminar este banco?')) {
      this.bankService.deleteBank(id).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Banco eliminado' });
          this.loadBanks();
        },
        error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudo eliminar el banco' })
      });
    }
  }
}
