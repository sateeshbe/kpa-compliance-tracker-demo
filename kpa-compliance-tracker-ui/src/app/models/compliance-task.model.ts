export interface ComplianceTask {
    id: string;
    title: string;
    category?: string;
    site?: string;
    owner?: string;
    dueDate?: string; 
    status?: string;
    updatedAt: string;
    sourceS3Key?: string; 
  }
  