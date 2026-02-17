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
  2: 'Rejeitada',
};

export const StatusIcons: Record<number, string> = {
  0: 'hourglass_empty',
  1: 'check_circle',
  2: 'cancel',
};

export const StatusColors: Record<number, string> = {
  0: '#FFA500',
  1: '#4CAF50',
  2: '#F44336',};