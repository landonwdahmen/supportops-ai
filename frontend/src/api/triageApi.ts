import { apiRequest } from "./apiClient";
import type { ApproveTriageRequest, EditTriageRequest, RejectTriageRequest, RetryTriageResponse, TriageResultResponse } from "../types/triage";

export function getTriageResult(ticketId: string) {
  return apiRequest<TriageResultResponse>(`/tickets/${ticketId}/triage`);
}

export function approveTriage(ticketId: string, request: ApproveTriageRequest) {
  return apiRequest<TriageResultResponse>(`/tickets/${ticketId}/triage/approve`, {
    method: "POST",
    body: request
  });
}

export function editTriage(ticketId: string, request: EditTriageRequest) {
  return apiRequest<TriageResultResponse>(`/tickets/${ticketId}/triage/edit`, {
    method: "POST",
    body: request
  });
}

export function rejectTriage(ticketId: string, request: RejectTriageRequest) {
  return apiRequest<TriageResultResponse>(`/tickets/${ticketId}/triage/reject`, {
    method: "POST",
    body: request
  });
}

export function retryTriage(ticketId: string) {
  return apiRequest<RetryTriageResponse>(`/tickets/${ticketId}/triage/retry`, {
    method: "POST"
  });
}
