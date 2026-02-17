import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Request,
  CreateRequestCommand,
  UpdateRequestCommand,
  ApproveRequestCommand,
  RejectRequestCommand
} from './request.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RequestsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiBaseUrl}/requests`;

  /**
   * Criar uma nova solicitação
   * POST /api/requests
   */
  createRequest(command: CreateRequestCommand): Observable<void> {
    return this.http.post<void>(this.apiUrl, command);
  }

  /**
   * Listar todas as solicitações
   * GET /api/requests
   */
  getRequests(): Observable<Request[]> {
    return this.http.get<Request[]>(this.apiUrl);
  }

  /**
   * Buscar uma solicitação específica pelo ID
   * GET /api/requests/{id}
   */
  getRequestById(id: string): Observable<Request> {
    return this.http.get<Request>(`${this.apiUrl}/${id}`);
  }

  /**
   * Atualizar uma solicitação existente
   * PUT /api/requests/{id}
   */
  updateRequest(id: string, command: UpdateRequestCommand): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, command);
  }

  /**
   * Deletar uma solicitação
   * DELETE /api/requests/{id}
   */
  deleteRequest(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Aprovar uma solicitação
   * POST /api/requests/{id}/approve
   */
  approveRequest(id: string, command: ApproveRequestCommand): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/approve`, command);
  }

  /**
   * Rejeitar uma solicitação
   * POST /api/requests/{id}/reject
   */
  rejectRequest(id: string, command: RejectRequestCommand): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/reject`, command);
  }
}
