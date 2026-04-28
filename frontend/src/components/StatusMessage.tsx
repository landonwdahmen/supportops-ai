type StatusMessageProps = {
  message: string | null;
  tone?: "error" | "success" | "info";
};

export function StatusMessage({ message, tone = "info" }: StatusMessageProps) {
  if (!message) {
    return null;
  }

  return <div className={`status-message ${tone}`}>{message}</div>;
}
