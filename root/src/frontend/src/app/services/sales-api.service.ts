import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiClientService } from '../core/api-client.service';
import {
  normalizePaginatedSalesResponse,
  normalizeSaleDetailPayload,
  unwrapSaleDetailEnvelope,
} from '../core/sale-payload.util';
import type {
  CreateSaleRequest,
  PaginatedSalesResponse,
  SaleDetailResponse,
  UpdateSaleRequest,
} from '../models/sale.model';

@Injectable({ providedIn: 'root' })
export class SalesApiService {
  private readonly api = inject(ApiClientService);

  private parseSaleDetailFromResponse(raw: unknown): SaleDetailResponse {
    const inner = unwrapSaleDetailEnvelope(raw);
    if (inner == null || typeof inner !== 'object')
      throw new Error('Resposta sem dados');
    return normalizeSaleDetailPayload(inner);
  }

  listSales(page: number, pageSize: number): Observable<PaginatedSalesResponse> {
    const q = new URLSearchParams({
      _page: String(page),
      _size: String(pageSize),
    });
    return this.api
      .requestJson<unknown>(`/api/Sales?${q}`)
      .pipe(map((raw) => normalizePaginatedSalesResponse(raw, page)));
  }

  getSale(id: string): Observable<SaleDetailResponse> {
    return this.api
      .requestJson<unknown>(`/api/Sales/${id}`)
      .pipe(map((r) => this.parseSaleDetailFromResponse(r)));
  }

  createSale(body: CreateSaleRequest): Observable<SaleDetailResponse> {
    return this.api
      .requestJson<unknown>('/api/Sales', { method: 'POST', body })
      .pipe(map((r) => this.parseSaleDetailFromResponse(r)));
  }

  updateSale(
    id: string,
    body: UpdateSaleRequest
  ): Observable<SaleDetailResponse> {
    return this.api
      .requestJson<unknown>(`/api/Sales/${id}`, { method: 'PUT', body })
      .pipe(map((r) => this.parseSaleDetailFromResponse(r)));
  }

  deleteSale(id: string): Observable<void> {
    return this.api.requestJson<{ message: string }>(`/api/Sales/${id}`, {
      method: 'DELETE',
    }).pipe(map(() => undefined));
  }

  cancelSale(id: string): Observable<SaleDetailResponse> {
    return this.api
      .requestJson<unknown>(`/api/Sales/${id}/cancel`, { method: 'POST' })
      .pipe(map((r) => this.parseSaleDetailFromResponse(r)));
  }

  cancelSaleItem(
    saleId: string,
    itemId: string
  ): Observable<SaleDetailResponse> {
    return this.api
      .requestJson<unknown>(
        `/api/Sales/${saleId}/items/${itemId}/cancel`,
        { method: 'POST' }
      )
      .pipe(map((r) => this.parseSaleDetailFromResponse(r)));
  }
}
