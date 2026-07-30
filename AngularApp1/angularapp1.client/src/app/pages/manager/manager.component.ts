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

  consignations: Consignation[] = [];
  selectedConsignation: Consignation | null = null;
  reviewComments: string = '';

  ngOnInit() {
    this.loadConsignations();
  }

  loadConsignations() {
    this.consignationService.getPendingConsignations().subscribe(data => {
      this.consignations = data;
      // Preselect the first discrepancy deposit for presentation
      const disc = this.consignations.find(d => d.detectedAmount !== d.declaredAmount);
      if (disc && !this.selectedConsignation) {
        this.selectedConsignation = disc;
      }
    });
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
    });
  }

  rejectDeposit(consignationId: number) {
    this.consignationService.auditConsignation(consignationId, { statusId: 3, comments: this.reviewComments }).subscribe(() => {
      this.messageService.add({ severity: 'error', summary: 'Rechazada', detail: 'Consignación rechazada. El cajero será notificado.' });
      this.selectedConsignation = null;
      this.loadConsignations();
    });
  }

  getImageUrl(url: string | undefined): string {
    if (!url) return '';
    return url;
  }
}
