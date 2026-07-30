export interface Consignation {
  id: number;
  companyId: number;
  companyName: string;
  storeId: number;
  storeName: string;
  bankId: number;
  bankName: string;
  statusId: number;
  statusName: string;
  referenceNumber: string;
  declaredAmount: number;
  detectedAmount?: number;
  consignationDate?: string; // ISO date string
  consignationTime?: string; // time string
  notes?: string;
  createdByName?: string;
  validatedByName?: string;
  validationDate?: string;
  createdAt: string;
  fileUrl?: string;
  ocr?: OcrResult;
}

export interface OcrResult {
  id: number;
  consignationId: number;
  detectedBank: string;
  detectedReference: string;
  detectedAmount?: number;
  detectedDate?: string;
  confidence: number;
  rawText: string;
  processedAt: string;
}

export interface AuditConsignation {
  statusId: number;
  comments: string;
}
