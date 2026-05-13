import type {
  PaginatedSalesResponse,
  SaleDetailResponse,
  SaleLineItemResponse,
  SaleSummaryResponse,
} from '../models/sale.model';

function pick(
  o: Record<string, unknown>,
  ...keys: string[]
): unknown {
  for (const k of keys) {
    if (k in o && o[k] !== undefined && o[k] !== null) return o[k];
  }
  return undefined;
}

function normalizeLine(row: unknown): SaleLineItemResponse {
  if (!row || typeof row !== 'object') {
    return {
      id: '',
      productId: '',
      productName: '',
      quantity: 0,
      unitPrice: 0,
      discountAmount: 0,
      lineTotal: 0,
      isCancelled: false,
    };
  }
  const r = row as Record<string, unknown>;
  return {
    id: String(pick(r, 'id', 'Id') ?? ''),
    productId: String(pick(r, 'productId', 'ProductId') ?? ''),
    productName: String(pick(r, 'productName', 'ProductName') ?? ''),
    quantity: Number(pick(r, 'quantity', 'Quantity') ?? 0),
    unitPrice: Number(pick(r, 'unitPrice', 'UnitPrice') ?? 0),
    discountAmount: Number(pick(r, 'discountAmount', 'DiscountAmount') ?? 0),
    lineTotal: Number(pick(r, 'lineTotal', 'LineTotal') ?? 0),
    isCancelled: Boolean(pick(r, 'isCancelled', 'IsCancelled')),
  };
}

export function unwrapApiEnvelope(raw: unknown): unknown {
  if (!raw || typeof raw !== 'object') return null;
  const o = raw as Record<string, unknown>;
  return pick(o, 'data', 'Data');
}

export function tryParseDocumentedPagedList(
  raw: unknown,
  fallbackPage: number
): {
  data: unknown[];
  totalItems: number;
  currentPage: number;
  totalPages: number;
} | null {
  if (!raw || typeof raw !== 'object' || Array.isArray(raw)) return null;
  const o = raw as Record<string, unknown>;
  const rootData = pick(o, 'data', 'Data');
  if (!Array.isArray(rootData) || pick(o, 'totalItems', 'TotalItems') === undefined)
    return null;
  return {
    data: rootData,
    totalItems: Number(pick(o, 'totalItems', 'TotalItems') ?? rootData.length),
    currentPage: Number(pick(o, 'currentPage', 'CurrentPage') ?? fallbackPage),
    totalPages: Math.max(
      1,
      Number(pick(o, 'totalPages', 'TotalPages') ?? 1)
    ),
  };
}

export function unwrapSaleDetailEnvelope(raw: unknown): unknown {
  let cur: unknown = raw;
  for (let i = 0; i < 6; i++) {
    if (!cur || typeof cur !== 'object') return cur;
    const o = cur as Record<string, unknown>;
    const items = pick(o, 'items', 'Items');
    const saleNumber = pick(o, 'saleNumber', 'SaleNumber');
    const looksLikeSale =
      Array.isArray(items) ||
      (typeof saleNumber === 'string' && saleNumber.length > 0);
    if (looksLikeSale) return cur;

    const nested = pick(o, 'data', 'Data');
    if (nested === undefined || nested === null) return cur;
    if (typeof nested !== 'object') return cur;
    cur = nested;
  }
  return cur;
}

export function normalizeSaleSummary(row: unknown): SaleSummaryResponse {
  if (!row || typeof row !== 'object') {
    return {
      id: '',
      saleNumber: '',
      saleDate: '',
      customerName: '',
      branchName: '',
      totalAmount: 0,
      isCancelled: false,
    };
  }
  const r = row as Record<string, unknown>;
  return {
    id: String(pick(r, 'id', 'Id') ?? ''),
    saleNumber: String(pick(r, 'saleNumber', 'SaleNumber') ?? ''),
    saleDate: String(pick(r, 'saleDate', 'SaleDate') ?? ''),
    customerName: String(pick(r, 'customerName', 'CustomerName') ?? ''),
    branchName: String(pick(r, 'branchName', 'BranchName') ?? ''),
    totalAmount: Number(pick(r, 'totalAmount', 'TotalAmount') ?? 0),
    isCancelled: Boolean(pick(r, 'isCancelled', 'IsCancelled')),
  };
}

export function normalizePaginatedSalesResponse(
  raw: unknown,
  fallbackPage: number
): PaginatedSalesResponse {
  if (!raw || typeof raw !== 'object') {
    return {
      data: [],
      currentPage: fallbackPage,
      totalPages: 1,
      totalItems: 0,
    };
  }
  const o = raw as Record<string, unknown>;

  const documented = tryParseDocumentedPagedList(raw, fallbackPage);
  if (documented) {
    return {
      data: documented.data.map(normalizeSaleSummary),
      currentPage: documented.currentPage,
      totalPages: documented.totalPages,
      totalItems: documented.totalItems,
    };
  }

  const dataField = pick(o, 'data', 'Data');

  let listRaw: unknown;
  let currentPage = fallbackPage;
  let totalPages = 1;
  let totalItems = 0;

  if (
    dataField != null &&
    typeof dataField === 'object' &&
    !Array.isArray(dataField)
  ) {
    const inner = dataField as Record<string, unknown>;
    listRaw = pick(inner, 'data', 'items', 'Items', 'Data');
    currentPage = Number(
      pick(inner, 'currentPage', 'CurrentPage') ?? fallbackPage
    );
    totalPages = Math.max(
      1,
      Number(pick(inner, 'totalPages', 'TotalPages') ?? 1)
    );
    totalItems = Number(pick(inner, 'totalItems', 'TotalItems') ?? 0);
  } else {
    listRaw = dataField ?? pick(o, 'items', 'Items');
    currentPage = Number(pick(o, 'currentPage', 'CurrentPage') ?? fallbackPage);
    totalPages = Math.max(
      1,
      Number(pick(o, 'totalPages', 'TotalPages') ?? 1)
    );
    totalItems = Number(pick(o, 'totalItems', 'TotalItems') ?? 0);
  }

  const data: SaleSummaryResponse[] = Array.isArray(listRaw)
    ? listRaw.map(normalizeSaleSummary)
    : [];

  if (totalItems === 0 && data.length > 0) {
    totalItems = data.length;
  }

  return {
    data,
    currentPage,
    totalPages,
    totalItems,
  };
}

export function normalizeSaleDetailPayload(raw: unknown): SaleDetailResponse {
  if (!raw || typeof raw !== 'object') {
    throw new Error('Resposta sem dados');
  }
  const o = raw as Record<string, unknown>;
  const itemsRaw = pick(o, 'items', 'Items');
  const items: SaleLineItemResponse[] = Array.isArray(itemsRaw)
    ? itemsRaw.map(normalizeLine)
    : [];

  return {
    id: String(pick(o, 'id', 'Id') ?? ''),
    saleNumber: String(pick(o, 'saleNumber', 'SaleNumber') ?? ''),
    saleDate: String(pick(o, 'saleDate', 'SaleDate') ?? ''),
    customerId: String(pick(o, 'customerId', 'CustomerId') ?? ''),
    customerName: String(pick(o, 'customerName', 'CustomerName') ?? ''),
    branchId: String(pick(o, 'branchId', 'BranchId') ?? ''),
    branchName: String(pick(o, 'branchName', 'BranchName') ?? ''),
    totalAmount: Number(pick(o, 'totalAmount', 'TotalAmount') ?? 0),
    isCancelled: Boolean(pick(o, 'isCancelled', 'IsCancelled')),
    items,
  };
}
