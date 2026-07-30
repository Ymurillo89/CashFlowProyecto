import { Component, OnInit, inject } from '@angular/core';
import { CompanyService } from '../../services/company.service';
import { StoreService } from '../../services/store.service';
import { BankService } from '../../services/bank.service';
import { ConsignationService } from '../../services/consignation.service';
import { MessageService } from 'primeng/api';
import { Company } from '../../core/interfaces/company.interface';
import { Store } from '../../core/interfaces/store.interface';
import { Bank } from '../../core/interfaces/bank.interface';
import { Consignation } from '../../core/interfaces/consignation.interface';

@Component({
  selector: 'app-cashier',
  templateUrl: './cashier.component.html',
  standalone: false
})
export class CashierComponent implements OnInit {
  companyService = inject(CompanyService);
  storeService = inject(StoreService);
  consignationService = inject(ConsignationService);
  bankService = inject(BankService);
  messageService = inject(MessageService);

  companies: Company[] = [];
  stores: Store[] = [];
  banks: Bank[] = [];
  consignations: Consignation[] = [];

  // Cashier Flow State
  selectedStoreId: number | null = null;
  declaredAmount: number | null = null;
  selectedBankId: number | null = null;
  declaredReference: string = '';
  uploadedFile: any = null;
  isScanning: boolean = false;
  scanProgress: number = 0;
  scanCompleted: boolean = false;

  // OCR Results (From backend)
  ocrBank: string = '';
  ocrReference: string = '';
  ocrAmount: number | null = null;
  ocrDate: string = '';
  ocrConfidence: number = 0;

  ngOnInit() {
    this.loadBackendData();
    this.loadConsignations();
  }

  loadBackendData() {
    this.companyService.getCompanies().subscribe({
      next: (data) => this.companies = data,
      error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error al cargar empresas' })
    });

    this.storeService.getStores().subscribe({
      next: (data) => this.stores = data,
      error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error al cargar sucursales' })
    });

    this.bankService.getBanks().subscribe({
      next: (data) => this.banks = data,
      error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error al cargar bancos' })
    });
  }

  loadConsignations() {
    this.consignationService.getPendingConsignations().subscribe({
      next: (data) => this.consignations = data,
      error: () => console.log('Error loading consignations')
    });
  }

  onUploadSelect(event: any) {
    if (event.files && event.files.length > 0) {
      this.uploadedFile = event.files[0];
    }
  }

  submitDeposit() {
    if (!this.selectedStoreId || !this.declaredAmount || !this.selectedBankId || !this.uploadedFile) {
      this.messageService.add({ severity: 'warn', summary: 'Error', detail: 'Complete los campos y suba el recibo' });
      return;
    }

    this.isScanning = true;
    this.scanProgress = 0;
    this.scanCompleted = false;

    const interval = setInterval(() => {
      if (this.scanProgress < 90) {
        this.scanProgress += 10;
      }
    }, 200);

    const formData = new FormData();
    formData.append('storeId', this.selectedStoreId.toString());
    formData.append('bankId', this.selectedBankId.toString());
    formData.append('referenceNumber', this.declaredReference);
    formData.append('declaredAmount', this.declaredAmount.toString());
    formData.append('consignationDate', new Date().toISOString());
    formData.append('consignationTime', '12:00:00'); // Dummy time
    formData.append('notes', 'Enviado desde caja');
    formData.append('file', this.uploadedFile);

    this.consignationService.submitConsignation(formData).subscribe({
      next: (res) => {
        clearInterval(interval);
        this.scanProgress = 100;
        
        // Fetch to show OCR results
        this.consignationService.getConsignationById(res.id).subscribe({
          next: (cons) => {
            this.isScanning = false;
            this.scanCompleted = true;
            if (cons.ocr) {
              this.ocrBank = cons.ocr.detectedBank;
              this.ocrReference = cons.ocr.detectedReference;
              this.ocrAmount = cons.ocr.detectedAmount || 0;
              this.ocrDate = cons.ocr.detectedDate || '';
              this.ocrConfidence = cons.ocr.confidence;
              
              const isMatch = this.declaredAmount === this.ocrAmount;
              this.messageService.add({
                severity: isMatch ? 'success' : 'warn',
                summary: 'Consignación Registrada',
                detail: isMatch ? 'Validación exitosa.' : '¡Discrepancia detectada!'
              });
            }
            this.loadConsignations();
          }
        });
      },
      error: () => {
        clearInterval(interval);
        this.isScanning = false;
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Fallo al procesar OCR' });
      }
    });
  }

  clearForm() {
    this.selectedStoreId = null;
    this.declaredAmount = null;
    this.selectedBankId = null;
    this.declaredReference = '';
    this.uploadedFile = null;
    this.scanCompleted = false;
  }
}
