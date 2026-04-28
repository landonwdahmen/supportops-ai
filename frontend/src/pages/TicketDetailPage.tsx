import { FormEvent, useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { addTicketComment, getTicket } from "../api/ticketsApi";
import { Badge } from "../components/Badge";
import { StatusMessage } from "../components/StatusMessage";
import { TriagePanel } from "../components/TriagePanel";
import type { TicketDetailResponse } from "../types/tickets";
import { formatDate } from "../utils/format";

export function TicketDetailPage() {
  const { ticketId } = useParams();
  const [ticket, setTicket] = useState<TicketDetailResponse | null>(null);
  const [comment, setComment] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [isCommenting, setIsCommenting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  async function loadTicket() {
    if (!ticketId) {
      return;
    }

    setError(null);
    try {
      setTicket(await getTicket(ticketId));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to load ticket.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    loadTicket();
  }, [ticketId]);

  async function handleComment(event: FormEvent) {
    event.preventDefault();
    if (!ticketId || !comment.trim()) {
      return;
    }

    setIsCommenting(true);
    setError(null);
    setMessage(null);
    try {
      await addTicketComment(ticketId, { body: comment });
      setComment("");
      await loadTicket();
      setMessage("Comment added.");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to add comment.");
    } finally {
      setIsCommenting(false);
    }
  }

  if (isLoading) {
    return <section className="panel">Loading ticket...</section>;
  }

  if (!ticket || !ticketId) {
    return (
      <section className="panel">
        <StatusMessage message={error ?? "Ticket not found."} tone="error" />
        <Link to="/tickets">Back to tickets</Link>
      </section>
    );
  }

  return (
    <section className="content-stack">
      <Link className="back-link" to="/tickets">Back to tickets</Link>
      <div className="panel">
        <div className="section-heading">
          <div>
            <p className="eyebrow">Ticket detail</p>
            <h1>{ticket.title}</h1>
            <p className="muted">Created {formatDate(ticket.createdAt)} by {ticket.createdByDisplayName ?? "Unknown"}</p>
          </div>
          <div className="ticket-meta">
            <Badge tone="blue">{ticket.status}</Badge>
            <Badge tone={ticket.priority === "Urgent" || ticket.priority === "High" ? "red" : "neutral"}>{ticket.priority}</Badge>
            <Badge>{ticket.category}</Badge>
          </div>
        </div>
        <p className="description">{ticket.description}</p>
      </div>

      <TriagePanel ticketId={ticketId} onTicketChanged={loadTicket} />

      <section className="panel">
        <div className="section-heading">
          <div>
            <p className="eyebrow">Conversation</p>
            <h2>Comments</h2>
          </div>
        </div>
        <StatusMessage message={message} tone="success" />
        <StatusMessage message={error} tone="error" />
        <div className="comments">
          {ticket.comments.length === 0 ? (
            <p className="muted">No comments yet.</p>
          ) : (
            ticket.comments.map((item) => (
              <article className="comment" key={item.id}>
                <strong>{item.authorDisplayName ?? "Unknown"}</strong>
                <span>{formatDate(item.createdAt)}</span>
                <p>{item.body}</p>
              </article>
            ))
          )}
        </div>
        <form onSubmit={handleComment} className="inline-form">
          <label>
            Add comment
            <textarea value={comment} onChange={(event) => setComment(event.target.value)} rows={3} required />
          </label>
          <button className="button" disabled={isCommenting}>{isCommenting ? "Adding..." : "Add comment"}</button>
        </form>
      </section>
    </section>
  );
}
