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
