import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ApiError } from '../../core/api-error';
import { formatDisplayDate, formatMoney } from '../../core/dates.util';
import { environment } from '../../../environments/environment';
import type { SaleSummaryResponse } from '../../models/sale.model';
import { SalesApiService } from '../../services/sales-api.service';

const PAGE_SIZES = [5, 10, 20, 50] as const;

@Component({
  selector: 'app-sales-list',
  standalone: true,
  imports: [],
  templateUrl: './sales-list.component.html',
  styleUrl: './sales-list.component.scss',
})
export class SalesListComponent implements OnInit, OnDestroy {
  private readonly salesApi = inject(SalesApiService);
  readonly router = inject(Router);

  readonly apiBaseUrl = environment.apiBaseUrl;

  page = 1;
  pageSize = 5;
  readonly rows = signal<SaleSummaryResponse[]>([]);
  readonly totalPages = signal(1);
  readonly totalItems = signal(0);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly copied = signal(false);

  readonly pageSizes = PAGE_SIZES;

  formatDate = formatDisplayDate;
  formatMoney = formatMoney;

  private copyResetTimer: ReturnType<typeof setTimeout> | null = null;

  async copyApiBase(): Promise<void> {
    const url = this.apiBaseUrl;
    try {
      await navigator.clipboard.writeText(url);
      this.copied.set(true);
      if (this.copyResetTimer) clearTimeout(this.copyResetTimer);
      this.copyResetTimer = setTimeout(() => this.copied.set(false), 2000);
    } catch {
      void url;
    }
  }

  ngOnInit(): void {
    void this.load();
  }

  async load(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      const res = await firstValueFrom(
        this.salesApi.listSales(this.page, this.pageSize)
      );
      this.rows.set(Array.isArray(res.data) ? res.data : []);
      this.totalPages.set(Math.max(1, res.totalPages));
      this.totalItems.set(res.totalItems);
    } catch (e) {
      this.error.set(
        e instanceof ApiError ? e.message : 'Falha ao carregar vendas'
      );
      this.rows.set([]);
    } finally {
      this.loading.set(false);
    }
  }

  setPageSize(n: number): void {
    this.pageSize = n;
    this.page = 1;
    void this.load();
  }

  prevPage(): void {
    this.page = Math.max(1, this.page - 1);
    void this.load();
  }

  nextPage(): void {
    this.page = this.page + 1;
    void this.load();
  }

  ngOnDestroy(): void {
    if (this.copyResetTimer) clearTimeout(this.copyResetTimer);
  }
}
