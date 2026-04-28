import { FormEvent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { StatusMessage } from "../components/StatusMessage";

export function RegisterPage() {
  const { registerUser } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("customer1@example.com");
  const [displayName, setDisplayName] = useState("Customer One");
  const [password, setPassword] = useState("Password123!");
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);
    try {
      await registerUser({ email, displayName, password, role: "Customer" });
      navigate("/tickets");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Registration failed.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="auth-page">
      <div className="panel auth-card">
        <p className="eyebrow">Customer flow</p>
        <h1>Register customer</h1>
        <p className="muted">Creates a local customer account through the existing auth API.</p>
        <StatusMessage message={error} tone="error" />
        <form onSubmit={handleSubmit} className="form-stack">
          <label>
            Display name
            <input value={displayName} onChange={(event) => setDisplayName(event.target.value)} required />
          </label>
          <label>
            Email
            <input value={email} onChange={(event) => setEmail(event.target.value)} type="email" required />
          </label>
          <label>
            Password
            <input value={password} onChange={(event) => setPassword(event.target.value)} type="password" minLength={8} required />
          </label>
          <button className="button" disabled={isSubmitting}>{isSubmitting ? "Creating..." : "Create account"}</button>
        </form>
        <p className="muted">Already registered? <Link to="/login">Log in</Link>.</p>
      </div>
    </section>
  );
}
