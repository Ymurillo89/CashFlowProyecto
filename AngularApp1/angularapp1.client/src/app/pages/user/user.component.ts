import { Component, OnInit, inject } from '@angular/core';
import { UserService } from '../../services/user.service';
import { RoleService } from '../../services/role.service';
import { CompanyService } from '../../services/company.service';
import { StoreService } from '../../services/store.service';
import { MessageService } from 'primeng/api';
import { User } from '../../core/interfaces/user.interface';
import { Role } from '../../core/interfaces/role.interface';
import { Company } from '../../core/interfaces/company.interface';
import { Store } from '../../core/interfaces/store.interface';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  standalone: false
})
export class UserComponent implements OnInit {
  userService = inject(UserService);
  roleService = inject(RoleService);
  companyService = inject(CompanyService);
  storeService = inject(StoreService);
  messageService = inject(MessageService);

  users: User[] = [];
  roles: Role[] = [];
  companies: Company[] = [];
  stores: Store[] = [];
  filteredStores: Store[] = [];

  // Dialog State
  displayUserDialog: boolean = false;
  userForm: Partial<User> = {};
  isEditingUser: boolean = false;
  storeFilter: string = '';

  ngOnInit() {
    this.loadAllData();
  }

  loadAllData() {
    this.userService.getUsers().subscribe({
      next: (data) => this.users = data,
      error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudieron cargar los usuarios' })
    });

    this.roleService.getRoles().subscribe({
      next: (data) => this.roles = data,
      error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudieron cargar los roles' })
    });

    this.companyService.getCompanies().subscribe({
      next: (data) => this.companies = data,
      error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudieron cargar las empresas' })
    });

    this.storeService.getStores().subscribe({
      next: (data) => {
        this.stores = data;
        if (this.userForm.companyId) {
          this.onCompanyChange();
        }
      },
      error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudieron cargar los puntos de venta' })
    });
  }

  onCompanyChange() {
    if (this.userForm.companyId) {
      const compId = Number(this.userForm.companyId);
      this.filteredStores = this.stores.filter(s => s.companyId === compId);
      
      // If selected store doesn't belong to the new company, reset storeId to null
      if (this.userForm.storeId) {
        const storeExists = this.filteredStores.some(s => s.id === Number(this.userForm.storeId));
        if (!storeExists) {
          this.userForm.storeId = null;
        }
      }
    } else {
      this.filteredStores = [];
      this.userForm.storeId = null;
    }
  }

  get filteredAndSearchedStores(): Store[] {
    if (!this.storeFilter) {
      return this.filteredStores;
    }
    const filter = this.storeFilter.toLowerCase();
    return this.filteredStores.filter(s => 
      s.name.toLowerCase().includes(filter) || s.code.toLowerCase().includes(filter)
    );
  }

  showAddUserDialog() {
    this.userForm = { isActive: true, roleId: undefined, companyId: undefined, storeId: null, assignedStoreIds: [] };
    this.filteredStores = [];
    this.storeFilter = '';
    this.isEditingUser = false;
    this.displayUserDialog = true;
  }

  showEditUserDialog(user: User) {
    this.userForm = { ...user, assignedStoreIds: [...(user.assignedStoreIds || [])], password: '' };
    this.storeFilter = '';
    this.isEditingUser = true;
    this.displayUserDialog = true;
    this.onCompanyChange();
  }

  isStoreSelected(storeId: number): boolean {
    if (!this.userForm.assignedStoreIds) return false;
    return this.userForm.assignedStoreIds.includes(storeId);
  }

  toggleStoreSelection(storeId: number) {
    if (!this.userForm.assignedStoreIds) {
      this.userForm.assignedStoreIds = [];
    }
    const idx = this.userForm.assignedStoreIds.indexOf(storeId);
    if (idx > -1) {
      this.userForm.assignedStoreIds.splice(idx, 1);
    } else {
      this.userForm.assignedStoreIds.push(storeId);
    }
  }

  getAssignedStoreNames(user: User): string {
    if (!user.assignedStoreIds || user.assignedStoreIds.length === 0) {
      return user.storeName ? user.storeName : 'Ninguno (Admin Empresa)';
    }
    return user.assignedStoreIds
      .map(id => this.stores.find(s => s.id === id)?.name)
      .filter(name => !!name)
      .join(', ');
  }

  saveUser() {
    if (!this.userForm.fullName || !this.userForm.email || !this.userForm.roleId || !this.userForm.companyId) {
      this.messageService.add({ severity: 'warn', summary: 'Validación', detail: 'Nombre completo, Correo, Rol y Empresa son obligatorios.' });
      return;
    }

    if (!this.isEditingUser && !this.userForm.password) {
      this.messageService.add({ severity: 'warn', summary: 'Validación', detail: 'La contraseña es obligatoria para nuevos usuarios.' });
      return;
    }

    const selectedStores = this.userForm.assignedStoreIds || [];
    const roleIdNum = Number(this.userForm.roleId);
    
    let storeIdVal: number | null = null;
    let storeIdsList: number[] = [];

    if (roleIdNum === 2) {
      // Gerente Sucursal
      storeIdVal = selectedStores.length > 0 ? Number(selectedStores[0]) : null;
      storeIdsList = selectedStores.map(Number);
    } else if (roleIdNum === 3) {
      // Cajero
      storeIdVal = this.userForm.storeId ? Number(this.userForm.storeId) : null;
      storeIdsList = storeIdVal ? [storeIdVal] : [];
    }

    // Prepare payload
    const payload: User = {
      companyId: Number(this.userForm.companyId),
      storeId: storeIdVal,
      roleId: roleIdNum,
      fullName: this.userForm.fullName,
      email: this.userForm.email,
      password: this.userForm.password || undefined,
      isActive: !!this.userForm.isActive,
      assignedStoreIds: storeIdsList
    };

    if (this.isEditingUser) {
      this.userService.updateUser(this.userForm.id!, payload).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Usuario actualizado con éxito' });
          this.loadAllData();
          this.displayUserDialog = false;
        },
        error: (err) => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: err.error || 'No se pudo actualizar el usuario' })
      });
    } else {
      this.userService.createUser(payload).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Usuario creado con éxito' });
          this.loadAllData();
          this.displayUserDialog = false;
        },
        error: (err) => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: err.error || 'No se pudo crear el usuario' })
      });
    }
  }

  deleteUser(id: number) {
    if (confirm('¿Está seguro de que desea eliminar este usuario?')) {
      this.userService.deleteUser(id).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Usuario eliminado con éxito' });
          this.loadAllData();
        },
        error: () => this.messageService.add({ severity: 'error', summary: 'Error de API', detail: 'No se pudo eliminar el usuario' })
      });
    }
  }
}
