import { Component, OnInit, inject } from '@angular/core';
import { CompanyService } from '../../services/company.service';
import { MessageService } from 'primeng/api';
import { Company } from '../../core/interfaces/company.interface';

@Component({
  selector: 'app-company',
  templateUrl: './company.component.html',
  standalone: false
})
export class CompanyComponent implements OnInit {
  companyService = inject(CompanyService);
  messageService = inject(MessageService);

  companies: Company[] = [];

  // Dialog State
  displayCompanyDialog: boolean = false;
  companyForm: Partial<Company> = {};
  isEditingCompany: boolean = false;

  ngOnInit() {
    this.loadCompanies();
  }

  loadCompanies() {
    this.companyService.getCompanies().subscribe({
      next: (data) => this.companies = data,
      error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudieron cargar las empresas' })
    });
  }

  showAddCompanyDialog() {
    this.companyForm = { isActive: true };
    this.isEditingCompany = false;
    this.displayCompanyDialog = true;
  }

  showEditCompanyDialog(company: Company) {
    this.companyForm = { ...company };
    this.isEditingCompany = true;
    this.displayCompanyDialog = true;
  }

  saveCompany() {
    if (!this.companyForm.name || !this.companyForm.nit) {
      this.messageService.add({ severity: 'warn', summary: 'Validación', detail: 'Nombre y NIT son obligatorios' });
      return;
    }

    if (this.isEditingCompany) {
      this.companyService.updateCompany(this.companyForm.id!, this.companyForm as Company).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Empresa actualizada' });
          this.loadCompanies();
          this.displayCompanyDialog = false;
        },
        error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudo actualizar la empresa' })
      });
    } else {
      this.companyService.createCompany(this.companyForm as Company).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Empresa creada' });
          this.loadCompanies();
          this.displayCompanyDialog = false;
        },
        error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudo crear la empresa' })
      });
    }
  }

  deleteCompany(id: number) {
    if (confirm('¿Está seguro de que desea eliminar esta empresa?')) {
      this.companyService.deleteCompany(id).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Empresa eliminada' });
          this.loadCompanies();
        },
        error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudo eliminar la empresa' })
      });
    }
  }
}
