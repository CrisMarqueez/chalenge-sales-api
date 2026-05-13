import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ApiError } from '../../core/api-error';
import { formatDisplayDate, formatMoney } from '../../core/dates.util';
import type { SaleDetailResponse } from '../../models/sale.model';
import { SalesApiService } from '../../services/sales-api.service';

@Component({
  selector: 'app-sale-detail',
  standalone: true,
  templateUrl: './sale-detail.component.html',
  styleUrl: './sale-detail.component.scss',
})
export class SaleDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  readonly router = inject(Router);
  private readonly salesApi = inject(SalesApiService);

  id: string | null = null;
  readonly sale = signal<SaleDetailResponse | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly deleteOpen = signal(false);
  readonly cancelSaleOpen = signal(false);
  readonly cancelItemId = signal<string | null>(null);
  readonly busy = signal(false);

  formatDate = formatDisplayDate;
  formatMoney = formatMoney;

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) void this.load();
    else this.loading.set(false);
  }

  async load(): Promise<void> {
    if (!this.id) return;
    this.loading.set(true);
    this.error.set(null);
    try {
      const data = await firstValueFrom(this.salesApi.getSale(this.id));
      this.sale.set(data);
    } catch (e) {
      this.error.set(
        e instanceof ApiError ? e.message : 'Não foi possível carregar a venda'
      );
      this.sale.set(null);
    } finally {
      this.loading.set(false);
    }
  }

  async handleDelete(): Promise<void> {
    if (!this.id) return;
    this.busy.set(true);
    try {
      await firstValueFrom(this.salesApi.deleteSale(this.id));
      this.deleteOpen.set(false);
      await this.router.navigateByUrl('/sales');
    } catch (e) {
      this.error.set(e instanceof ApiError ? e.message : 'Falha ao excluir');
    } finally {
      this.busy.set(false);
    }
  }

  async handleCancelSale(): Promise<void> {
    if (!this.id) return;
    this.busy.set(true);
    try {
      const updated = await firstValueFrom(this.salesApi.cancelSale(this.id));
      this.sale.set(updated);
      this.cancelSaleOpen.set(false);
    } catch (e) {
      this.error.set(e instanceof ApiError ? e.message : 'Falha ao cancelar venda');
    } finally {
      this.busy.set(false);
    }
  }

  async handleCancelItem(itemId: string): Promise<void> {
    if (!this.id) return;
    this.busy.set(true);
    try {
      const updated = await firstValueFrom(
        this.salesApi.cancelSaleItem(this.id, itemId)
      );
      this.sale.set(updated);
      this.cancelItemId.set(null);
    } catch (e) {
      this.error.set(e instanceof ApiError ? e.message : 'Falha ao cancelar item');
    } finally {
      this.busy.set(false);
    }
  }
}
