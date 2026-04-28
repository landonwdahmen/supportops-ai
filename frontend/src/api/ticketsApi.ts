import { apiRequest } from "./apiClient";
import type { CreateTicketRequest, TicketCommentRequest, TicketCommentResponse, TicketDetailResponse, TicketResponse } from "../types/tickets";

export function createTicket(request: CreateTicketRequest) {
  return apiRequest<TicketResponse>("/tickets", {
    method: "POST",
    body: request
  });
}

export function getTickets() {
  return apiRequest<TicketResponse[]>("/tickets");
}

export function getTicket(ticketId: string) {
  return apiRequest<TicketDetailResponse>(`/tickets/${ticketId}`);
}

export function addTicketComment(ticketId: string, request: TicketCommentRequest) {
  return apiRequest<TicketCommentResponse>(`/tickets/${ticketId}/comments`, {
    method: "POST",
    body: request
  });
}
