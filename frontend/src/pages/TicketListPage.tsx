import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getTickets } from "../api/ticketsApi";
import { Badge } from "../components/Badge";
import { StatusMessage } from "../components/StatusMessage";
import type { TicketResponse } from "../types/tickets";
import { formatDate } from "../utils/format";

export function TicketListPage() {
  const [tickets, setTickets] = useState<TicketResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function loadTickets() {
      try {
        setTickets(await getTickets());
      } catch (err) {
        setError(err instanceof Error ? err.message : "Unable to load tickets.");
      } finally {
        setIsLoading(false);
      }
    }

    loadTickets();
  }, []);

  return (
    <section className="content-stack">
      <div className="page-heading">
        <div>
          <p className="eyebrow">Support queue</p>
          <h1>Tickets</h1>
        </div>
        <Link className="button" to="/tickets/new">Create ticket</Link>
      </div>

      <StatusMessage message={error} tone="error" />

      {isLoading ? (
        <div className="panel">Loading tickets...</div>
      ) : tickets.length === 0 ? (
        <div className="panel empty-state">
          <h2>No tickets yet</h2>
          <p>Create a customer ticket to queue AI triage.</p>
          <Link className="button" to="/tickets/new">Create ticket</Link>
        </div>
      ) : (
        <div className="ticket-list">
          {tickets.map((ticket) => (
            <Link className="ticket-row" key={ticket.id} to={`/tickets/${ticket.id}`}>
              <div>
                <h2>{ticket.title}</h2>
                <p>{formatDate(ticket.createdAt)}</p>
              </div>
              <div className="ticket-meta">
                <Badge tone="blue">{ticket.status}</Badge>
                <Badge tone={ticket.priority === "Urgent" || ticket.priority === "High" ? "red" : "neutral"}>{ticket.priority}</Badge>
                <Badge>{ticket.category}</Badge>
              </div>
            </Link>
          ))}
        </div>
      )}
    </section>
  );
}
