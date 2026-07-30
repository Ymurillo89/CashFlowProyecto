import { Component, OnInit, inject } from '@angular/core';
import { CompanyService } from '../../services/company.service';
import { StoreService } from '../../services/store.service';
import { MessageService } from 'primeng/api';
import { Company } from '../../core/interfaces/company.interface';
import { Store } from '../../core/interfaces/store.interface';

@Component({
  selector: 'app-store',
  templateUrl: './store.component.html',
  standalone: false
})
export class StoreComponent implements OnInit {
  companyService = inject(CompanyService);
  storeService = inject(StoreService);
  messageService = inject(MessageService);

  companies: Company[] = [];
  stores: Store[] = [];

  // Dialog State
  displayStoreDialog: boolean = false;
  storeForm: Partial<Store> = {};
  isEditingStore: boolean = false;

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.companyService.getCompanies().subscribe({
      next: (data) => this.companies = data,
      error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudieron cargar las empresas' })
    });

    this.storeService.getStores().subscribe({
      next: (data) => this.stores = data,
      error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudieron cargar las sucursales' })
    });
  }

  showAddStoreDialog() {
    this.storeForm = { isActive: true };
    this.isEditingStore = false;
    this.displayStoreDialog = true;
  }

  showEditStoreDialog(store: Store) {
    this.storeForm = { ...store };
    this.isEditingStore = true;
    this.displayStoreDialog = true;
  }

  saveStore() {
    if (!this.storeForm.name || !this.storeForm.code || !this.storeForm.companyId) {
      this.messageService.add({ severity: 'warn', summary: 'Validación', detail: 'Nombre, Código y Empresa son obligatorios' });
      return;
    }

    // Force CompanyId to be numeric
    this.storeForm.companyId = Number(this.storeForm.companyId);

    if (this.isEditingStore) {
      this.storeService.updateStore(this.storeForm.id!, this.storeForm as Store).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Punto de venta actualizado' });
          this.loadData();
          this.displayStoreDialog = false;
        },
        error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudo actualizar el punto de venta' })
      });
    } else {
      this.storeService.createStore(this.storeForm as Store).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Punto de venta creado' });
          this.loadData();
          this.displayStoreDialog = false;
        },
        error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudo crear el punto de venta' })
      });
    }
  }

  deleteStore(id: number) {
    if (confirm('¿Está seguro de que desea eliminar este punto de venta?')) {
      this.storeService.deleteStore(id).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Punto de venta eliminado' });
          this.loadData();
        },
        error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudo eliminar el punto de venta' })
      });
    }
  }

  getCompanyName(companyId: number): string {
    const comp = this.companies.find(c => c.id === companyId);
    return comp ? comp.name : 'Desconocido';
  }
}
