export interface Deposit {
  id: string;
  storeId: number;
  storeName: string;
  bankName: string;
  declaredAmount: number;
  ocrAmount?: number;
  ocrBank?: string;
  reference?: string;
  date?: string;
  confidence?: number;
  status: 'Matched' | 'Discrepancy' | 'Pending' | 'Error';
  notes?: string;
}
