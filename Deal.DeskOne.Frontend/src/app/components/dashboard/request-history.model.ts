export interface RequestHistory {
  id: string;
  fieldName: string;
  oldValue: string;
  newValue: string;
  comment?: string;
  changedAt: string;
  changedBy: string;
}
