import { Component, OnInit, inject } from '@angular/core';
import { ConsignationService } from '../../services/consignation.service';
import { MessageService } from 'primeng/api';
import { Consignation } from '../../core/interfaces/consignation.interface';

@Component({
  selector: 'app-manager',
  templateUrl: './manager.component.html',
  standalone: false
})
export class ManagerComponent implements OnInit {
  consignationService = inject(ConsignationService);
  messageService = inject(MessageService);

  consignations: Consignation[] = []; // Pending consignations for audit queue
  allConsignations: Consignation[] = []; // All consignations for reports
  filteredConsignations: Consignation[] = []; // Filtered list for reports
  selectedConsignation: Consignation | null = null;
  reviewComments: string = '';
  currentTab: 'audit' | 'reports' = 'audit';

  // Queue Filters State
  queueSearchQuery: string = '';
  queueTypeFilter: 'all' | 'discrepancy' | 'illegible' | 'match' = 'all';

  // Reports Filters State
  filterStoreId: number | null = null;
  filterStartDate: string = '';
  filterEndDate: string = '';
  availableStores: { id: number; name: string }[] = [];

  stats = {
    totalDeclared: 0,
    totalValidated: 0,
    totalDiscrepancyAmount: 0,
    matchRate: 0,
    pendingCount: 0,
    validatedCount: 0,
    discrepancyCount: 0,
    illegibleCount: 0,
    rejectedCount: 0,
    pendingPercent: 0,
    validatedPercent: 0,
    discrepancyPercent: 0,
    illegiblePercent: 0,
    rejectedPercent: 0,
    totalCount: 0,
    bankDistribution: [] as { name: string; amount: number; percent: number; color: string }[]
  };

  ngOnInit() {
    this.loadConsignations();
    this.loadReportsData();
  }

  loadConsignations() {
    this.consignationService.getPendingConsignations().subscribe(data => {
      this.consignations = data;
      // Preselect the first discrepancy deposit for presentation
      const disc = this.consignations.find(d => d.detectedAmount !== d.declaredAmount);
      if (disc && !this.selectedConsignation) {
        this.selectedConsignation = disc;
      } else if (this.consignations.length > 0 && !this.selectedConsignation) {
        this.selectedConsignation = this.consignations[0];
      }
    });
  }

  loadReportsData() {
    this.consignationService.getAllConsignations().subscribe(data => {
      this.allConsignations = data;
      this.extractAvailableStores();
      this.applyFilters();
    });
  }

  extractAvailableStores() {
    const storeMap = new Map<number, string>();
    this.allConsignations.forEach(c => {
      if (c.storeId) {
        storeMap.set(c.storeId, c.storeName || `Punto ${c.storeId}`);
      }
    });
    this.availableStores = Array.from(storeMap.entries()).map(([id, name]) => ({ id, name }));
  }

  applyFilters() {
    let list = [...this.allConsignations];

    if (this.filterStoreId) {
      list = list.filter(c => c.storeId === Number(this.filterStoreId));
    }

    if (this.filterStartDate) {
      const start = new Date(this.filterStartDate + 'T00:00:00');
      list = list.filter(c => c.consignationDate && new Date(c.consignationDate) >= start);
    }

    if (this.filterEndDate) {
      const end = new Date(this.filterEndDate + 'T23:59:59');
      list = list.filter(c => c.consignationDate && new Date(c.consignationDate) <= end);
    }

    this.filteredConsignations = list;
    this.calculateStats();
  }

  resetFilters() {
    this.filterStoreId = null;
    this.filterStartDate = '';
    this.filterEndDate = '';
    this.applyFilters();
  }

  get filteredQueueConsignations(): Consignation[] {
    let list = [...this.consignations];

    // 1. Filter by Status/Type
    if (this.queueTypeFilter === 'discrepancy') {
      list = list.filter(d => d.statusId === 3 || (d.detectedAmount !== null && d.declaredAmount !== d.detectedAmount));
    } else if (this.queueTypeFilter === 'illegible') {
      list = list.filter(d => d.statusId === 4);
    } else if (this.queueTypeFilter === 'match') {
      list = list.filter(d => d.statusId === 1 && d.declaredAmount === d.detectedAmount);
    }

    // 2. Filter by search query
    if (this.queueSearchQuery) {
      const query = this.queueSearchQuery.toLowerCase();
      list = list.filter(d => 
        d.id.toString().includes(query) ||
        (d.storeName && d.storeName.toLowerCase().includes(query)) ||
        (d.bankName && d.bankName.toLowerCase().includes(query)) ||
        (d.referenceNumber && d.referenceNumber.toLowerCase().includes(query)) ||
        (d.notes && d.notes.toLowerCase().includes(query))
      );
    }

    return list;
  }

  calculateStats() {
    const total = this.filteredConsignations.length;
    if (total === 0) {
      this.stats = {
        totalDeclared: 0,
        totalValidated: 0,
        totalDiscrepancyAmount: 0,
        matchRate: 0,
        pendingCount: 0,
        validatedCount: 0,
        discrepancyCount: 0,
        illegibleCount: 0,
        rejectedCount: 0,
        pendingPercent: 0,
        validatedPercent: 0,
        discrepancyPercent: 0,
        illegiblePercent: 0,
        rejectedPercent: 0,
        totalCount: 0,
        bankDistribution: []
      };
      return;
    }

    let declaredSum = 0;
    let validatedSum = 0;
    let discrepancySum = 0;
    let matches = 0;

    let pending = 0;
    let validated = 0;
    let discrepancy = 0;
    let illegible = 0;
    let rejected = 0;

    const bankMap = new Map<string, number>();
    const colors = ['#00F2FE', '#4FACFE', '#00FF87', '#FEC163', '#DE4313'];

    this.filteredConsignations.forEach(c => {
      declaredSum += c.declaredAmount;
      if (c.statusId === 2) {
        validatedSum += c.declaredAmount;
      }
      
      const diff = Math.abs(c.declaredAmount - (c.detectedAmount || 0));
      if (c.statusId === 3 || c.declaredAmount !== c.detectedAmount) {
        discrepancySum += diff;
      }

      if (c.declaredAmount === c.detectedAmount) {
        matches++;
      }

      if (c.statusId === 1) pending++;
      else if (c.statusId === 2) validated++;
      else if (c.statusId === 3) discrepancy++;
      else if (c.statusId === 4) illegible++;
      else if (c.statusId === 5) rejected++;

      const bankName = c.bankName || 'Otro';
      bankMap.set(bankName, (bankMap.get(bankName) || 0) + c.declaredAmount);
    });

    // Calculate bank distribution
    const bankDist = Array.from(bankMap.entries()).map(([name, amount], index) => {
      return {
        name,
        amount,
        percent: Math.round((amount / (declaredSum || 1)) * 100),
        color: colors[index % colors.length]
      };
    }).sort((a, b) => b.amount - a.amount);

    this.stats = {
      totalDeclared: declaredSum,
      totalValidated: validatedSum,
      totalDiscrepancyAmount: discrepancySum,
      matchRate: Math.round((matches / total) * 100),
      pendingCount: pending,
      validatedCount: validated,
      discrepancyCount: discrepancy,
      illegibleCount: illegible,
      rejectedCount: rejected,
      pendingPercent: Math.round((pending / total) * 100),
      validatedPercent: Math.round((validated / total) * 100),
      discrepancyPercent: Math.round((discrepancy / total) * 100),
      illegiblePercent: Math.round((illegible / total) * 100),
      rejectedPercent: Math.round((rejected / total) * 100),
      totalCount: total,
      bankDistribution: bankDist
    };
  }

  setTab(tab: 'audit' | 'reports') {
    this.currentTab = tab;
    if (tab === 'reports') {
      this.loadReportsData();
    } else {
      this.loadConsignations();
    }
  }

  selectConsignationForReview(consignation: Consignation) {
    this.selectedConsignation = consignation;
    this.reviewComments = consignation.notes || '';
  }

  validateDeposit(consignationId: number) {
    this.consignationService.auditConsignation(consignationId, { statusId: 2, comments: this.reviewComments }).subscribe(() => {
      this.messageService.add({ severity: 'success', summary: 'Aprobada', detail: 'Consignación aprobada y conciliada.' });
      this.selectedConsignation = null;
      this.loadConsignations();
      this.loadReportsData();
    });
  }

  rejectDeposit(consignationId: number) {
    this.consignationService.auditConsignation(consignationId, { statusId: 5, comments: this.reviewComments }).subscribe(() => {
      this.messageService.add({ severity: 'error', summary: 'Rechazada', detail: 'Consignación rechazada. El cajero será notificado.' });
      this.selectedConsignation = null;
      this.loadConsignations();
      this.loadReportsData();
    });
  }

  getImageUrl(url: string | undefined): string {
    if (!url) return '';
    return url;
  }

  isBankMatch(c: Consignation): boolean {
    if (!c.ocr?.detectedBank) return false;
    const dbBank = c.bankName.toLowerCase();
    const ocrBank = c.ocr.detectedBank.toLowerCase();
    return ocrBank.includes(dbBank) || dbBank.includes(ocrBank);
  }

  isReferenceMatch(c: Consignation): boolean {
    if (!c.ocr?.detectedReference) return false;
    return c.referenceNumber === c.ocr.detectedReference;
  }

  isDateMatch(c: Consignation): boolean {
    if (!c.consignationDate || !c.ocr?.detectedDate) return false;
    try {
      const d1 = new Date(c.consignationDate).toISOString().split('T')[0];
      const d2 = new Date(c.ocr.detectedDate).toISOString().split('T')[0];
      return d1 === d2;
    } catch {
      return false;
    }
  }
}
