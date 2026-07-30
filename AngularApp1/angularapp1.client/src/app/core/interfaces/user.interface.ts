export interface User {
  id?: number;
  companyId: number;
  companyName?: string;
  storeId?: number | null;
  storeName?: string;
  roleId: number;
  roleName?: string;
  fullName: string;
  email: string;
  password?: string;
  isActive: boolean;
  createdAt?: string;
}
