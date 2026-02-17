export interface Request {
  id: string;
  title: string;
  description: string;
  category: number;
  priority: number;
  status: number;
  createdAt: string;
}

export interface CreateRequestCommand {
  title: string;
  description: string;
  category: number;
  priority: number;
}

export interface UpdateRequestCommand {
  title?: string;
  description?: string;
  category?: number;
  priority?: number;
}

export interface ApproveRequestCommand {
  approvedBy: string;
}

export interface RejectRequestCommand {
  reason: string;
}