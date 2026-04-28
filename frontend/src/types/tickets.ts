import type { TicketCategory, TicketPriority, TicketStatus } from "./common";

export type CreateTicketRequest = {
  title: string;
  description: string;
  priority: TicketPriority;
  category: TicketCategory;
};

export type TicketResponse = {
  id: string;
  title: string;
  status: TicketStatus;
  priority: TicketPriority;
  category: TicketCategory;
  createdByUserId: string;
  assignedToUserId: string | null;
  createdAt: string;
};

export type TicketDetailResponse = TicketResponse & {
  description: string;
  createdByDisplayName: string | null;
  assignedToDisplayName: string | null;
  updatedAt: string | null;
  comments: TicketCommentResponse[];
};

export type TicketCommentRequest = {
  body: string;
};

export type TicketCommentResponse = {
  id: string;
  ticketId: string;
  authorUserId: string;
  authorDisplayName: string | null;
  body: string;
  createdAt: string;
};
