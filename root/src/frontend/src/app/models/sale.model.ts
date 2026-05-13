export interface SaleLineItemRequest {
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  isCancelled?: boolean;
}

export interface CreateSaleRequest {
  saleDate: string;
  customerId: string;
  customerName: string;
  branchId: string;
  branchName: string;
  items: SaleLineItemRequest[];
}

export interface UpdateSaleRequest {
  saleDate: string;
  customerId: string;
  customerName: string;
  branchId: string;
  branchName: string;
  isCancelled: boolean;
  items: SaleLineItemRequest[];
}

export interface SaleLineItemResponse {
  id: string;
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  discountAmount: number;
  lineTotal: number;
  isCancelled: boolean;
}

export interface SaleDetailResponse {
  id: string;
  saleNumber: string;
  saleDate: string;
  customerId: string;
  customerName: string;
  branchId: string;
  branchName: string;
  totalAmount: number;
  isCancelled: boolean;
  items: SaleLineItemResponse[];
}

export interface SaleSummaryResponse {
  id: string;
  saleNumber: string;
  saleDate: string;
  customerName: string;
  branchName: string;
  totalAmount: number;
  isCancelled: boolean;
}

export interface PaginatedSalesResponse {
  data: SaleSummaryResponse[];
  currentPage: number;
  totalPages: number;
  totalItems: number;
}

export interface ApiResponseWithData<T> {
  success: boolean;
  message?: string;
  data?: T;
}
