import { FormEvent, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { StatusMessage } from "../components/StatusMessage";

const customerDemo = { email: "customer1@example.com", password: "Password123!" };
const agentDemo = { email: "agent@supportops.local", password: "AgentPassword123!" };

export function LoginPage() {
  const { loginUser } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [email, setEmail] = useState(customerDemo.email);
  const [password, setPassword] = useState(customerDemo.password);
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const destination = (location.state as { from?: { pathname?: string } } | null)?.from?.pathname ?? "/tickets";

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);
    try {
      await loginUser({ email, password });
      navigate(destination, { replace: true });
    } catch (err) {
      setError(err instanceof Error ? err.message : "Login failed.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="auth-page">
      <div className="panel auth-card">
        <p className="eyebrow">Demo access</p>
        <h1>Log in</h1>
        <p className="muted">Use a registered customer account, or the seeded development agent account.</p>
        <p className="muted">Demo credentials are for local development only.</p>
        <div className="quick-actions">
          <button className="button button-secondary" type="button" onClick={() => { setEmail(customerDemo.email); setPassword(customerDemo.password); }}>
            Local demo customer
          </button>
          <button className="button button-secondary" type="button" onClick={() => { setEmail(agentDemo.email); setPassword(agentDemo.password); }}>
            Local demo agent
          </button>
        </div>
        <StatusMessage message={error} tone="error" />
        <form onSubmit={handleSubmit} className="form-stack">
          <label>
            Email
            <input value={email} onChange={(event) => setEmail(event.target.value)} type="email" required />
          </label>
          <label>
            Password
            <input value={password} onChange={(event) => setPassword(event.target.value)} type="password" required />
          </label>
          <button className="button" disabled={isSubmitting}>{isSubmitting ? "Logging in..." : "Login"}</button>
        </form>
        <p className="muted">Need a customer account? <Link to="/register">Register one</Link>.</p>
      </div>
    </section>
  );
}
