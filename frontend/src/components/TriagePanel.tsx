import { FormEvent, useEffect, useState } from "react";
import { ApiError } from "../api/apiClient";
import { approveTriage, editTriage, getTriageResult, rejectTriage, retryTriage } from "../api/triageApi";
import { useAuth } from "../auth/AuthContext";
import type { TicketCategory, TicketPriority } from "../types/common";
import type { TriageResultResponse } from "../types/triage";
import { categoryOptions, priorityOptions } from "../utils/options";
import { Badge } from "./Badge";
import { StatusMessage } from "./StatusMessage";

type TriagePanelProps = {
  ticketId: string;
  onTicketChanged: () => Promise<void>;
};

export function TriagePanel({ ticketId, onTicketChanged }: TriagePanelProps) {
  const { user } = useAuth();
  const [triage, setTriage] = useState<TriageResultResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [notes, setNotes] = useState("");
  const [rejectReason, setRejectReason] = useState("");
  const [editCategory, setEditCategory] = useState<TicketCategory>("Technical");
  const [editPriority, setEditPriority] = useState<TicketPriority>("High");

  const canReview = user?.role === "Agent" || user?.role === "Admin";

  async function loadTriage() {
    setIsLoading(true);
    setError(null);
    try {
      const result = await getTriageResult(ticketId);
      setTriage(result);
      setEditCategory(result.suggestedCategory);
      setEditPriority(result.suggestedPriority);
    } catch (err) {
      if (err instanceof ApiError && err.status === 404) {
        setTriage(null);
        setError("No triage result yet. Make sure the worker is running, or retry triage as an agent.");
      } else {
        setError(err instanceof Error ? err.message : "Unable to load triage result.");
      }
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    loadTriage();
  }, [ticketId]);

  async function runAction(action: () => Promise<unknown>, success: string) {
    setIsSaving(true);
    setError(null);
    setMessage(null);
    try {
      await action();
      await loadTriage();
      await onTicketChanged();
      setMessage(success);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Action failed.");
    } finally {
      setIsSaving(false);
    }
  }

  async function handleApprove(event: FormEvent) {
    event.preventDefault();
    await runAction(() => approveTriage(ticketId, { notes: notes || null }), "Triage approved and applied to the ticket.");
  }

  async function handleEdit(event: FormEvent) {
    event.preventDefault();
    await runAction(
      () => editTriage(ticketId, { category: editCategory, priority: editPriority, notes: notes || null }),
      "Triage edited and applied to the ticket."
    );
  }

  async function handleReject(event: FormEvent) {
    event.preventDefault();
    await runAction(() => rejectTriage(ticketId, { reason: rejectReason }), "Triage recommendation rejected.");
  }

  if (isLoading) {
    return <section className="panel">Loading triage...</section>;
  }

  return (
    <section className="panel">
      <div className="section-heading">
        <div>
          <p className="eyebrow">AI triage</p>
          <h2>Recommendation</h2>
        </div>
        {triage && <Badge tone={triage.reviewStatus === "PendingReview" ? "amber" : "green"}>{triage.reviewStatus}</Badge>}
      </div>

      <StatusMessage message={message} tone="success" />
      <StatusMessage message={error} tone="error" />

      {triage ? (
        <div className="triage-grid">
          <div className="metric">
            <span>Suggested category</span>
            <strong>{triage.suggestedCategory}</strong>
          </div>
          <div className="metric">
            <span>Suggested priority</span>
            <strong>{triage.suggestedPriority}</strong>
          </div>
          <div className="metric">
            <span>Confidence</span>
            <strong>{Math.round(triage.confidenceScore * 100)}%</strong>
          </div>
          <div className="triage-copy">
            <h3>Reasoning</h3>
            <p>{triage.reasoningSummary}</p>
          </div>
          <div className="triage-copy">
            <h3>Suggested steps</h3>
            <p>{triage.suggestedSteps}</p>
          </div>
          {triage.reviewNotes && (
            <div className="triage-copy">
              <h3>Review notes</h3>
              <p>{triage.reviewNotes}</p>
            </div>
          )}
        </div>
      ) : (
        <p className="muted">Triage will appear here after the worker processes the ticket job.</p>
      )}

      {canReview && (
        <div className="review-actions">
          <form onSubmit={handleApprove} className="inline-form">
            <label>
              Review notes
              <textarea value={notes} onChange={(event) => setNotes(event.target.value)} rows={3} />
            </label>
            <button className="button" disabled={isSaving || !triage}>Approve</button>
          </form>

          <form onSubmit={handleEdit} className="inline-form">
            <div className="form-row">
              <label>
                Category
                <select value={editCategory} onChange={(event) => setEditCategory(event.target.value as TicketCategory)}>
                  {categoryOptions.map((category) => (
                    <option key={category} value={category}>{category}</option>
                  ))}
                </select>
              </label>
              <label>
                Priority
                <select value={editPriority} onChange={(event) => setEditPriority(event.target.value as TicketPriority)}>
                  {priorityOptions.map((priority) => (
                    <option key={priority} value={priority}>{priority}</option>
                  ))}
                </select>
              </label>
            </div>
            <button className="button button-secondary" disabled={isSaving || !triage}>Edit and apply</button>
          </form>

          <form onSubmit={handleReject} className="inline-form">
            <label>
              Rejection reason
              <textarea value={rejectReason} onChange={(event) => setRejectReason(event.target.value)} rows={2} required />
            </label>
            <div className="button-row">
              <button className="button button-danger" disabled={isSaving || !triage}>Reject</button>
              <button
                className="button button-secondary"
                disabled={isSaving}
                type="button"
                onClick={() => runAction(() => retryTriage(ticketId), "Triage retry queued. Refresh after the worker completes.")}
              >
                Retry triage
              </button>
            </div>
          </form>
        </div>
      )}
    </section>
  );
}
