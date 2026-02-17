# 🧪 Guia de Testes - Histórico de Auditoria

## Testes Manuais (QA)

### ✅ Teste 1: Criar Solicitação e Visualizar Histórico Vazio

**Pré-requisito:** Login como User

**Passos:**
1. Navegar para "Nova Solicitação"
2. Preencher título, descrição, categoria e prioridade
3. Clicar "Criar"
4. Sistema redireciona para detalhe da solicitação
5. Rolar até seção "Histórico de Mudanças"

**Resultado Esperado:**
- Mensagem: "Nenhuma mudança de status registrada"
- Componente mostra ícone de histórico vazio

---

### ✅ Teste 2: Manager Aprova Solicitação com Comentário

**Pré-requisito:** Login como Manager, existir solicitação Pendente criada

**Passos:**
1. Navegar para a solicitação (status = Pendente)
2. Clicar botão "Aprovar"
3. Modal abre com campo de comentário
4. Preencher: "Aprovado conforme documentação"
5. Clicar "Confirmar"
6. Aguardar atualização
7. Rolar até "Histórico de Mudanças"

**Resultado Esperado:**
- Histórico mostra 1 entrada:
  - Status: "Pendente → Aprovada" ✅
  - Ícone: Checkmark verde
  - Comentário: "Aprovado conforme documentação"
  - Data/Hora: Atual
  - Quem: Email do Manager truncado

---

### ✅ Teste 3: Manager Rejeita Solicitação com Motivo

**Pré-requisito:** Login como Manager, existir solicitação Pendente criada

**Passos:**
1. Navegar para a solicitação (status = Pendente)
2. Clicar botão "Rejeitar"
3. Modal abre com campo de motivo
4. Preencher: "Falta documentação de autorização"
5. Clicar "Confirmar"
6. Aguardar atualização
7. Rolar até "Histórico de Mudanças"

**Resultado Esperado:**
- Histórico mostra 1 entrada:
  - Status: "Pendente → Rejeitada" ❌
  - Ícone: X vermelho
  - Motivo: "Falta documentação de autorização"
  - Data/Hora: Atual
  - Quem: Email do Manager truncado

---

### ✅ Teste 4: Múltiplas Ações Aparecem em Ordem Reversa

**Pré-requisito:** Uma solicitação com 3+ mudanças de status

**Passos:**
1. Abrir solicitação com histórico
2. Rolar até "Histórico de Mudanças"

**Resultado Esperado:**
- Entradas aparecem em ordem **mais recente primeiro** (DESC)
- Exemplo:
  ```
  1. [15:45] Pendente → Rejeitada (motivo: Falta dados)
  2. [14:30] Pendente → Aprovada (comentário: Validado)
  3. [13:00] Criado (sem mudança de status)
  ```

---

### ✅ Teste 5: Timeline Responsiva em Mobile

**Passos:**
1. Abrir solicitação em dispositivo mobile (ou DevTools)
2. Rolar até "Histórico de Mudanças"

**Resultado Esperado:**
- Componente adapta-se à tela
- Texto não é cortado
- Ícones estão visíveis
- Datas formatadas corretamente (DD/MM/YYYY HH:MM:SS)

---

### ✅ Teste 6: Sem Permissão (Erro 403)

**Pré-requisito:** Login como User que não criou a solicitação

**Passos:**
1. Tentar acessar detalhe de solicitação de outro usuário
2. Sistema tenta carregar histórico

**Resultado Esperado:**
- Toast/mensagem de erro: "Você não tem permissão para acessar este histórico"
- Histórico não carrega

---

### ✅ Teste 7: Sem Conexão (Network Error)

**Passos:**
1. Desativar conexão internet (ou simular no DevTools)
2. Abrir solicitação
3. Aguardar carregamento do histórico

**Resultado Esperado:**
- Toast/mensagem de erro: "Falha ao carregar histórico. Tente novamente"
- Componente mostra estado vazio graciouosamente

---

## Testes Automatizados (e2e com Cypress/Playwright)

### Teste de Carregamento do Histórico

```typescript
describe('Request History Feature', () => {
  beforeEach(() => {
    cy.login('manager@example.com', 'password');
    cy.visit('/requests');
  });

  it('should load and display request history timeline', () => {
    // Abrir primeira solicitação
    cy.get('[data-testid="request-row-0"]').click();

    // Aguardar carregamento
    cy.get('[data-testid="request-detail-title"]').should('be.visible');

    // Rolar até histórico
    cy.get('[data-testid="history-component"]').scrollIntoView();

    // Verificar que histórico carregou
    cy.get('[data-testid="history-item"]').should('have.length.greaterThan', 0);

    // Verificar primeira entrada
    cy.get('[data-testid="history-item-0"]').within(() => {
      cy.get('[data-testid="history-status-from"]').should('contain', 'Pendente');
      cy.get('[data-testid="history-status-to"]').should('contain', 'Aprovada');
      cy.get('[data-testid="history-date"]').should('be.visible');
    });
  });

  it('should show error when history fails to load', () => {
    // Interceptar e falhar requisição
    cy.intercept('GET', '/api/requests/*/history', { statusCode: 500 });

    cy.get('[data-testid="request-row-0"]').click();
    cy.get('[data-testid="history-component"]').scrollIntoView();

    // Verificar mensagem de erro
    cy.get('[data-testid="error-message"]').should('contain', 'Falha ao carregar histórico');
  });

  it('should display history in reverse chronological order', () => {
    cy.get('[data-testid="request-row-0"]').click();
    cy.get('[data-testid="history-component"]').scrollIntoView();

    // Pegar timestamps de todas as entradas
    cy.get('[data-testid="history-item"]').then(($items) => {
      const timestamps = [];
      $items.each((i, item) => {
        const time = Cypress.$(item).find('[data-testid="history-date"]').text();
        timestamps.push(new Date(time).getTime());
      });

      // Verificar que estão em ordem decrescente
      for (let i = 0; i < timestamps.length - 1; i++) {
        expect(timestamps[i]).to.be.greaterThan(timestamps[i + 1]);
      }
    });
  });
});
```

---

## Testes de Integração (Component Testing)

### Teste do RequestHistoryComponent

```typescript
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RequestHistoryComponent } from './request-history.component';
import { RequestsService } from '../requests.service';
import { of, throwError } from 'rxjs';

describe('RequestHistoryComponent', () => {
  let component: RequestHistoryComponent;
  let fixture: ComponentFixture<RequestHistoryComponent>;
  let requestsService: jasmine.SpyObj<RequestsService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('RequestsService', ['getRequestHistory']);

    await TestBed.configureTestingModule({
      imports: [RequestHistoryComponent],
      providers: [
        { provide: RequestsService, useValue: spy }
      ]
    }).compileComponents();

    requestsService = TestBed.inject(RequestsService) as jasmine.SpyObj<RequestsService>;
    fixture = TestBed.createComponent(RequestHistoryComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load history on init', () => {
    const mockHistory = [
      {
        id: '1',
        requestId: 'req-1',
        fromStatus: 0,
        toStatus: 1,
        changedBy: 'user-1',
        changedAt: '2026-02-18T14:30:00Z',
        comment: 'Aprovado'
      }
    ];

    requestsService.getRequestHistory.and.returnValue(of(mockHistory));

    component.requestId = 'req-1';
    component.ngOnInit();

    expect(requestsService.getRequestHistory).toHaveBeenCalledWith('req-1');
    expect(component.historicalEvents()).toEqual(mockHistory);
  });

  it('should handle error when loading history', () => {
    const error = new Error('Network error');
    requestsService.getRequestHistory.and.returnValue(throwError(() => error));

    component.requestId = 'req-1';
    component.ngOnInit();

    expect(component.historicalEvents()).toEqual([]);
  });

  it('should return correct status label', () => {
    expect(component.getStatusLabel(0)).toBe('Pendente');
    expect(component.getStatusLabel(1)).toBe('Aprovada');
    expect(component.getStatusLabel(2)).toBe('Rejeitada');
  });

  it('should return correct status color', () => {
    expect(component.getStatusColor(0)).toBe('warn');
    expect(component.getStatusColor(1)).toBe('accent');
    expect(component.getStatusColor(2)).toBe('primary');
  });

  it('should return correct status icon', () => {
    expect(component.getStatusIcon(0)).toBe('hourglass_empty');
    expect(component.getStatusIcon(1)).toBe('check_circle');
    expect(component.getStatusIcon(2)).toBe('cancel');
  });
});
```

---

## Dados de Teste

### Solicitação com Histórico Completo

```typescript
const mockRequestWithHistory = {
  request: {
    id: 'req-123',
    title: 'Requisição de Compra - Laptop',
    description: 'Computador para desenvolvimento',
    category: 0,
    priority: 2,
    status: 1,
    createdBy: 'user-001',
    createdAt: '2026-02-18T10:00:00Z'
  },
  history: [
    {
      id: 'hist-003',
      requestId: 'req-123',
      fromStatus: 0,
      toStatus: 1,
      changedBy: 'manager-001',
      changedAt: '2026-02-18T14:30:00Z',
      comment: 'Aprovado conforme orçamento disponível'
    },
    {
      id: 'hist-002',
      requestId: 'req-123',
      fromStatus: 0,
      toStatus: 0,
      changedBy: 'user-001',
      changedAt: '2026-02-18T11:00:00Z',
      comment: null // Criação não gera histórico
    }
  ]
};
```

---

## Matriz de Testes

| Cenário | User Role | Ação | Resultado Esperado | Status |
|---------|-----------|------|-------------------|--------|
| Criar solicitação | User | Criar | Sem histórico inicialmente | ✅ |
| Aprovar com comentário | Manager | Aprovar | Histórico: 0→1 com comentário | ✅ |
| Rejeitar com motivo | Manager | Rejeitar | Histórico: 0→2 com motivo | ✅ |
| Múltiplas ações | Manager | Múltiplas | Histórico em ordem DESC | ✅ |
| Sem permissão | User (outro) | Ver histórico | Erro 403 | ✅ |
| Erro de rede | Any | Carregar | Mensagem de erro | ✅ |
| Mobile | Any | Visualizar | Responsivo | ✅ |
| Sem conexão | Any | Carregar | Erro gracioso | ✅ |

---

## Checklist Final

- [ ] Histórico carrega sem erros
- [ ] Entradas aparecem em ordem correta (mais recente primeiro)
- [ ] Ícones e cores correspondem ao status
- [ ] Datas formatadas em PT-BR
- [ ] Comentários/motivos exibidos quando presentes
- [ ] Responsivo em mobile
- [ ] Erros tratados graciosamente
- [ ] Teste com múltiplos navegadores (Chrome, Firefox, Safari)
- [ ] Performance aceitável com muitas entradas
- [ ] Acessibilidade (WCAG 2.1)

---

**Data:** 18/02/2026  
**Versão:** 1.0
