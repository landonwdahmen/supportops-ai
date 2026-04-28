import { Link } from "react-router-dom";

export function NotFoundPage() {
  return (
    <section className="panel empty-state">
      <h1>Page not found</h1>
      <p>The demo page you requested does not exist.</p>
      <Link className="button" to="/tickets">Go to tickets</Link>
    </section>
  );
}
