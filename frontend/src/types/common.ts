export type UserRole = "Customer" | "Agent" | "Admin";
export type TicketStatus = "New" | "PendingTriage" | "Open" | "InProgress" | "TriageCompleted" | "Resolved" | "Closed";
export type TicketPriority = "Low" | "Medium" | "High" | "Urgent";
export type TicketCategory = "General" | "Billing" | "Technical" | "Account" | "Bug" | "FeatureRequest";
export type TriageReviewStatus = "PendingReview" | "Accepted" | "Rejected" | "Revised";
