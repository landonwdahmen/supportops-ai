import type { TicketCategory, TicketPriority, TriageReviewStatus } from "./common";

export type TriageResultResponse = {
  id: string;
  ticketId: string;
  suggestedCategory: TicketCategory;
  suggestedPriority: TicketPriority;
  confidenceScore: number;
  reasoningSummary: string;
  suggestedSteps: string;
  reviewStatus: TriageReviewStatus;
  reviewedByUserId: string | null;
  reviewNotes: string | null;
  createdAt: string;
  reviewedAt: string | null;
};

export type ApproveTriageRequest = {
  notes: string | null;
};

export type EditTriageRequest = {
  category: TicketCategory;
  priority: TicketPriority;
  notes: string | null;
};

export type RejectTriageRequest = {
  reason: string;
};

export type RetryTriageResponse = {
  triageJobId: string;
  ticketId: string;
  status: string;
  correlationId: string;
};
