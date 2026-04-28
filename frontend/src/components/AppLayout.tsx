import { Link, NavLink, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export function AppLayout() {
  const { logout, user } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <div className="app-shell">
      <header className="topbar">
        <Link className="brand" to="/tickets">
          <span className="brand-mark">S</span>
          <span>
            <strong>SupportOps AI</strong>
            <small>Backend demo UI</small>
          </span>
        </Link>
        <nav className="nav-links">
          {user ? (
            <>
              <NavLink to="/tickets">Tickets</NavLink>
              <NavLink to="/tickets/new">Create</NavLink>
              <span className="user-chip">{user.displayName} · {user.role}</span>
              <button className="button button-secondary" onClick={handleLogout}>Logout</button>
            </>
          ) : (
            <>
              <NavLink to="/login">Login</NavLink>
              <NavLink to="/register">Register</NavLink>
            </>
          )}
        </nav>
      </header>
      <main className="page">
        <Outlet />
      </main>
    </div>
  );
}
