export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  fullName: string;
  email: string;
  roleId: number;
  roleName: string;
  companyId: number;
  companyName: string;
  storeId: number | null;
  storeName: string;
  expiresAt: string;
}

export interface DecodedToken {
  sub: string;
  email: string;
  name: string;
  role: string;
  roleId: string;
  companyId: string;
  companyName: string;
  storeId: string;
  storeName: string;
  exp: number;
  iss: string;
  aud: string;
}
