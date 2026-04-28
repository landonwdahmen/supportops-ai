import { FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import { createTicket } from "../api/ticketsApi";
import { StatusMessage } from "../components/StatusMessage";
import type { TicketCategory, TicketPriority } from "../types/common";
import { categoryOptions, priorityOptions } from "../utils/options";

export function CreateTicketPage() {
  const navigate = useNavigate();
  const [title, setTitle] = useState("Cannot access billing dashboard");
  const [description, setDescription] = useState("I can sign in, but the billing dashboard shows a blank page after the latest invoice email.");
  const [priority, setPriority] = useState<TicketPriority>("Medium");
  const [category, setCategory] = useState<TicketCategory>("General");
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);
    try {
      const ticket = await createTicket({ title, description, priority, category });
      navigate(`/tickets/${ticket.id}`);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to create ticket.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="content-stack">
      <div className="page-heading">
        <div>
          <p className="eyebrow">Customer flow</p>
          <h1>Create ticket</h1>
        </div>
      </div>

      <div className="panel">
        <StatusMessage message={error} tone="error" />
        <form onSubmit={handleSubmit} className="form-stack">
          <label>
            Title
            <input value={title} onChange={(event) => setTitle(event.target.value)} maxLength={200} required />
          </label>
          <label>
            Description
            <textarea value={description} onChange={(event) => setDescription(event.target.value)} rows={6} maxLength={5000} required />
          </label>
          <div className="form-row">
            <label>
              Priority
              <select value={priority} onChange={(event) => setPriority(event.target.value as TicketPriority)}>
                {priorityOptions.map((option) => (
                  <option key={option} value={option}>{option}</option>
                ))}
              </select>
            </label>
            <label>
              Category
              <select value={category} onChange={(event) => setCategory(event.target.value as TicketCategory)}>
                {categoryOptions.map((option) => (
                  <option key={option} value={option}>{option}</option>
                ))}
              </select>
            </label>
          </div>
          <button className="button" disabled={isSubmitting}>{isSubmitting ? "Creating..." : "Create and queue triage"}</button>
        </form>
      </div>
    </section>
  );
}
