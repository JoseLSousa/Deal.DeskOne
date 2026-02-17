# 📋 Guia de Implementação - Histórico de Auditoria (Timeline)

## 🎯 Visão Geral

A feature de **Histórico de Auditoria** rastreia todas as mudanças de status de uma solicitação (Request). Quando um gestor aprova ou rejeita uma solicitação, um registro é criado automaticamente com informações sobre quem fez a ação, quando foi feita e qual comentário foi deixado.

**Objetivo:** Exibir uma timeline visual mostrando toda a jornada de uma solicitação desde sua criação até a decisão final.

---

## 🔌 API Backend Disponível

### Endpoint: Obter Histórico de Mudanças

```
GET /api/requests/{requestId}/history
Authorization: Bearer {token}
```

**Parâmetros:**
- `requestId` (path parameter): UUID da solicitação

**Resposta (200 OK):**
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "requestId": "550e8400-e29b-41d4-a716-446655440000",
    "fromStatus": 0,
    "toStatus": 1,
    "changedBy": "550e8400-e29b-41d4-a716-446655440002",
    "changedAt": "2026-02-18T14:30:00Z",
    "comment": "Aprovado conforme solicitado"
  },
  {
    "id": "550e8400-e29b-41d4-a716-446655440003",
    "requestId": "550e8400-e29b-41d4-a716-446655440000",
    "fromStatus": 0,
    "toStatus": 2,
    "changedBy": "550e8400-e29b-41d4-a716-446655440004",
    "changedAt": "2026-02-18T15:45:00Z",
    "comment": "Falta documentação necessária"
  }
]
```

**Campos de Resposta:**
| Campo | Tipo | Descrição |
|-------|------|-----------|
| `id` | UUID | Identificador único do registro de histórico |
| `requestId` | UUID | ID da solicitação associada |
| `fromStatus` | int | Status anterior (0=Pendente, 1=Aprovada, 2=Rejeitada) |
| `toStatus` | int | Novo status |
| `changedBy` | UUID | ID do usuário que fez a mudança |
| `changedAt` | DateTime (ISO 8601) | Data/hora da mudança em UTC |
| `comment` | string\|null | Comentário/justificativa (opcional) |

**Status Codes:**
- `200 OK`: Histórico retornado com sucesso
- `401 Unauthorized`: Token não fornecido ou inválido
- `403 Forbidden`: Usuário sem permissão de leitura

---

## 📦 Modelos de Dados (TypeScript)

### RequestHistory Interface

```typescript
export interface RequestHistory {
  id: string;
  requestId: string;
  fromStatus: number;
  toStatus: number;
  changedBy: string;
  changedAt: string;
  comment?: string;
}

export const StatusLabels: Record<number, string> = {
  0: 'Pendente',
  1: 'Aprovada',
  2: 'Rejeitada'
};

export const StatusColors: Record<number, string> = {
  0: 'warn',      // Amarelo/Laranja
  1: 'accent',    // Verde
  2: 'primary'    // Vermelho
};

export const StatusIcons: Record<number, string> = {
  0: 'hourglass_empty',  // Relógio
  1: 'check_circle',     // Verificado
  2: 'cancel'            // Cancelado
};
```

---

## 🛠️ Integração no RequestsService

O método `getRequestHistory()` já está implementado no `RequestsService`:

```typescript
/**
 * Obter histórico de mudanças de uma solicitação
 * GET /api/requests/{id}/history
 */
getRequestHistory(id: string): Observable<RequestHistory[]> {
  return this.http.get<RequestHistory[]>(`${this.apiUrl}/${id}/history`);
}
```

**Uso:**
```typescript
constructor(private requestsService: RequestsService) {}

loadHistory(requestId: string) {
  this.requestsService.getRequestHistory(requestId).subscribe({
    next: (history) => {
      console.log('Histórico carregado:', history);
      this.historicalEvents.set(history);
    },
    error: (err) => {
      console.error('Erro ao carregar histórico:', err);
      this.showError('Falha ao carregar histórico');
    }
  });
}
```

---

## 🎨 Componente RequestHistoryComponent

Um componente **standalone** foi criado em:
`Deal.DeskOne.Frontend/src/app/components/dashboard/request-history/request-history.ts`

**Características:**
- ✅ Standalone (pode ser usado em qualquer componente)
- ✅ Signals do Angular para reatividade
- ✅ Material Design icons e cards
- ✅ Timeline visual com cores e ícones
- ✅ Suporte a múltiplas entradas de histórico
- ✅ Formatação de datas em PT-BR

**Inputs:**
```typescript
@Input() requestId: string = '';  // ID da solicitação
```

**Signals:**
```typescript
readonly historicalEvents = signal<RequestHistory[]>([]);
readonly loading = signal(false);
```

---

## 📱 Integração no Componente de Detalhe

### 1️⃣ Importar o Componente

```typescript
import { RequestHistoryComponent } from './request-history/request-history';

@Component({
  selector: 'app-request-detail',
  imports: [
    CommonModule,
    RequestHistoryComponent,  // ← Adicionar aqui
    // ... outros imports
  ],
  template: `
    <!-- seu HTML -->
  `
})
export class RequestDetailComponent {
  // ...
}
```

### 2️⃣ Adicionar no Template

```html
<!-- Após exibir os dados principais da solicitação -->
<div class="request-details">
  <!-- Dados da solicitação -->
  <h2>{{ request.title }}</h2>
  <p>{{ request.description }}</p>
  <!-- ... mais dados ... -->

  <!-- Histórico/Timeline -->
  <app-request-history 
    [requestId]="request.id">
  </app-request-history>
</div>
```

---

## 📊 Fluxo de Dados

```
Componente de Detalhe
    ↓
Carrega RequestHistory com requestId
    ↓
RequestsService.getRequestHistory(id)
    ↓
GET /api/requests/{id}/history
    ↓
Backend retorna array de mudanças
    ↓
RequestHistoryComponent renderiza timeline
```

---

## ✨ Exemplo de Uso Completo

### Componente Pai (RequestDetailComponent)

```typescript
import { Component, OnInit, signal, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RequestHistoryComponent } from './request-history/request-history';
import { RequestsService } from './requests.service';
import { Request } from './request.model';

@Component({
  selector: 'app-request-detail',
  standalone: true,
  imports: [CommonModule, RequestHistoryComponent],
  template: `
    <div class="request-container">
      <h1>Detalhe da Solicitação</h1>
      
      @if (request(); as req) {
        <div class="request-info">
          <h2>{{ req.title }}</h2>
          <p>{{ req.description }}</p>
          <p><strong>Status:</strong> {{ getStatusLabel(req.status) }}</p>
          <p><strong>Categoria:</strong> {{ getCategoryLabel(req.category) }}</p>
          <p><strong>Prioridade:</strong> {{ getPriorityLabel(req.priority) }}</p>
          <p><strong>Criado por:</strong> {{ req.createdBy }}</p>
          <p><strong>Data de criação:</strong> {{ req.createdAt | date:'dd/MM/yyyy HH:mm' }}</p>
        </div>

        <!-- Componente de histórico -->
        <app-request-history [requestId]="req.id"></app-request-history>
      }
    </div>
  `,
  styles: [`
    .request-container {
      padding: 20px;
      max-width: 1000px;
      margin: 0 auto;
    }

    .request-info {
      background-color: #f5f5f5;
      padding: 16px;
      border-radius: 4px;
      margin-bottom: 24px;
    }

    .request-info h2 {
      margin: 0 0 12px 0;
    }

    .request-info p {
      margin: 8px 0;
    }
  `]
})
export class RequestDetailComponent implements OnInit {
  private readonly requestsService = inject(RequestsService);
  private readonly route = inject(ActivatedRoute);

  readonly request = signal<Request | null>(null);
  readonly loading = signal(false);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadRequest(id);
    }
  }

  private loadRequest(id: string) {
    this.loading.set(true);
    this.requestsService.getRequestById(id).subscribe({
      next: (request) => {
        this.request.set(request);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Erro ao carregar solicitação:', err);
        this.loading.set(false);
      }
    });
  }

  getStatusLabel(status: number): string {
    const labels: Record<number, string> = {
      0: 'Pendente',
      1: 'Aprovada',
      2: 'Rejeitada'
    };
    return labels[status] || 'Desconhecido';
  }

  getCategoryLabel(category: number): string {
    const labels: Record<number, string> = {
      0: 'Compra',
      1: 'Acesso',
      2: 'Reembolso',
      3: 'TI',
      4: 'Outro'
    };
    return labels[category] || 'Desconhecido';
  }

  getPriorityLabel(priority: number): string {
    const labels: Record<number, string> = {
      0: 'Baixa',
      1: 'Média',
      2: 'Alta'
    };
    return labels[priority] || 'Desconhecido';
  }
}
```

---

## 🔄 Fluxo de Aprovação com Histórico

### Cenário 1: User Cria Solicitação

1. User clica em "Nova Solicitação"
2. Preenche formulário e clica "Criar"
3. Backend cria a solicitação com status `0 (Pendente)`
4. **Histórico NÃO é criado neste momento** (status inicial não gera mudança)

### Cenário 2: Manager Aprova

1. Manager entra na tela de detalhe da solicitação
2. Sistema carrega histórico (vazio ou com ações anteriores)
3. Manager clica "Aprovar" e deixa comentário (opcional)
4. Backend:
   - Atualiza status para `1 (Aprovada)`
   - **Cria registro de histórico**: `0 → 1` com comentário
5. Frontend carrega novamente o histórico
6. Timeline mostra: "Pendente → Aprovada" com data, horário e comentário

### Cenário 3: Manager Rejeita

1. Manager entra na tela de detalhe da solicitação
2. Sistema carrega histórico
3. Manager clica "Rejeitar" e preenche motivo (obrigatório)
4. Backend:
   - Atualiza status para `2 (Rejeitada)`
   - **Cria registro de histórico**: `0 → 2` com motivo
5. Frontend carrega novamente o histórico
6. Timeline mostra: "Pendente → Rejeitada" com data, horário e motivo

---

## 🐛 Tratamento de Erros

### Erro ao Carregar Histórico

```typescript
loadHistory(requestId: string) {
  this.loading.set(true);
  this.requestsService.getRequestHistory(requestId).subscribe({
    next: (history) => {
      this.historicalEvents.set(history);
      this.loading.set(false);
    },
    error: (err) => {
      console.error('Erro ao carregar histórico:', err);
      
      // Mostrar mensagem amigável ao usuário
      if (err.status === 401) {
        this.showError('Sessão expirada. Faça login novamente.');
      } else if (err.status === 403) {
        this.showError('Você não tem permissão para acessar este histórico.');
      } else {
        this.showError('Falha ao carregar histórico. Tente novamente.');
      }
      
      this.loading.set(false);
    }
  });
}

private showError(message: string) {
  // Usar MatSnackBar ou toast do projeto
  console.error(message);
}
```

---

## 📋 Checklist de Implementação

- [ ] Importar `RequestHistoryComponent` no componente de detalhe
- [ ] Adicionar `<app-request-history>` ao template
- [ ] Passar `[requestId]` como input
- [ ] Testar carregamento do histórico
- [ ] Testar com múltiplas ações (aprovar, rejeitar)
- [ ] Validar formatação de datas (PT-BR)
- [ ] Testar responsividade em mobile
- [ ] Testar tratamento de erro (sem conexão, sem permissão, etc)
- [ ] Ajustar estilos conforme design system do projeto

---

## 🎨 Customização de Estilos

O componente usa classes CSS que podem ser customizadas:

```typescript
// Cores por status
const colors = {
  0: '#FFA500',  // Laranja (Pendente)
  1: '#4CAF50',  // Verde (Aprovada)
  2: '#F44336'   // Vermelho (Rejeitada)
};

// Ícones (Material Icons)
const icons = {
  0: 'hourglass_empty',
  1: 'check_circle',
  2: 'cancel'
};
```

---

## 📞 Contato Backend

**Endpoints Relacionados:**
- `POST /api/requests/{id}/approve` - Aprovar com comentário opcional
- `POST /api/requests/{id}/reject` - Rejeitar com motivo obrigatório
- `GET /api/requests/{id}/history` - Obter histórico ✅ (Backend pronto)

**Contato:** Time Backend - Slack: @backend-team

---

## 🚀 Próximos Passos

1. ✅ Backend implementou histórico de auditoria
2. ⏳ Frontend integra componente de timeline
3. ⏳ Testes E2E validam fluxo completo
4. ⏳ Deploy em produção

---

## 📚 Referências

- [Angular Signals Documentation](https://angular.io/guide/signals)
- [Material Design Components](https://material.angular.io/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)

---

**Versão:** 1.0  
**Data:** 18/02/2026  
**Status:** ✅ Backend Implementado | ⏳ Frontend em Desenvolvimento
